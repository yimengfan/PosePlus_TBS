using System;
using System.Collections.Generic;
using UnityEngine;
using Pomelo.DotNetClient;
using SimpleJson;
using MyJson;
public class RequestMessage
{
    public string route;
    public JsonNode_Object msg;
    public Action<JsonNode_Object> action;

    public RequestMessage(string route,JsonNode_Object msg,Action<JsonNode_Object> action)
    {
        this.route = route;
        this.msg = msg;
        this.action = action;
    }
}

public class Network
{
    public delegate void OnNetState();
    private static Network _instance; 
    private PomeloClient client;
    private Queue<RequestMessage> requests;
    private NetWorkState netWorkState;
    public static OnNetState OnDisconnect;
    public static OnNetState OnTimeOut;
 //   public static 
    public Network()
    {
        client = new PomeloClient();
        requests = new Queue<RequestMessage>();
        client.NetWorkStateChangedEvent += NetWorkStateChange;
        netWorkState = NetWorkState.CLOSED;
    }

    public static Network Inst
    {
        get
        {
            if(_instance == null)
            {
                _instance = new Network();
            }
            return _instance;
        }
    }

    private void NetWorkStateChange(NetWorkState state)
    {
        netWorkState = state;
         BDebug.Log(state);
        switch (state)
        {
            case NetWorkState.CLOSED:
                {
                     BDebug.Log("关闭:" + host );
                }
                break;
            case NetWorkState.CONNECTING:
                {
                     BDebug.Log("连接:" + host + " --连接中");
                }
                break;
            case NetWorkState.CONNECTED:
                {
                     BDebug.Log("连接:" + host + " --连接上");
                }
                break;
            case NetWorkState.WORK:
                {
                    IEnumeratorTool.ExecAction(() =>
                    {
                        //UIWidgetMgr.Inst.Turn_Chrysanthemum.Hide();
                    });                
                     BDebug.Log("连接:" + host + " --正常工作");
                }
                break;
           case NetWorkState.DISCONNECTED:
                {
                    if(isByClientCall)
                    {
                        isByClientCall = false;
                    }
                    else
                    {
                        if(OnDisconnect != null)
                        {
                            IEnumeratorTool.ExecAction(() =>
                            {
                                OnDisconnect();
                            });
                        }
                    }
                }
                break;
            case NetWorkState.TIMEOUT:
            case NetWorkState.ERROR:
                {
                    if(OnTimeOut!= null)
                    {
                        IEnumeratorTool.ExecAction(() =>
                        {
                            OnTimeOut();
                        });
                    }
                   
                }
                break;
            default:
                break;
        }
    }

    private string host;
    private int port;
    private Action<NetWorkState> action;
    public void Connect(string host,int port,Action<NetWorkState> action = null)
    {
        this.host = host;
        this.port = port;
        this.action = action;
        // BDebug.Log("连接:" + host);
        client.initClient(host, port, () =>
        {
              client.connect((data) =>
              {
                  netWorkState = NetWorkState.WORK;
                  IEnumeratorTool.ExecAction(() =>
                  {
                      //UIWidgetMgr.Inst.Turn_Chrysanthemum.Hide();
                      if (action!=null)
                      {
                         action(netWorkState);
                         action = null;
                      }
                  });
              });
          });
    }
   
    public void ReLogin(Action cb= null)
    {
        var lastAction = action;
        Connect(host, port, (s)=>
        {
            if(cb!=null && s== NetWorkState.WORK)
            {
                cb();
            }
            lastAction(s);
        });
    }

    bool isByClientCall = false;
    public void DisConnect()
    {
        isByClientCall = true;
        client.disconnect();
    }

    public void Request(string route, JsonNode_Object msg, Action<JsonNode_Object> action = null ,bool isNeedShowError = true)
    {
        //requests.Enqueue(new RequestMessage(route, msg, action));
        requests.Enqueue(new RequestMessage(route, msg, (JsonNode_Object json) =>
        {
            if (isNeedShowError)
            {
                IJsonNode error;
                json.TryGetValue("error", out error);
                if (error != null)
                {
                    //UIWidgetMgr.Inst.Widget_SingleSelectWindow.Show(error.AsString());
                }
            }
            //
            action(json);

            //检测绑定
            CheckBind(route, json.ToString());
        }));
    }

    #region 消息绑定

    public delegate void DelegateBindSource(string o);
    private Dictionary<string,DelegateBindSource> bindSourceDictionary = new Dictionary<string, DelegateBindSource>();
    /// <summary>
    /// 这里原则上是绑定 客户端主动请求的消息
    /// 临时接口 后面需要统一
    /// </summary>
    /// <param name="route"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public void BindSource(string route, DelegateBindSource del)
    {
        DelegateBindSource _del;
        if (bindSourceDictionary.ContainsKey(route) == false)
        {
            bindSourceDictionary[route] = (string s) => { Debug.Log("执行回调:" + route); };
        }
        bindSourceDictionary[route] += del;
  
    }

    /// <summary>
    /// 移除绑定
    /// </summary>
    /// <param name="route"></param>
    public void RemoveBindSouce(string route, DelegateBindSource  del)
    {
        DelegateBindSource _del;
        if (bindSourceDictionary.TryGetValue(route, out _del))
        {
            _del -= del;
            bindSourceDictionary[route] = _del;
        }
    }

    /// <summary>
    /// 检测绑定
    /// </summary>
    /// <param name="route"></param>
    /// <param name="data"></param>
    private void CheckBind(string route, string data)
    {
        DelegateBindSource _del;
        if (bindSourceDictionary.TryGetValue(route, out _del))
        {
            _del(data);
        }
    }
    #endregion

   
    /// <summary>
    /// 这里监听服务器主动发送的消息
    /// </summary>
    /// <param name="route"></param>
    /// <param name="action"></param>
    public void Register(string route, Action<JsonNode_Object> action)
    {
        client.on(route, action);
    }

    public void UnRegister(string route, Action<JsonNode_Object> action)
    {
        client.cancel(route, action);
    }

    public void Update()
    {
        if (netWorkState != NetWorkState.WORK)
            return;
        while(requests.Count>0)
        {
            RequestMessage req = requests.Dequeue();
            if(req!=null)
            {
                if(req.action==null)
                {
                    client.notify(req.route, req.msg);
                }
                else
                {
                    client.request(req.route, req.msg, req.action);
                }
            }
        }
    }

    public void Dispose()
    {
        client.Dispose();
    }
}
