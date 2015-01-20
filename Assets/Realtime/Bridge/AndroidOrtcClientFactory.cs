#if UNITY_ANDROID
// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Realtime.Messaging.Bridge
{
    /// <summary>
    /// NDK Interface
    /// </summary>
    public class AndroidOrtcClientFactory
    {
#region factory
        /// <summary>
        /// Listing of all clients
        /// </summary>
        static internal readonly Dictionary<int, AndroidOrtcClient> Clients = new Dictionary<int, AndroidOrtcClient>();

        /// <summary>
        /// creates a new Android Client
        /// </summary>
        /// <returns></returns>
        static public AndroidOrtcClient CreateClient()
        {
            var client = new AndroidOrtcClient();

            Clients.Add(int.Parse(client.Id), client);

            return client;
        }

        /// <summary>
        /// Disposes of the android client
        /// </summary>
        /// <param name="client"></param>
        static public void DestroyClient(AndroidOrtcClient client)
        {
            Clients.Remove(int.Parse(client.Id));
        }
#endregion

#region private helpers
        static bool HasClient(int id)
        {
            return Clients.ContainsKey(id);
        }

        static AndroidOrtcClient GetClient(int id)
        {
            return Clients[id];
        }
#endregion

#region ctor

        static AndroidOrtcClientFactory()
        {
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
        protected delegate void OnLogDelegate(int id, string m);

        protected delegate void OnMessageDelegate(int id, string c, string m);
#endregion

#region Callback Registration

        // 0
        [DllImport("android_bridge")]
        protected static extern void RegisterOnConnectedDelegate(OnConnectedDelegate callback);

        [DllImport("android_bridge")]
        protected static extern void RegisterOnDisconnectedDelegate(OnDisconnectedDelegate callback);

        [DllImport("android_bridge")]
        protected static extern void RegisterOnReconnectingDelegate(OnReconnectingDelegate callback);

        [DllImport("android_bridge")]
        protected static extern void RegisterOnReconnectedDelegate(OnReconnectedDelegate callback);

        // 1
        [DllImport("android_bridge")]
        protected static extern void RegisterOnSubscribedDelegate(OnSubscribedDelegate callback);

        [DllImport("android_bridge")]
        protected static extern void RegisterOnUnsubscribedDelegate(OnUnsubscribedDelegate callback);

        [DllImport("android_bridge")]
        protected static extern void RegisterOnExceptionDelegate(OnExceptionDelegate callback);

        [DllImport("android_bridge")]
        protected static extern void RegisterOnLogDelegate(OnLogDelegate callback);

        // 2
        [DllImport("android_bridge")]
        protected static extern void RegisterOnMessageDelegate(OnMessageDelegate callback);


#endregion

#region handlers

        // 0
        [MonoPInvokeCallback(typeof(OnConnectedDelegate))]
        protected static void OnConnected(int id)
        {
            Debug.Log("Connected " + id);
            if (!HasClient(id))
            {
                Debug.LogError("Ortc Client not found : " + id);
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
            if (m.ToLower().Contains("warning"))
                Debug.LogWarning(string.Format("Ortc {0} : {1} ", id, m));
            else if (m.ToLower().Contains("error"))
                Debug.LogError(string.Format("Ortc {0} : {1} ", id, m));
            else
                Debug.Log(string.Format("Ortc {0} : {1} ", id, m));
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

            GetClient(id).RaiseOnMessage(c, m);
        }

#endregion
    }
}
#endif