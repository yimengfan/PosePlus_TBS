using System;
using SimpleJson;
using MyJson;
namespace Pomelo.DotNetClient
{
    public class Message
    {
        public MessageType type;
        public string route;
        public uint id;
        public JsonNode_Object data;

        public Message(MessageType type, uint id, string route, JsonNode_Object data)
        {
            this.type = type;
            this.id = id;
            this.route = route;
            this.data = data;
        }
    }
}