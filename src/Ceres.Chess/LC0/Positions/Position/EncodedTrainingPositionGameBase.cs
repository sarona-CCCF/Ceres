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



#endregion

using Ceres.Chess.LC0.Boards;
using System;

namespace Ceres.Chess.EncodedPositions
{
  /// <summary>
  /// Base class to encapsulate a sequence of EncodedTrainingPosition struct
  /// </summary>
  public abstract class EncodedTrainingPositionGameBase
  {
    /// <summary>
    /// Number of positions in sequence.
    /// </summary>
    public abstract int NumPositions { get; }

    /// <summary>
    /// Version number of file.
    /// </summary>
    public abstract int Version { get; }

    /// <summary>
    ///  Board representation input format.
    /// </summary>
    public abstract int InputFormat { get; }

    /// <summary>
    /// Policies (of length 1858 * 4 bytes).
    /// </summary>
    public abstract EncodedPolicyVector PolicyAtIndex(int index);

    /// <summary>
    /// Returns reference to raw (still mirrored) position at a specified index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected abstract ref readonly EncodedPositionWithHistory PositionRawMirroredRefAtIndex(int index);

    #region Base class methods (helpers)

    /// <summary>
    /// Board position (including history planes).
    /// Note that these LC0 training data files (TARs) contain mirrored positions
    /// (compared to the representation that must be fed to the LC0 neural network).
    /// However this mirroring is undone before the boards are returned by this method.
    /// </summary>
    public EncodedPositionWithHistory PositionAtIndex(int index)
    {
      EncodedPositionWithHistory pos = PositionRawMirroredRefAtIndex(index);
      pos.BoardsHistory.MirrorBoardsInPlace();
      return pos;
    }

    /// <summary>
    /// Returns if the game appears to be a FRC (Fischer random chess) game (does not start with classical starting position).
    /// </summary>
    public bool IsFRCGame => PositionAtIndex(0).FinalPosition != Position.StartPosition;

    public EncodedPositionMiscInfo PositionMiscInfoAtIndex(int index) => PositionRawMirroredRefAtIndex(index).MiscInfo.InfoPosition;

    public EncodedPositionEvalMiscInfoV6 PositionTrainingInfoAtIndex(int index) => PositionRawMirroredRefAtIndex(index).MiscInfo.InfoTraining;

    public delegate bool MatchBoardPredicate(in EncodedPositionBoard board);

    public bool PosExistsWithCondition(MatchBoardPredicate matchBoardPredicate)
    {
      for (int i = NumPositions - 1; i>= 0; i--)
      {
        ref readonly EncodedPositionBoard firstBoard = ref PositionRawMirroredRefAtIndex(i).BoardsHistory.History_0;
        if (matchBoardPredicate(PositionRawMirroredRefAtIndex(i).BoardsHistory.History_0))
        {
          return true;
        }
      }
      return false;
    }
    #endregion
  }
}
