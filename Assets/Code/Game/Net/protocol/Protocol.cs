using System;
using SimpleJson;
using System.Text;
using MyJson;
namespace Pomelo.DotNetClient
{
    public class Protocol
    {
        private MessageProtocol messageProtocol;
        private ProtocolState state;
        private Transporter transporter;
        private HandShakeService handshake;
        private HeartBeatService heartBeatService = null;
        private PomeloClient pc;

        public PomeloClient getPomeloClient()
        {
            return this.pc;
        }

        public Protocol(PomeloClient pc, System.Net.Sockets.Socket socket)
        {
            this.pc = pc;
            this.transporter = new Transporter(socket, this.processMessage);
            this.transporter.onDisconnect = onDisconnect;

            this.handshake = new HandShakeService(this);
            this.state = ProtocolState.start;
        }

        internal void start(JsonNode_Object user, Action<JsonNode_Object> callback)
        {
            this.transporter.start();
            this.handshake.request(user, callback);

            this.state = ProtocolState.handshaking;
        }

        //Send notify, do not need id
        internal void send(string route, JsonNode_Object msg)
        {
            send(route, 0, msg);
        }

        //Send request, user request id 
        internal void send(string route, uint id, JsonNode_Object msg)
        {
            if (this.state != ProtocolState.working) return;

            byte[] body = messageProtocol.encode(route, id, msg);

            send(PackageType.PKG_DATA, body);
        }

        internal void send(PackageType type)
        {
            if (this.state == ProtocolState.closed) return;
            transporter.send(PackageProtocol.encode(type));
        }

        //Send system message, these message do not use messageProtocol
        internal void send(PackageType type, JsonNode_Object msg)
        {
            //This method only used to send system package
            if (type == PackageType.PKG_DATA) return;

            byte[] body = Encoding.UTF8.GetBytes(msg.ToString());

            send(type, body);
        }

        //Send message use the transporter
        internal void send(PackageType type, byte[] body)
        {
            if (this.state == ProtocolState.closed) return;

            byte[] pkg = PackageProtocol.encode(type, body);

            transporter.send(pkg);
        }

        //Invoke by Transporter, process the message
        internal void processMessage(byte[] bytes)
        {
            Package pkg = PackageProtocol.decode(bytes);

            //Ignore all the message except handshading at handshake stage
            if (pkg.type == PackageType.PKG_HANDSHAKE && this.state == ProtocolState.handshaking)
            {
                //Ignore all the message except handshading
                JsonNode_Object data = (JsonNode_Object)MyJson.MyJson.Parse(Encoding.UTF8.GetString(pkg.body));

                processHandshakeData(data);

                this.state = ProtocolState.working;

            }
            else if (pkg.type == PackageType.PKG_HEARTBEAT && this.state == ProtocolState.working)
            {
                this.heartBeatService.resetTimeout();
            }
            else if (pkg.type == PackageType.PKG_DATA && this.state == ProtocolState.working)
            {
                this.heartBeatService.resetTimeout();
                pc.processMessage(messageProtocol.decode(pkg.body));
            }
            else if (pkg.type == PackageType.PKG_KICK)
            {
                this.getPomeloClient().disconnect();
                this.close();
            }
        }

        private void processHandshakeData(JsonNode_Object msg)
        {
            //Handshake error
            if (!msg.ContainsKey("code") || !msg.ContainsKey("sys") || msg["code"].AsInt() != 200)
            {
                throw new Exception("Handshake error! Please check your handshake config.");
            }

            //Set compress data
            JsonNode_Object sys = (JsonNode_Object)msg["sys"];

            JsonNode_Object dict = new JsonNode_Object();
            if (sys.ContainsKey("dict")) dict = (JsonNode_Object)sys["dict"];

            JsonNode_Object protos = new JsonNode_Object();
            JsonNode_Object serverProtos = new JsonNode_Object();
            JsonNode_Object clientProtos = new JsonNode_Object();

            if (sys.ContainsKey("protos"))
            {
                protos = (JsonNode_Object)sys["protos"];
                serverProtos = (JsonNode_Object)protos["server"];
                clientProtos = (JsonNode_Object)protos["client"];
            }

            messageProtocol = new MessageProtocol(dict, serverProtos, clientProtos);

            //Init heartbeat service
            int interval = 0;
            if (sys.ContainsKey("heartbeat")) interval = Convert.ToInt32(sys["heartbeat"].AsInt());
            heartBeatService = new HeartBeatService(interval, this);

            if (interval > 0)
            {
                heartBeatService.start();
            }

            //send ack and change protocol state
            handshake.ack();
            this.state = ProtocolState.working;

            //Invoke handshake callback
            JsonNode_Object user = new JsonNode_Object();
            if (msg.ContainsKey("user")) user = (JsonNode_Object)msg["user"];
            handshake.invokeCallback(user);
        }

        //The socket disconnect
        private void onDisconnect()
        {
            this.pc.disconnect();
        }

        internal void close()
        {
            transporter.close();

            if (heartBeatService != null) heartBeatService.stop();

            this.state = ProtocolState.closed;
        }
    }
}