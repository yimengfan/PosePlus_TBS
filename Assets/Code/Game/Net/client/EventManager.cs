using System;
using System.Collections.Generic;
using System.Text;
using SimpleJson;
using MyJson;
namespace Pomelo.DotNetClient
{
    public class EventManager : IDisposable
    {
        private Dictionary<uint, Action<JsonNode_Object>> callBackMap;
        private Dictionary<string, List<Action<JsonNode_Object>>> eventMap;

        public EventManager()
        {
            this.callBackMap = new Dictionary<uint, Action<JsonNode_Object>>();
            this.eventMap = new Dictionary<string, List<Action<JsonNode_Object>>>();
        }

        //Adds callback to callBackMap by id.
        public void AddCallBack(uint id, Action<JsonNode_Object> callback)
        {
            if (id > 0 && callback != null)
            {
                this.callBackMap.Add(id, callback);
            }
        }

        /// <summary>
        /// Invoke the callback when the server return messge .
        /// </summary>
        /// <param name='pomeloMessage'>
        /// Pomelo message.
        /// </param>
        public void InvokeCallBack(uint id, JsonNode_Object data)
        {
            if (!callBackMap.ContainsKey(id)) return;
            IEnumeratorTool.ExecAction(() =>
            {
                callBackMap[id](data);
            });
        }

        //Adds the event to eventMap by name.
        public void AddOnEvent(string eventName, Action<JsonNode_Object> callback)
        {
            List<Action<JsonNode_Object>> list = null;
            if (this.eventMap.TryGetValue(eventName, out list))
            {
                list.Add(callback);
            }
            else
            {
                list = new List<Action<JsonNode_Object>>();
                list.Add(callback);
                this.eventMap.Add(eventName, list);
            }
        }

        public void RemoveOnEvent(string eventName, Action<JsonNode_Object> callback)
        {
            List<Action<JsonNode_Object>> list = null;
            if (this.eventMap.TryGetValue(eventName, out list))
            {
                list.Remove(callback);
            }
        }

        /// <summary>
        /// If the event exists,invoke the event when server return messge.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///
        public void InvokeOnEvent(string route, JsonNode_Object msg)
        {
            if (!this.eventMap.ContainsKey(route)) return;

            List<Action<JsonNode_Object>> list = eventMap[route];
            foreach (Action<JsonNode_Object> action in list)
            {
                IEnumeratorTool.ExecAction(() =>
                {
                    action.Invoke(msg);
                });
              
            }
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected void Dispose(bool disposing)
        {
            this.callBackMap.Clear();
            this.eventMap.Clear();
        }
    }
}