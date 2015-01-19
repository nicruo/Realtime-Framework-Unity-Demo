// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Realtime.Messaging.Ortc;
using Realtime.Tasks;
using UnityEngine;

namespace Realtime.Messaging
{
    #region subclass

    /// <summary>
    /// Channel Permission Instructions
    /// </summary>
    [Serializable]
    public class RealtimePermission
    {
        /// <summary>
        /// Channel Name
        /// </summary>
        [SerializeField]
        public string Channel;
        /// <summary>
        /// Permission
        /// </summary>
        [SerializeField]
        public ChannelPermissions Permission;

        public RealtimePermission()
        {

        }

        public RealtimePermission(string c, ChannelPermissions p)
        {
            Channel = c;
            Permission = p;
        }
    }

    /// <summary>
    /// Occurs when the client connects to the gateway.
    /// </summary>
    public delegate void OnConnectionChangedDelegate(ConnectionState state);

    
    /// <summary>
    /// Occurs when there is an exception.
    /// </summary>
    /// <param name="message"></param>
    public delegate void OnChannelMessageDelegate(string message);

    /// <summary>
    /// Describes the status of a connection
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Not connected
        /// </summary>
        Disconnected,
        /// <summary>
        /// Is Connecting
        /// </summary>
        Connecting,
        /// <summary>
        /// Lost connection
        /// </summary>
        Reconnecting,
        /// <summary>
        /// connected
        /// </summary>
        Connected,
        /// <summary>
        /// Disconnected with saved subscriptions
        /// </summary>
        Paused,
        /// <summary>
        /// Is disconnecting with saved subscriptions
        /// </summary>
        Pausing,
        /// <summary>
        /// Is reconnecting with saved subscriptions
        /// </summary>
        Resuming,
    }

    /// <summary>
    /// describes the current status of a channel subscription
    /// </summary>
    public enum SubscriptionState
    {
        /// <summary>
        /// Not receiving messages
        /// </summary>
        Unsubscribed,
        /// <summary>
        /// Waiting for subscription confirmation
        /// </summary>
        Subscribing,
        /// <summary>
        /// Connection was lost and is reconnecting
        /// </summary>
        Resubscribing,
        /// <summary>
        /// Receiving messages
        /// </summary>
        Subscribed,
        /// <summary>
        /// Subscription will occur on resume
        /// </summary>
        Paused,
    }

    /// <summary>
    /// Url for the realtime network
    /// </summary>
    public struct RealtimeUrl
    {
        /// <summary>
        /// The url to the server
        /// </summary>
        public string Path;

        /// <summary>
        /// Is this server a cluster
        /// </summary>
        public bool IsCluster;

        public RealtimeUrl(string url, bool isCluster)
        {
            Path = url;
            IsCluster = isCluster;
        }

    }

    #endregion

    /// <summary>
    /// A Unity-Friendly messenger api using the IBT.ORTC service
    /// </summary>
    public class RealtimeMessenger
    {
        #region fields
        private readonly OrtcClient _client;

        // cache tasks. Use "Custom" Execution. Pass Exception/Success result back in ORTC handlers
        private Task _connectionTask;
        private Task _unsubscribeTask;
        private Task _disconnectTask;
        private Task _subscribeTask;
        private string _lastSubscribeChannel;
        private string _lastUnsubscribeChannel;
        #endregion

        #region properties

        private ConnectionState _state;
        /// <summary>
        /// Current connection state
        /// </summary>
        public ConnectionState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;
                _state = value;

                if (OnConnectionChanged != null)
                {
                    OnConnectionChanged(value);
                }
            }
        }

        /// <summary>
        ///  State == ConnectionState.Connected
        /// </summary>
        public bool IsConnected
        {
            get { return State == ConnectionState.Connected; }
        }

        /// <summary>
        /// Unique identifier set be the server for this connection
        /// </summary>
        public string SessionId
        {
            get { return _client.SessionId; }
        }

        /// <summary>
        /// Unique identifier for this client
        /// </summary>
        public string Id
        {
            get { return _client.Id; }
        }

        /// <summary> 
        /// UserName or UserId
        /// </summary>
        /// <remarks>
        /// Should be set prior to connection.
        /// </remarks>
        public string ConnectionMetadata
        {
            get { return _client.ConnectionMetadata; }
            set { _client.ConnectionMetadata = value; }
        }

        /// <summary>
        /// Url for the Realtime Service
        /// </summary>
        public RealtimeUrl Url
        {
            get
            {
                if (_client.IsCluster)
                    return new RealtimeUrl
                    {
                        IsCluster = _client.IsCluster,
                        Path = _client.ClusterUrl,
                    };
                return new RealtimeUrl
                {
                    IsCluster = _client.IsCluster,
                    Path = _client.Url,
                };
            }
            set
            {
                if (value.IsCluster)
                    _client.ClusterUrl = value.Path;
                else
                    _client.Url = value.Path;
            }
        }

        /// <summary>
        /// Default Application Key.
        /// Acquired from RealtimeSettings 
        /// </summary>
        public string ApplicationKey { get; set; }

        /// <summary>
        /// Default Private Key. Used for authentication.
        /// Acquired from RealtimeSettings 
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Time for an authentication token to live in seconds.
        /// Expires after inactivity.
        /// </summary>
        public int AuthenticationTime { get; set; }

        /// <summary>
        /// Restricts authentication token's use to a single client
        /// </summary>
        public bool AuthenticationIsPrivate { get; set; }

        /// <summary>
        /// Current Authentication Token
        /// </summary>
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// Indicates that client is disconnected and has cached subscriptions.
        /// Call Resume to reconnect and reapply cached subscriptions.
        /// </summary>
        public bool IsPaused
        {
            get { return State == ConnectionState.Paused; }
        }

        /// <summary>
        /// The ortc client.
        /// </summary>
        public OrtcClient Client
        {
            get { return _client; }
        }

        /// <summary>
        /// Collection of all message handlers : Channel Name by a list of handlers
        /// </summary>
        protected Dictionary<string, List<OnChannelMessageDelegate>> Listeners = new Dictionary<string, List<OnChannelMessageDelegate>>();

        /// <summary>
        /// All subscriptions : Channel Name by The current state of the subscription
        /// </summary>
        protected Dictionary<string, SubscriptionState> SubscriptionStates = new Dictionary<string, SubscriptionState>();

        #endregion

        #region events
        /// <summary>
        /// Raised when the connection status of the client has changed
        /// </summary>
        public event OnConnectionChangedDelegate OnConnectionChanged;

        /// <summary>
        /// Raised when any exception has happened
        /// </summary>
        public event OnExceptionDelegate OnException;

        /// <summary>
        /// Raised when a message is received
        /// </summary>
        public event OnMessageDelegate OnMessage;

        #endregion

        #region ctor
        /// <summary>
        /// Creates a new messenger with the default url
        /// </summary>
        public RealtimeMessenger()
        {
            _client = OrtcClientFactory.CreateClient();

            _client.OnConnected += _client_OnConnected;
            _client.OnDisconnected += _client_OnDisconnected;
            _client.OnReconnected += _client_OnReconnected;
            _client.OnReconnecting += _client_OnReconnecting;
            _client.OnException += _client_OnException;
            _client.OnSubscribed += _client_OnSubscribed;
            _client.OnUnsubscribed += _client_OnUnsubscribed;

            var settings = RealtimeMessengerSettings.Instance;
            if (settings != null)
            {
                Url = new RealtimeUrl(settings.Url, settings.IsCluster);
                ApplicationKey = settings.ApplicationKey;
                PrivateKey = settings.PrivateKey;
                AuthenticationTime = settings.AuthenticationTime;
                AuthenticationIsPrivate = settings.AuthenticationIsPrivate;
            }
            else
                Debug.LogWarning("Realtime settings is null. You will need to configure the messenger manually.");
        }
        #endregion

        #region Messenger Methods
        /// <summary>
        /// Begins a Connection Task.
        /// Be sure to set your AuthenticationToken and Metadata First !
        /// </summary>
        /// <returns>A task for the duration of the process</returns>
        public Task Connect()
        {
            if (State != ConnectionState.Disconnected && State != ConnectionState.Paused)
            {
                return new Task(new Exception("Already connected or is connecting"));
            }

            if (State == ConnectionState.Paused)
            {
                State = ConnectionState.Disconnected;
                SubscriptionStates.Clear();
            }

            _connectionTask = new Task(TaskStrategy.Custom);

            State = ConnectionState.Connecting;
            _client.Connect(ApplicationKey, AuthenticationToken);

            return _connectionTask;
        }

        /// <summary>
        /// Begins Disconnection
        /// </summary>
        /// <returns>A task for the duration of the process</returns>
        public Task Disconnect()
        {
            if (State == ConnectionState.Disconnected)
            {
                return new Task(new Exception("The client is already disconnected"));
            }

            _disconnectTask = new Task(TaskStrategy.Custom);

            _client.Disconnect();

            return _disconnectTask;
        }

        /// <summary>
        /// Begins a Resume Task.
        /// Will reconnect and reapply paused subscriptions
        /// </summary>
        /// <returns>A task for the duration of the process</returns>
        public Task Resume()
        {
            if (State != ConnectionState.Disconnected && State != ConnectionState.Paused)
            {
                return new Task(new Exception("Already connected or is connecting"));
            }

            State = ConnectionState.Resuming;

            _connectionTask = new Task(TaskStrategy.Custom);

            _client.Connect(ApplicationKey, AuthenticationToken);

            return _connectionTask;
        }

        /// <summary>
        /// Disconnects but caches subscriptions. Call Resume to resubscribe from cache.
        /// </summary>
        /// <returns>A task for the duration of the process</returns>
        public Task Pause()
        {
            if (State == ConnectionState.Disconnected)
            {
                return new Task(new Exception("The client is already disconnected"));
            }

            State = ConnectionState.Pausing;

            _disconnectTask = new Task(TaskStrategy.Custom);


            _client.Disconnect();

            return _disconnectTask;
        }



        /// <summary>
        /// Begins subscription to the ORTC channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>A task for the duration of the process</returns>
        public Task Subscribe(string channel)
        {
            if (State != ConnectionState.Connected)
            {
                return new Task(new Exception("Not connected"));
            }
            var s = GetSubscriptionState(channel);
            if (s != SubscriptionState.Unsubscribed && s != SubscriptionState.Paused)
            {
                return new Task(new Exception("Already subscribed or is Subscribing"));
            }

            _subscribeTask = new Task(TaskStrategy.Custom);
            _lastSubscribeChannel = channel;

            _client.Subscribe(channel, _client_OnMessage);

            return _subscribeTask;
        }

        /// <summary>
        /// Begins unsubscription to the ORTC channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>A task for the duration of the process</returns>
        public Task Unsubscribe(string channel)
        {
            if (State != ConnectionState.Connected)
            {
                return new Task(new Exception("Not connected"));
            }

            if (GetSubscriptionState(channel) != SubscriptionState.Subscribed)
            {
                return new Task(new Exception("Not subscribed"));
            }

            _unsubscribeTask = new Task(TaskStrategy.Custom);
            _lastUnsubscribeChannel = channel;

            _client.Unsubscribe(channel);

            return _unsubscribeTask;
        }

        /// <summary>
        /// Sends a message to the specific channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns>A task for the duration of the process</returns>
        public Task Send(string channel, string message)
        {
            if (State != ConnectionState.Connected)
            {
                return new Task(new Exception("Not connected"));
            }

            //if (GetSubscriptionState(channel) != SubscriptionState.Subscribed)
            //{
            //    return new Task(new Exception("Not subscribed"));
            //}

            _client.Send(channel, message);

            return new Task(TaskStrategy.Custom)
            {
                Status = TaskStatus.Success
            };
        }

        /// <summary>
        /// Returns the subscription state of the channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>The current subscription state for the channel</returns>
        public SubscriptionState GetSubscriptionState(string channel)
        {
            if (!SubscriptionStates.ContainsKey(channel))
                return SubscriptionState.Unsubscribed;
            return SubscriptionStates[channel];
        }

        /// <summary>
        /// Adds a handler for a specific channel
        /// </summary>
        /// <param name="channel">the channel to listen to</param>
        /// <param name="action">the message received action handler </param>
        public void AddListener(string channel, OnChannelMessageDelegate action)
        {
            if (!Listeners.ContainsKey(channel))
                Listeners.Add(channel, new List<OnChannelMessageDelegate>());

            Listeners[channel].Add(action);
        }

        /// <summary>
        /// Removes a message handler for a specific channel
        /// </summary>
        /// <param name="channel">the channel to listen to</param>
        /// <param name="action">the message received action handler </param>
        public void RemoveListener(string channel, OnChannelMessageDelegate action)
        {
            if (!Listeners.ContainsKey(channel))
                return;

            Listeners[channel].Remove(action);
        }

        /// <summary>
        /// Wait coroutine for waiting for the connection state to == Connected
        /// </summary>
        /// <returns>A coroutine method</returns>
        public IEnumerator WaitForConnected()
        {
            while (State != ConnectionState.Connected)
            {
                yield return 1;
            }
        }
        #endregion

        #region authentication

        /// <summary>
        /// Posts an authentication token to the network.
        /// This token may then be used by a connecting client to gain access 
        /// </summary>
        /// <remarks>
        /// - Authentication may be disabled (if you want)
        /// - It is suggested you do not authenticate from the client, but, a webserver
        /// </remarks>
        /// <returns>A task with true if authenticated</returns>
        public Task<bool> PostAuthentication(IEnumerable<RealtimePermission> permissions)
        {
            var p = new Dictionary<string, List<ChannelPermissions>>();

            foreach (var k in permissions)
            {
                if (!p.ContainsKey(k.Channel))
                    p.Add(k.Channel, new List<ChannelPermissions>());

                p[k.Channel].Add(k.Permission);
            }

            return AuthenticationClient.PostAuthenticationAsync(Url.Path, Url.IsCluster, AuthenticationToken, AuthenticationIsPrivate, ApplicationKey, AuthenticationTime, PrivateKey, p);
        }

        #endregion

        #region presence

        /// <summary>
        /// Returns the metadata for a channel
        /// </summary>
        /// <param name="authenticationToken">Current Authentication token</param>
        /// <param name="channel"></param>
        /// <returns>A task with the current presence state</returns>
        public Task<Presence> GetPresence(string authenticationToken, string channel)
        {
            return PresenceClient.GetPresenceAsync(Url.Path, Url.IsCluster, ApplicationKey, authenticationToken, channel);
        }

        /// <summary>
        /// Enables Presence
        /// </summary>
        /// <param name="channel">channel to enable</param>
        /// <param name="metadata">If should collect the first 100 unique metadata</param>
        /// <returns>A task with the current presence state</returns>
        public Task<string> EnablePresence(string channel, bool metadata)
        {
            return PresenceClient.EnablePresenceAsync(Url.Path, Url.IsCluster, ApplicationKey, PrivateKey, channel, metadata);
        }

        /// <summary>
        /// Disables Presence
        /// </summary>
        /// <param name="channel">channel to disable</param>
        /// <returns>A task with the current presence state</returns>
        public Task<string> DisabledPresence(string channel)
        {
            return PresenceClient.DisablePresenceAsync(Url.Path, Url.IsCluster, ApplicationKey, PrivateKey, channel);
        }
        #endregion

        #region internal Methods

        void _client_OnUnsubscribed(string channel)
        {
            if (!SubscriptionStates.ContainsKey(channel))
                SubscriptionStates.Add(channel, SubscriptionState.Unsubscribed);
            else
                SubscriptionStates[channel] = SubscriptionState.Unsubscribed;

            _unsubscribeTask.Status = TaskStatus.Success;
            _unsubscribeTask = null;
        }

        void _client_OnSubscribed(string channel)
        {
            if (!SubscriptionStates.ContainsKey(channel))
                SubscriptionStates.Add(channel, SubscriptionState.Subscribed);
            else
                SubscriptionStates[channel] = SubscriptionState.Subscribed;

            _subscribeTask.Status = TaskStatus.Success;
            _subscribeTask = null;
        }

        void _client_OnException(Exception ex)
        {
            if (OnException != null)
            {
                OnException(ex);
            }

            if (_connectionTask != null)
            {
                SubscriptionStates.Clear();
                State = ConnectionState.Disconnected;
                // is connecting, fault it
                _connectionTask.Exception = ex;
                _connectionTask.Status = TaskStatus.Faulted;
            }

            if (_subscribeTask != null)
            {
                if (SubscriptionStates.ContainsKey(_lastSubscribeChannel))
                    SubscriptionStates.Add(_lastSubscribeChannel, SubscriptionState.Unsubscribed);

                // fault it
                _subscribeTask.Exception = ex;
                _subscribeTask.Status = TaskStatus.Faulted;
            }
            if (_unsubscribeTask != null)
            {
                if (SubscriptionStates.ContainsKey(_lastUnsubscribeChannel))
                    SubscriptionStates.Add(_lastUnsubscribeChannel, SubscriptionState.Unsubscribed);

                //fault it
                _unsubscribeTask.Exception = ex;
                _unsubscribeTask.Status = TaskStatus.Faulted;
            }
            if (_disconnectTask != null)
            {
                //fault it
                _disconnectTask.Exception = ex;
                _disconnectTask.Status = TaskStatus.Faulted;
            }
        }

        void _client_OnReconnecting()
        {
            State = ConnectionState.Reconnecting;
        }

        void _client_OnReconnected()
        {
            State = ConnectionState.Connected;
        }

        void _client_OnDisconnected()
        {
            if (State == ConnectionState.Pausing)
                State = ConnectionState.Paused;
            else
                State = ConnectionState.Disconnected;

            if (State == ConnectionState.Paused)
            {
                foreach (var state in SubscriptionStates.Keys.ToArray())
                {
                    SubscriptionStates[state] = SubscriptionState.Paused;
                }
            }
            else
            {
                SubscriptionStates.Clear();
            }

            if (_disconnectTask != null)
                _disconnectTask.Status = TaskStatus.Success;
        }

        void _client_OnConnected()
        {
            State = ConnectionState.Connected;

            if (_connectionTask != null)
            {
                _connectionTask.Status = TaskStatus.Success;
                _connectionTask = null;
            }

            // Re apply
            var subs = SubscriptionStates.Where(o => o.Value == SubscriptionState.Paused).ToArray();
            foreach (var s in subs)
            {
                Subscribe(s.Key);
            }
        }

        void _client_OnMessage(string channel, string message)
        {
            if (OnMessage != null)
            {
                OnMessage(channel, message);
            }

            if (Listeners.ContainsKey(channel))
            {
                for (int i = 0;i < Listeners[channel].Count;i++)
                {
                    Listeners[channel][i](message);
                }
            }
        }

        #endregion

    }
}