using SimpleJson;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MyJson;
using UnityEngine;
namespace Pomelo.DotNetClient
{
    /// <summary>
    /// network state enum
    /// </summary>
    public enum NetWorkState
    {
        [Description("initial state")]
        CLOSED,

        [Description("connecting server")]
        CONNECTING,

        [Description("server connected")]
        CONNECTED,

        WORK,

        [Description("disconnected with server")]
        DISCONNECTED,

        [Description("connect timeout")]
        TIMEOUT,

        [Description("netwrok error")]
        ERROR
    }

    public class PomeloClient : IDisposable
    {
        /// <summary>
        /// netwrok changed event
        /// </summary>
        public event Action<NetWorkState> NetWorkStateChangedEvent;


        private NetWorkState netWorkState = NetWorkState.CLOSED;   //current network state

        private EventManager eventManager;
        private Socket socket;
        private Protocol protocol;
        private bool disposed = false;
        private uint reqId = 1;

        private ManualResetEvent timeoutEvent = new ManualResetEvent(false);
        private int timeoutMSec = 8000;    //connect timeout count in millisecond

        public PomeloClient()
        {
        }

        /// <summary>
        /// initialize pomelo client
        /// </summary>
        /// <param name="host">server name or server ip (www.xxx.com/127.0.0.1/::1/localhost etc.)</param>
        /// <param name="port">server port</param>
        /// <param name="callback">socket successfully connected callback(in network thread)</param>
        public void initClient(string host, int port, Action callback = null)
        {
            timeoutEvent.Reset();
            eventManager = new EventManager();
            NetWorkChanged(NetWorkState.CONNECTING);

            IPAddress ipAddress;
            if (IPAddress.TryParse(host, out ipAddress) == false)
            {
                 BDebug.Log("ip 解析失败");
            }
            else
            {
               //  BDebug.Log("ip 解析成功");
            }
           //  BDebug.Log("---------------0---------------");
           if(this.socket!= null)
            {
                this.socket.Close();

            }
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.disposed = false;
           //  BDebug.Log("---------------1---------------");
            IPEndPoint ie = new IPEndPoint(ipAddress, port);
           //  BDebug.Log("---------------2---------------");
            socket.BeginConnect(ie, new AsyncCallback((result) =>
            {
                try
                {
                    this.socket.EndConnect(result);
                    this.protocol = new Protocol(this, this.socket);
                    NetWorkChanged(NetWorkState.CONNECTED);

                    if (callback != null)
                    {
                        callback();
                    }
                }
                catch (SocketException e)
                {
                    Debug.LogError(e.Message);
                    if (netWorkState != NetWorkState.TIMEOUT)
                    {
                        NetWorkChanged(NetWorkState.ERROR);
                    }
                    Dispose();
                }
                finally
                {
                    timeoutEvent.Set();
                }
            }), this.socket);

            if (timeoutEvent.WaitOne(timeoutMSec, false))
            {
                if (netWorkState != NetWorkState.CONNECTED && netWorkState != NetWorkState.ERROR)
                {
                    NetWorkChanged(NetWorkState.TIMEOUT);
                    Dispose();
                }
            }

           
        }

        /// <summary>
        /// 网络状态变化
        /// </summary>
        /// <param name="state"></param>
        private void NetWorkChanged(NetWorkState state)
        {
            netWorkState = state;

            if (NetWorkStateChangedEvent != null)
            {
                NetWorkStateChangedEvent(state);
            }
        }

        public void connect()
        {
            connect(null, null);
        }

        public void connect(JsonNode_Object user)
        {
            connect(user, null);
        }

        public void connect(Action<JsonNode_Object> handshakeCallback)
        {
            connect(null, handshakeCallback);
        }

        public bool connect(JsonNode_Object user, Action<JsonNode_Object> handshakeCallback)
        {
            try
            {
                protocol.start(user, handshakeCallback);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private JsonNode_Object emptyMsg = new JsonNode_Object();
        public void request(string route, Action<JsonNode_Object> action)
        {
            this.request(route, emptyMsg, action);
        }

        public void request(string route, JsonNode_Object msg, Action<JsonNode_Object> action)
        {
            this.eventManager.AddCallBack(reqId, action);
            protocol.send(route, reqId, msg);

            reqId++;
        }

        public void notify(string route, JsonNode_Object msg)
        {
            protocol.send(route, msg);
        }

        public void on(string eventName, Action<JsonNode_Object> action)
        {
            eventManager.AddOnEvent(eventName, action);
        }

        public void cancel(string eventName, Action<JsonNode_Object> action)
        {
            eventManager.RemoveOnEvent(eventName, action);
        }

        internal void processMessage(Message msg)
        {
            //try
            //{
            if (msg.type == MessageType.MSG_RESPONSE)
            {
                //msg.data["__route"] = msg.route;
                //msg.data["__type"] = "resp";
                eventManager.InvokeCallBack(msg.id, msg.data);
            }
            else if (msg.type == MessageType.MSG_PUSH)
            {
                //msg.data["__route"] = msg.route;
                //msg.data["__type"] = "push";
                eventManager.InvokeOnEvent(msg.route, msg.data);
            }
            //}
            //catch (Exception e)
            //{
            //    UnityEngine.Debug.LogError(e.Message);
            //}

        }

        public void disconnect()
        {
            timeoutEvent.Reset();
            Dispose();
            NetWorkChanged(NetWorkState.DISCONNECTED);
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                // free managed resources
                if (this.protocol != null)
                {
                    this.protocol.close();
                }

                if (this.eventManager != null)
                {
                    this.eventManager.Dispose();
                }

                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
                    this.socket = null;
                }
                catch (Exception)
                {
                    //todo : 有待确定这里是否会出现异常，这里是参考之前官方github上pull request。emptyMsg
                }

                this.disposed = true;
            }
        }
    }
}