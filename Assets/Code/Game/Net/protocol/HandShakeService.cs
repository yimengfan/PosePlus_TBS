using System;
using System.Text;
using SimpleJson;
using System.Net;
using System.Net.Sockets;
using MyJson;
namespace Pomelo.DotNetClient
{
    public class HandShakeService
    {
        private Protocol protocol;
        private Action<JsonNode_Object> callback;

        public const string Version = "0.3.0";
        public const string Type = "unity-socket";


        public HandShakeService(Protocol protocol)
        {
            this.protocol = protocol;
        }

        public void request(JsonNode_Object user, Action<JsonNode_Object> callback)
        {
            byte[] body = Encoding.UTF8.GetBytes(buildMsg(user).ToString());

            protocol.send(PackageType.PKG_HANDSHAKE, body);

            this.callback = callback;
        }

        internal void invokeCallback(JsonNode_Object data)
        {
            //Invoke the handshake callback
            if (callback != null) callback.Invoke(data);
        }

        public void ack()
        {
            protocol.send(PackageType.PKG_HANDSHAKE_ACK, new byte[0]);
        }

        private JsonNode_Object buildMsg(JsonNode_Object user)
        {
            if (user == null) user = new JsonNode_Object();

            JsonNode_Object msg = new JsonNode_Object();

            //Build sys option
            JsonNode_Object sys = new JsonNode_Object();
            sys["version"] = new JsonNode_ValueString(Version);
            sys["type"] = new JsonNode_ValueString(Type);

            //Build handshake message
            msg["sys"] = sys;
            msg["user"] = user;

            return msg;
        }
    }
}