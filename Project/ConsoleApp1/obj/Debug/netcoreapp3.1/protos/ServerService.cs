// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: protos/ServerService.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from protos/ServerService.proto</summary>
public static partial class ServerServiceReflection {

  #region Descriptor
  /// <summary>File descriptor for protos/ServerService.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static ServerServiceReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Chpwcm90b3MvU2VydmVyU2VydmljZS5wcm90byI0CgtSZWFkUmVxdWVzdBIT",
          "CgtwYXJ0aXRpb25JZBgBIAEoCRIQCghvYmplY3RJZBgCIAEoBSIgCglSZWFk",
          "UmVwbHkSEwoLb2JqZWN0VmFsdWUYASABKAkiSgoMV3JpdGVSZXF1ZXN0EhMK",
          "C3BhcnRpdGlvbklkGAEgASgJEhAKCG9iamVjdElkGAIgASgFEhMKC29iamVj",
          "dFZhbHVlGAMgASgJIhgKCldyaXRlUmVwbHkSCgoCb2sYASABKAgyVgoNU2Vy",
          "dmVyU2VydmljZRIgCgRSZWFkEgwuUmVhZFJlcXVlc3QaCi5SZWFkUmVwbHkS",
          "IwoFV3JpdGUSDS5Xcml0ZVJlcXVlc3QaCy5Xcml0ZVJlcGx5YgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::ReadRequest), global::ReadRequest.Parser, new[]{ "PartitionId", "ObjectId" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::ReadReply), global::ReadReply.Parser, new[]{ "ObjectValue" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::WriteRequest), global::WriteRequest.Parser, new[]{ "PartitionId", "ObjectId", "ObjectValue" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::WriteReply), global::WriteReply.Parser, new[]{ "Ok" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class ReadRequest : pb::IMessage<ReadRequest>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<ReadRequest> _parser = new pb::MessageParser<ReadRequest>(() => new ReadRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ReadRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ServerServiceReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ReadRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ReadRequest(ReadRequest other) : this() {
    partitionId_ = other.partitionId_;
    objectId_ = other.objectId_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ReadRequest Clone() {
    return new ReadRequest(this);
  }

  /// <summary>Field number for the "partitionId" field.</summary>
  public const int PartitionIdFieldNumber = 1;
  private string partitionId_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string PartitionId {
    get { return partitionId_; }
    set {
      partitionId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "objectId" field.</summary>
  public const int ObjectIdFieldNumber = 2;
  private int objectId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int ObjectId {
    get { return objectId_; }
    set {
      objectId_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ReadRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ReadRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (PartitionId != other.PartitionId) return false;
    if (ObjectId != other.ObjectId) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (PartitionId.Length != 0) hash ^= PartitionId.GetHashCode();
    if (ObjectId != 0) hash ^= ObjectId.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (PartitionId.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(PartitionId);
    }
    if (ObjectId != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ObjectId);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (PartitionId.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(PartitionId);
    }
    if (ObjectId != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ObjectId);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (PartitionId.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(PartitionId);
    }
    if (ObjectId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ObjectId);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ReadRequest other) {
    if (other == null) {
      return;
    }
    if (other.PartitionId.Length != 0) {
      PartitionId = other.PartitionId;
    }
    if (other.ObjectId != 0) {
      ObjectId = other.ObjectId;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          PartitionId = input.ReadString();
          break;
        }
        case 16: {
          ObjectId = input.ReadInt32();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          PartitionId = input.ReadString();
          break;
        }
        case 16: {
          ObjectId = input.ReadInt32();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class ReadReply : pb::IMessage<ReadReply>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<ReadReply> _parser = new pb::MessageParser<ReadReply>(() => new ReadReply());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ReadReply> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ServerServiceReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ReadReply() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ReadReply(ReadReply other) : this() {
    objectValue_ = other.objectValue_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ReadReply Clone() {
    return new ReadReply(this);
  }

  /// <summary>Field number for the "objectValue" field.</summary>
  public const int ObjectValueFieldNumber = 1;
  private string objectValue_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string ObjectValue {
    get { return objectValue_; }
    set {
      objectValue_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ReadReply);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ReadReply other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (ObjectValue != other.ObjectValue) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (ObjectValue.Length != 0) hash ^= ObjectValue.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (ObjectValue.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(ObjectValue);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (ObjectValue.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(ObjectValue);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (ObjectValue.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(ObjectValue);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ReadReply other) {
    if (other == null) {
      return;
    }
    if (other.ObjectValue.Length != 0) {
      ObjectValue = other.ObjectValue;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          ObjectValue = input.ReadString();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          ObjectValue = input.ReadString();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class WriteRequest : pb::IMessage<WriteRequest>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<WriteRequest> _parser = new pb::MessageParser<WriteRequest>(() => new WriteRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<WriteRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ServerServiceReflection.Descriptor.MessageTypes[2]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public WriteRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public WriteRequest(WriteRequest other) : this() {
    partitionId_ = other.partitionId_;
    objectId_ = other.objectId_;
    objectValue_ = other.objectValue_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public WriteRequest Clone() {
    return new WriteRequest(this);
  }

  /// <summary>Field number for the "partitionId" field.</summary>
  public const int PartitionIdFieldNumber = 1;
  private string partitionId_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string PartitionId {
    get { return partitionId_; }
    set {
      partitionId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "objectId" field.</summary>
  public const int ObjectIdFieldNumber = 2;
  private int objectId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int ObjectId {
    get { return objectId_; }
    set {
      objectId_ = value;
    }
  }

  /// <summary>Field number for the "objectValue" field.</summary>
  public const int ObjectValueFieldNumber = 3;
  private string objectValue_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string ObjectValue {
    get { return objectValue_; }
    set {
      objectValue_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as WriteRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(WriteRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (PartitionId != other.PartitionId) return false;
    if (ObjectId != other.ObjectId) return false;
    if (ObjectValue != other.ObjectValue) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (PartitionId.Length != 0) hash ^= PartitionId.GetHashCode();
    if (ObjectId != 0) hash ^= ObjectId.GetHashCode();
    if (ObjectValue.Length != 0) hash ^= ObjectValue.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (PartitionId.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(PartitionId);
    }
    if (ObjectId != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ObjectId);
    }
    if (ObjectValue.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(ObjectValue);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (PartitionId.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(PartitionId);
    }
    if (ObjectId != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ObjectId);
    }
    if (ObjectValue.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(ObjectValue);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (PartitionId.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(PartitionId);
    }
    if (ObjectId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ObjectId);
    }
    if (ObjectValue.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(ObjectValue);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(WriteRequest other) {
    if (other == null) {
      return;
    }
    if (other.PartitionId.Length != 0) {
      PartitionId = other.PartitionId;
    }
    if (other.ObjectId != 0) {
      ObjectId = other.ObjectId;
    }
    if (other.ObjectValue.Length != 0) {
      ObjectValue = other.ObjectValue;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          PartitionId = input.ReadString();
          break;
        }
        case 16: {
          ObjectId = input.ReadInt32();
          break;
        }
        case 26: {
          ObjectValue = input.ReadString();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          PartitionId = input.ReadString();
          break;
        }
        case 16: {
          ObjectId = input.ReadInt32();
          break;
        }
        case 26: {
          ObjectValue = input.ReadString();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class WriteReply : pb::IMessage<WriteReply>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<WriteReply> _parser = new pb::MessageParser<WriteReply>(() => new WriteReply());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<WriteReply> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ServerServiceReflection.Descriptor.MessageTypes[3]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public WriteReply() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public WriteReply(WriteReply other) : this() {
    ok_ = other.ok_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public WriteReply Clone() {
    return new WriteReply(this);
  }

  /// <summary>Field number for the "ok" field.</summary>
  public const int OkFieldNumber = 1;
  private bool ok_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Ok {
    get { return ok_; }
    set {
      ok_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as WriteReply);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(WriteReply other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Ok != other.Ok) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Ok != false) hash ^= Ok.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Ok != false) {
      output.WriteRawTag(8);
      output.WriteBool(Ok);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Ok != false) {
      output.WriteRawTag(8);
      output.WriteBool(Ok);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Ok != false) {
      size += 1 + 1;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(WriteReply other) {
    if (other == null) {
      return;
    }
    if (other.Ok != false) {
      Ok = other.Ok;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Ok = input.ReadBool();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          Ok = input.ReadBool();
          break;
        }
      }
    }
  }
  #endif

}

#endregion


#endregion Designer generated code
