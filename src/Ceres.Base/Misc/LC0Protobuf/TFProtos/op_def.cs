// This file was generated by a tool; you should avoid making direct changes.
// Consider using 'partial classes' to extend these types
// Input: op_def.proto

#pragma warning disable CS0612, CS1591, CS3021, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
namespace Tensorflow
{

    [global::ProtoBuf.ProtoContract()]
    public partial class OpDef : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"name")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Name { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2, Name = @"input_arg")]
        public global::System.Collections.Generic.List<ArgDef> InputArgs { get; } = new global::System.Collections.Generic.List<ArgDef>();

        [global::ProtoBuf.ProtoMember(3, Name = @"output_arg")]
        public global::System.Collections.Generic.List<ArgDef> OutputArgs { get; } = new global::System.Collections.Generic.List<ArgDef>();

        [global::ProtoBuf.ProtoMember(4, Name = @"attr")]
        public global::System.Collections.Generic.List<AttrDef> Attrs { get; } = new global::System.Collections.Generic.List<AttrDef>();

        [global::ProtoBuf.ProtoMember(8, Name = @"deprecation")]
        public OpDeprecation Deprecation { get; set; }

        [global::ProtoBuf.ProtoMember(5, Name = @"summary")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Summary { get; set; } = "";

        [global::ProtoBuf.ProtoMember(6, Name = @"description")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Description { get; set; } = "";

        [global::ProtoBuf.ProtoMember(18, Name = @"is_commutative")]
        public bool IsCommutative { get; set; }

        [global::ProtoBuf.ProtoMember(16, Name = @"is_aggregate")]
        public bool IsAggregate { get; set; }

        [global::ProtoBuf.ProtoMember(17, Name = @"is_stateful")]
        public bool IsStateful { get; set; }

        [global::ProtoBuf.ProtoMember(19, Name = @"allows_uninitialized_input")]
        public bool AllowsUninitializedInput { get; set; }

        [global::ProtoBuf.ProtoContract()]
        public partial class ArgDef : global::ProtoBuf.IExtensible
        {
            private global::ProtoBuf.IExtension __pbn__extensionData;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

            [global::ProtoBuf.ProtoMember(1, Name = @"name")]
            [global::System.ComponentModel.DefaultValue("")]
            public string Name { get; set; } = "";

            [global::ProtoBuf.ProtoMember(2, Name = @"description")]
            [global::System.ComponentModel.DefaultValue("")]
            public string Description { get; set; } = "";

            [global::ProtoBuf.ProtoMember(3, Name = @"type")]
            public DataType Type { get; set; }

            [global::ProtoBuf.ProtoMember(4, Name = @"type_attr")]
            [global::System.ComponentModel.DefaultValue("")]
            public string TypeAttr { get; set; } = "";

            [global::ProtoBuf.ProtoMember(5, Name = @"number_attr")]
            [global::System.ComponentModel.DefaultValue("")]
            public string NumberAttr { get; set; } = "";

            [global::ProtoBuf.ProtoMember(6, Name = @"type_list_attr")]
            [global::System.ComponentModel.DefaultValue("")]
            public string TypeListAttr { get; set; } = "";

            [global::ProtoBuf.ProtoMember(16, Name = @"is_ref")]
            public bool IsRef { get; set; }

        }

        [global::ProtoBuf.ProtoContract()]
        public partial class AttrDef : global::ProtoBuf.IExtensible
        {
            private global::ProtoBuf.IExtension __pbn__extensionData;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

            [global::ProtoBuf.ProtoMember(1, Name = @"name")]
            [global::System.ComponentModel.DefaultValue("")]
            public string Name { get; set; } = "";

            [global::ProtoBuf.ProtoMember(2, Name = @"type")]
            [global::System.ComponentModel.DefaultValue("")]
            public string Type { get; set; } = "";

            [global::ProtoBuf.ProtoMember(3, Name = @"default_value")]
            public AttrValue DefaultValue { get; set; }

            [global::ProtoBuf.ProtoMember(4, Name = @"description")]
            [global::System.ComponentModel.DefaultValue("")]
            public string Description { get; set; } = "";

            [global::ProtoBuf.ProtoMember(5, Name = @"has_minimum")]
            public bool HasMinimum { get; set; }

            [global::ProtoBuf.ProtoMember(6, Name = @"minimum")]
            public long Minimum { get; set; }

            [global::ProtoBuf.ProtoMember(7, Name = @"allowed_values")]
            public AttrValue AllowedValues { get; set; }

        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class OpDeprecation : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"version")]
        public int Version { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"explanation")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Explanation { get; set; } = "";

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class OpList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"op")]
        public global::System.Collections.Generic.List<OpDef> Ops { get; } = new global::System.Collections.Generic.List<OpDef>();

    }

}

#pragma warning restore CS0612, CS1591, CS3021, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
