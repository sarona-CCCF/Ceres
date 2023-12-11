﻿#region License notice

/*
  This file is part of the Ceres project at https://github.com/dje-dev/ceres.
  Copyright (C) 2020- by David Elliott and the Ceres Authors.

  Ceres is free software under the terms of the GNU General Public License v3.0.
  You should have received a copy of the GNU General Public License
  along with Ceres. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

#region Using directives

using System;
using System.Collections.Generic;
using System.Threading;
using Ceres.Base.OperatingSystem.Windows;
using Ceres.Chess.GameEngines;
using Ceres.Chess.MoveGen;
using Ceres.Chess.Positions;
using Ceres.Chess.UserSettings;
using static Ceres.Chess.NNEvaluators.LC0DLL.LC0DLLSyzygyEvaluator;

#endregion

namespace Ceres.Chess.NNEvaluators.LC0DLL
{
  public enum WDLResult
  {
    Uninitialized,
    Unknown,
    Loss,
    Draw,
    Win
  }


  /// <summary>
  /// Interface shared by all Syzygy evaluators (and some shared logic).
  /// </summary>
  public interface ISyzygyEvaluatorEngine
  {
    /// <summary>
    /// Maximum number of pieces of available tablebase positions.
    /// </summary>
    public int MaxCardinality { get; }


    /// <summary>
    /// If the DTZ files supported by the engine and potentially usable
    /// (if the necessary tablebase files are found for a given piece combination).
    /// The case of partial DTZ availability is not generally supported
    /// (not guaranteed to produce correct play).
    /// </summary>
    public bool DTZAvailable { get;}

    /// <summary>
    /// The number of wdl tablebase files available, if known.
    /// </summary>
    public int? NumWDLTablebaseFiles { get; }

    /// <summary>
    /// The number of DTZ tablebase files available, if known.
    /// </summary>
    public int? NumDTZTablebaseFiles { get; }

    /// <summary>
    /// Initializes tablebases to use a specied set of paths.
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public bool Initialize(string paths);


    /// <summary>
    /// Probes the win/draw/loss information for a specified position.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="score"></param>
    /// <param name="result"></param>
    public void ProbeWDL(in Position pos, out SyzygyWDLScore score, out SyzygyProbeState result);


    /// <summary>
    /// Probes the distance to zero tables to find the suggested move to play.
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="result"></param>
    /// <param name="moveList"></param>
    /// <param name="dtz"></param>
    /// <param name="returnOnlyWinningMoves"></param>
    /// <returns></returns>
    public MGMove CheckTablebaseBestNextMoveViaDTZ(in Position currentPos, out WDLResult result, 
                                                   out List<(MGMove, short)> moveList, 
                                                   out short dtz, bool returnOnlyWinningMoves = true);


    /// <summary>
    /// Shuts down the evaluators (releasing associated resources).
    /// </summary>
    public void Dispose();


    #region Helper interface  methods

    const bool VERBOSE = false;

    /// <summary>
    /// Returns the default Syzygy evaluator engine.
    /// </summary>
    public static ISyzygyEvaluatorEngine DefaultEngine => SyzygyEvaluatorPool.GetSessionForPaths(CeresUserSettingsManager.Settings.SyzygyPath);


    /// <summary>
    /// Probes tablebase for WDL and returns as an integer for V (-1 = loss, 0 = draw, 1 = win).
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="score"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public int ProbeWDLAsV(in Position pos)
    {
      ProbeWDL(in pos, out SyzygyWDLScore score, out SyzygyProbeState result);
      if (result == SyzygyProbeState.Fail)
      {
        throw new Exception("Failure on tablebase WDL probe of position " + pos.FEN);
      }

      return score switch
      {
        SyzygyWDLScore.WDLLoss => -1,
        SyzygyWDLScore.WDLWin => 1,
        _ => 0
      };
    }


    /// <summary>
    /// Returns if the specified move is optimal in a given position, 
    /// in the sense that it preserves the best possible outcome in the position (win, draw, or loss).
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="move"></param>
    /// <returns></returns>
    public bool MoveIsInOptimalCategoryForPosition(in Position pos, MGMove move)
    {
      // Compute new position.
      MGPosition mgPos = pos.ToMGPosition;
      mgPos.MakeMove(move);
      Position newPos = mgPos.ToPosition;

      // Check if WDL status for current and new position is same (after negation to adjust for perspective change).
      return ProbeWDLAsV(in pos) == -ProbeWDLAsV(in newPos);
    }


    /// <summary>
    /// Returns best winning move (if any) from a specified position
    /// (and possibly a list of all possible winning moves, with shortest mates first).
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="result"></param>
    /// <param name="fullWinningMoveList"></param>
    /// <returns></returns>
    public MGMove CheckTablebaseBestNextMove(in Position currentPos, out WDLResult result, out List<(MGMove, short)> fullWinningMoveList, out bool winningMoveListOrderedByDTM)
    {
      fullWinningMoveList = null;
      winningMoveListOrderedByDTM = false;

      // TODO: Ponder if this could be done for PieceCount == MaxCardinality + 1
      if (currentPos.PieceCount > MaxCardinality)
      {
        result = WDLResult.Unknown;
        winningMoveListOrderedByDTM = false;
        return default;
      }

      if (DTZAvailable)
      {
        // Try to use DTZ table, which may may not work (depending on file availability).
        MGMove dtzMove = CheckTablebaseBestNextMoveViaDTZ(in currentPos, out result, out fullWinningMoveList, out _);

        if (result != WDLResult.Unknown)
        {
          if (VERBOSE)
          {
            Console.WriteLine($"\r\nCheckTablebaseBestNextMove via DTZ yields {result} {dtzMove} with winning list "
                                       + $"size {fullWinningMoveList?.Count} in position {currentPos.FEN}");
          }

          winningMoveListOrderedByDTM = true;
          return dtzMove;
        }
      }

      // Fall thru to use WDL
      MGMove ret =  CheckTablebaseBestNextMoveViaWDL(in currentPos, out result, out List<MGMove> winningMoves);
      if (result == WDLResult.Unknown)
      {
        return default;
      } 

      if (fullWinningMoveList != null)
      {
        foreach (MGMove move in winningMoves)
        {
          fullWinningMoveList.Add((move, 1));
        }
      }

      if (VERBOSE)
      {
        Console.WriteLine($"\r\nCheckTablebaseBestNextMove via WDL yields {result} {ret} with winning list "
                                   + $"size {fullWinningMoveList?.Count} in position {currentPos.FEN}");
      }

      return ret;
    }


    MGMove CheckTablebaseBestNextMoveViaWDL(in Position currentPos, out WDLResult result, out List<MGMove> fullWinningMoveList)
    {
      fullWinningMoveList = new();

      // First check for immediate winning or drawing moves known by TB probe
      MGMove winningMove = default;
      MGMove winningCursedMove = default;
      MGMove drawingMove = default;
      bool allNextPositionsInTablebase = true;

      // Generate all possible next moves and look up in tablebase
      foreach ((MGMove move, Position nextPos) in PositionsGenerator1Ply.GenPositions(currentPos))
      {
        ProbeWDL(in nextPos, out SyzygyWDLScore score, out SyzygyProbeState probeResult);
        if (!(probeResult == SyzygyProbeState.Ok || probeResult == SyzygyProbeState.ZeroingBestMove))
        {
          allNextPositionsInTablebase = false;
          continue;
        }

        switch (score)
        {
          case SyzygyWDLScore.WDLBlessedLoss: // blessed loss for the opponent
            winningCursedMove = move;
            break;

          case SyzygyWDLScore.WDLLoss: // loss for the opponent
            fullWinningMoveList.Add(move);
            winningMove = move;
            break;

          case SyzygyWDLScore.WDLDraw:
            drawingMove = move;
            break;

          default:
            break;
        }
      }

      MGMove bestMove = default;
      if (winningMove != default(MGMove))
      {
        // If we found a winning move, definitely make it 
        bestMove = winningMove;
        result = WDLResult.Win;
      }
      else if (winningCursedMove != default(MGMove))
      {
        // If we found a cursed winning move, we might as well try making it
        bestMove = winningCursedMove;
        result = WDLResult.Draw;
      }
      else if (!allNextPositionsInTablebase)
      {
        // Unable to determine result because not all possible next positions were found in tablebase.
        result = WDLResult.Unknown;
      }
      else if (drawingMove != default(MGMove))
      {
        // If we were able to find all next positions and none are winning, 
        // then take (any) drawing move if we found it
        bestMove = drawingMove;
        result = WDLResult.Draw;
      }
      else 
      {
        bestMove = default; // all moves lose, we don't know which is least bad
        result = WDLResult.Loss;
      }

      return bestMove;
    }

#endregion
  }

}
