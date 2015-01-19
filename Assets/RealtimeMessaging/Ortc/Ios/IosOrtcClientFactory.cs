// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (UNITY_IOS && !UNITY_PRO_LICENSE)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Realtime.Messaging.Ortc.Ios
{
    internal class MonoPInvokeCallbackAttribute : Attribute
    {
        // ReSharper disable once InconsistentNaming
        public Type type;

        public MonoPInvokeCallbackAttribute(Type t)
        {
            type = t;
        }
    }

    /// <summary>
    /// NDK Interface
    /// </summary>
    public class IosOrtcClientFactory
    {
#region factory
        /// <summary>
        /// Listing of all clients
        /// </summary>
        static internal readonly Dictionary<int, IosOrtcClient> Clients = new Dictionary<int, IosOrtcClient>();

        /// <summary>
        /// creates a new Android Client
        /// </summary>
        /// <returns></returns>
        static public IosOrtcClient CreateClient()
        {
            var id = Create();

            var client = new IosOrtcClient(id);

            Clients.Add(id, client);

            return client;
        }

        /// <summary>
        /// Disposes of the android client
        /// </summary>
        /// <param name="client"></param>
        static public void DestroyClient(IosOrtcClient client)
        {
            Clients.Remove(int.Parse(client.Id));
        }
        
        #endregion

#region private helpers
        static bool HasClient(int id)
        {
           return Clients.ContainsKey(id);
        }
        
        static IosOrtcClient GetClient(int id)
        {
            return Clients[id];
        }
        #endregion

#region ctor

        static IosOrtcClientFactory()
        {
            Init();

            RegisterOnConnectedDelegate(OnConnected);
            RegisterOnDisconnectedDelegate(OnDisconnected);
            RegisterOnReconnectingDelegate(OnReconnecting);
            RegisterOnReconnectedDelegate(OnReconnected);

            RegisterOnSubscribedDelegate(OnSubscribed);
            RegisterOnUnsubscribedDelegate(OnUnsubscribed);
            RegisterOnExceptionDelegate(OnException);
            RegisterOnLogDelegate(OnLog);

            RegisterOnMessageDelegate(OnMessage);
        }
        #endregion

#region delegates
        protected delegate void OnConnectedDelegate(int id);
        protected delegate void OnDisconnectedDelegate(int id);
        protected delegate void OnReconnectingDelegate(int id);
        protected delegate void OnReconnectedDelegate(int id);

        protected delegate void OnSubscribedDelegate(int id, string m);
        protected delegate void OnUnsubscribedDelegate(int id, string m);
        protected delegate void OnExceptionDelegate(int id, string m);
        protected delegate void OnLogDelegate(int id,  string m);

        protected delegate void OnMessageDelegate(int id,string c, string m);
        #endregion

        // factory

#region Commands
        [DllImport("__Internal")]
        protected static extern void Init();

        [DllImport("__Internal")]
        protected static extern int Create();

        [DllImport("__Internal")]
        protected static extern void Destroy(int clientId);

        // props

        [DllImport("__Internal")]
        public static extern void SetUrl(int clientId, string url);

        [DllImport("__Internal")]
		public static extern void SetClusterUrl(int clientId, string url);

        [DllImport("__Internal")]
		public static extern int GetConnectionTimeout(int clientId);

        [DllImport("__Internal")]
		public static extern void SetConnectionTimeout(int clientId, int t);

        [DllImport("__Internal")]
		public static extern void SetConnectionMetadata(int clientId, string meta);

        [DllImport("__Internal")]
		public static extern void SetAnnouncementSubChannel(int clientId, string channel);

        [DllImport("__Internal")]
		public static extern bool GetHeartbeatActive(int clientId);

      //  [DllImport("__Internal")]
	//	public static extern void SetHeartbeatActive(int clientId, bool active);

        [DllImport("__Internal")]
		public static extern int GetHeartbeatTime(int clientId);

        [DllImport("__Internal")]
		public static extern void SetHeartbeatTime(int clientId, int t);

        [DllImport("__Internal")]
		public static extern int GetHeartbeatFails(int clientId);

        [DllImport("__Internal")]
		public static extern string GetSessionId(int clientId);
        
        // methods

        [DllImport("__Internal")]
		public static extern void Connect(int clientId, string appId, string token);

        [DllImport("__Internal")]
		public static extern void Disconnect(int clientId);
        
        [DllImport("__Internal")]
		public static extern void Subscribe(int clientId, string channel);

        [DllImport("__Internal")]
		public static extern void Unsubscribe(int clientId, string channel);

        [DllImport("__Internal")]
		public static extern void SendMessage(int clientId, string channel, string message);
        
        #endregion

#region Subscribe
        // 0
        [DllImport("__Internal")]
        protected static extern void RegisterOnConnectedDelegate(OnConnectedDelegate callback);

        [DllImport("__Internal")]
        protected static extern void RegisterOnDisconnectedDelegate(OnDisconnectedDelegate  callback);

        [DllImport("__Internal")]
        protected static extern void RegisterOnReconnectingDelegate(OnReconnectingDelegate  callback);

        [DllImport("__Internal")]
        protected static extern void RegisterOnReconnectedDelegate(OnReconnectedDelegate callback);

        // 1
        [DllImport("__Internal")]
        protected static extern void RegisterOnSubscribedDelegate(OnSubscribedDelegate callback);

        [DllImport("__Internal")]
        protected static extern void RegisterOnUnsubscribedDelegate(OnUnsubscribedDelegate callback);

        [DllImport("__Internal")]
        protected static extern void RegisterOnExceptionDelegate(OnExceptionDelegate callback);

        [DllImport("__Internal")]
        protected static extern void RegisterOnLogDelegate(OnLogDelegate callback);
        
        // 2
        [DllImport("__Internal")]
        protected static extern void RegisterOnMessageDelegate(OnMessageDelegate callback);

        #endregion

#region handlers

        // 0
        [MonoPInvokeCallback(typeof(OnConnectedDelegate))]
        protected static void OnConnected(int id)
        {
			Debug.Log ("Connected");

            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : "+id);
                return;
            }

            GetClient(id).RaiseOnConnected();
        }

        [MonoPInvokeCallback(typeof(OnDisconnectedDelegate))]
        protected static void OnDisconnected(int id)
        {
            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
                return;
            }

            GetClient(id).RaiseOnDisconnected();
        }

        [MonoPInvokeCallback(typeof(OnReconnectingDelegate))]
        protected static void OnReconnecting(int id)
        {
            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
                return;
            }

            GetClient(id).RaiseOnReconnecting();
        }

        [MonoPInvokeCallback(typeof(OnReconnectedDelegate))]
        protected static void OnReconnected(int id)
        {
            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
                return;
            }

            GetClient(id).RaiseOnReconnected();
        }

        // 1

        [MonoPInvokeCallback(typeof(OnSubscribedDelegate))]
        protected static void OnSubscribed(int id, string arg)
        {
            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
                return;
            }

            GetClient(id).RaiseOnSubscribed(arg);
        }

        [MonoPInvokeCallback(typeof(OnUnsubscribedDelegate))]
        protected static void OnUnsubscribed(int id, string arg)
        {
            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
                return;
            }

            GetClient(id).RaiseOnUnsubscribed(arg);
        }

        [MonoPInvokeCallback(typeof(OnExceptionDelegate))]
        protected static void OnException(int id, string arg)
        {
            Debug.LogError(string.Format("Ortc {0} : {1} ", id, arg));

            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
                return;
            }

            GetClient(id).RaiseOnException(arg);
        }
        
        [MonoPInvokeCallback(typeof(OnLogDelegate))]
        protected static void OnLog(int id, string m)
        {
            if(m.Contains("warning"))
                Debug.LogWarning(string.Format("Ortc {0} : {1} ",id, m));
            else if (m.Contains("error"))
                Debug.LogError(string.Format("Ortc {0} : {1} ",id, m));
            else
                Debug.Log(string.Format("Ortc {0} : {1} ",id, m));
        }

        // 2

        [MonoPInvokeCallback(typeof(OnMessageDelegate))]
        protected static void OnMessage(int id, string c, string m)
        {
            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
                return;
            }

            GetClient(id).RaiseOnMessage(c,m);
        }

        #endregion

    }
}
#endif