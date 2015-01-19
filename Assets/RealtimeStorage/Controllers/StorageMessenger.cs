// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Collections.Generic;
using Realtime.Messaging.Ortc;
using Realtime.Storage.Models;
using Realtime.Tasks;

namespace Realtime.Storage.Controllers
{
    /// <summary>
    /// Wraps the OrtcCLient to allow for multiple message listeners.
    /// </summary>
    public class StorageMessenger
    {
        #region props / fields

        protected OrtcClient Client;
        protected IDictionary<string, OnMessageDelegate> PendingSubscriptions = new Dictionary<string, OnMessageDelegate>();
        protected List<string> SubscribingChannels = new List<string>();
        protected bool IsConnecting;
        protected string AppKey;
        protected string Url;
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new messenger
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="appKey"></param>
        public StorageMessenger(UriPrototype uri, string appKey)
        {
            AppKey = appKey;
            Client = OrtcClientFactory.CreateClient();
            Client.OnConnected += OnConnected;
            Client.OnSubscribed += OnSubscribed;
            Client.OnException += OnException;

            if (uri.IsCluster)
            {
                Url = uri.IsSecure ? "https://" + uri.Url + "/server/ssl/2.1" : "http://" + uri.Url + "/server/2.1";
                Client.ClusterUrl = Url;
            }
            else
            {
                Url = uri.IsSecure ? "https://" + uri.Url : "http://" + uri.Url;
                Client.Url = Url;
            }
        }
        #endregion

        #region private methods
        private void OnException(Exception error)
        {
            // Androids wants this on the main thread
            UnityEngine.Debug.LogError(error.Message);
        }

        private void OnConnected()
        {
            IsConnecting = false;

            // Androids wants this on the main thread
            TaskManager.RunOnMainThread(() =>
            {
                // subscribe pending channels
                foreach (var pendingSubscription in PendingSubscriptions)
                {
                    SubscribingChannels.Add(pendingSubscription.Key);
                    Client.Subscribe(pendingSubscription.Key, pendingSubscription.Value);
                }

                PendingSubscriptions.Clear();
            });
        }

        private void OnSubscribed(string channel)
        {
            SubscribingChannels.Remove(channel);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Is the messenger connected ?
        /// </summary>
        public bool IsConnected
        {
            get { return Client.IsConnected; }
        }

        /// <summary>
        /// Starts the connection
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public Task Connect(string authToken)
        {
            Client.Connect(AppKey, authToken);

            return Task.Run(() =>
            {
                // Wait for connection
                for (int i = 0;i < 100;i++)
                {
                    if (Client.IsConnected)
                        break;

                    //Logger.Log("Waiting for Ortc Connect");
                    Task.Delay(100);
                }

                if (!Client.IsConnected)
                {
                    throw new Exception("Failed to connect");
                }

            });
        }

        /// <summary>
        /// disconnects the messenger
        /// </summary>
        public void Disconnect()
        {
            Client.Disconnect();
        }

        /// <summary>
        /// subscribes to a channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="onMessage"></param>
        public void Subscribe(string channel, OnMessageDelegate onMessage)
        {
            // Androids wants this on the main thread
            TaskManager.RunOnMainThread(() =>
            {
                if (Client.IsConnected && !IsConnecting)
                {
                    if (!Client.IsSubscribed(channel) && !SubscribingChannels.Exists(str => str == channel))
                    {
                        SubscribingChannels.Add(channel);
                        Client.Subscribe(channel, onMessage);
                    }
                }
                else
                {
                    if (!PendingSubscriptions.ContainsKey(channel))
                        PendingSubscriptions.Add(channel, onMessage);
                }
            });
        }

        /// <summary>
        /// Returns true if subscribed to the channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool IsSubscribed(string channel)
        {
            return Client.IsSubscribed(channel);
        }
        #endregion


    }
}