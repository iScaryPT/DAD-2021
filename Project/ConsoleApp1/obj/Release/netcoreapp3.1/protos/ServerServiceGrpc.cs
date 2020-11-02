// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: protos/ServerService.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

public static partial class ServerService
{
  static readonly string __ServiceName = "ServerService";

  static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
  {
    #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
    if (message is global::Google.Protobuf.IBufferMessage)
    {
      context.SetPayloadLength(message.CalculateSize());
      global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
      context.Complete();
      return;
    }
    #endif
    context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
  }

  static class __Helper_MessageCache<T>
  {
    public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
  }

  static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
  {
    #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
    if (__Helper_MessageCache<T>.IsBufferMessage)
    {
      return parser.ParseFrom(context.PayloadAsReadOnlySequence());
    }
    #endif
    return parser.ParseFrom(context.PayloadAsNewBuffer());
  }

  static readonly grpc::Marshaller<global::ReadRequest> __Marshaller_ReadRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ReadRequest.Parser));
  static readonly grpc::Marshaller<global::ReadReply> __Marshaller_ReadReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ReadReply.Parser));
  static readonly grpc::Marshaller<global::WriteRequest> __Marshaller_WriteRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::WriteRequest.Parser));
  static readonly grpc::Marshaller<global::WriteReply> __Marshaller_WriteReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::WriteReply.Parser));

  static readonly grpc::Method<global::ReadRequest, global::ReadReply> __Method_Read = new grpc::Method<global::ReadRequest, global::ReadReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "Read",
      __Marshaller_ReadRequest,
      __Marshaller_ReadReply);

  static readonly grpc::Method<global::WriteRequest, global::WriteReply> __Method_Write = new grpc::Method<global::WriteRequest, global::WriteReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "Write",
      __Marshaller_WriteRequest,
      __Marshaller_WriteReply);

  /// <summary>Service descriptor</summary>
  public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
  {
    get { return global::ServerServiceReflection.Descriptor.Services[0]; }
  }

  /// <summary>Base class for server-side implementations of ServerService</summary>
  [grpc::BindServiceMethod(typeof(ServerService), "BindService")]
  public abstract partial class ServerServiceBase
  {
    public virtual global::System.Threading.Tasks.Task<global::ReadReply> Read(global::ReadRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

    public virtual global::System.Threading.Tasks.Task<global::WriteReply> Write(global::WriteRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

  }

  /// <summary>Creates service definition that can be registered with a server</summary>
  /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
  public static grpc::ServerServiceDefinition BindService(ServerServiceBase serviceImpl)
  {
    return grpc::ServerServiceDefinition.CreateBuilder()
        .AddMethod(__Method_Read, serviceImpl.Read)
        .AddMethod(__Method_Write, serviceImpl.Write).Build();
  }

  /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
  /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
  /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
  /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
  public static void BindService(grpc::ServiceBinderBase serviceBinder, ServerServiceBase serviceImpl)
  {
    serviceBinder.AddMethod(__Method_Read, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ReadRequest, global::ReadReply>(serviceImpl.Read));
    serviceBinder.AddMethod(__Method_Write, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::WriteRequest, global::WriteReply>(serviceImpl.Write));
  }

}
#endregion
