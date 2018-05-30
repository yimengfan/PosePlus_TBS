using System;
using SimpleJson;
using MyJson;
namespace Pomelo.Protobuf
{
    public class Protobuf
    {
        private MsgDecoder decoder;
        private MsgEncoder encoder;

        public Protobuf(JsonNode_Object encodeProtos, JsonNode_Object decodeProtos)
        {
            this.encoder = new MsgEncoder(encodeProtos);
            this.decoder = new MsgDecoder(decodeProtos);
        }

        public byte[] encode(string route, JsonNode_Object msg)
        {
            return encoder.encode(route, msg);
        }

        public JsonNode_Object decode(string route, byte[] buffer)
        {
            return decoder.decode(route, buffer);
        }
    }
}