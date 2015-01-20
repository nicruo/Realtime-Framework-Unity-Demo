using System.Collections;
#if UNITY_ANDROID
// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Realtime.Messaging.Ortc;
using Realtime.Tasks;
using UnityEngine;

namespace Realtime.Messaging.Bridge
{
    public class AndroidOrtcClient : OrtcClient
    {

        #region Events (7)

        /// <summary>
        /// Occurs when a connection attempt was successful.
        /// </summary>
        public override event OnConnectedDelegate OnConnected;

        /// <summary>
        /// Occurs when the client connection terminated. 
        /// </summary>
        public override event OnDisconnectedDelegate OnDisconnected;

        /// <summary>
        /// Occurs when the client subscribed to a channel.
        /// </summary>
        public override event OnSubscribedDelegate OnSubscribed;

        /// <summary>
        /// Occurs when the client unsubscribed from a channel.
        /// </summary>
        public override event OnUnsubscribedDelegate OnUnsubscribed;

        /// <summary>
        /// Occurs when there is an error.
        /// </summary>
        public override event OnExceptionDelegate OnException;

        /// <summary>
        /// Occurs when a client attempts to reconnect.
        /// </summary>
        public override event OnReconnectingDelegate OnReconnecting;

        /// <summary>
        /// Occurs when a client reconnected.
        /// </summary>
        public override event OnReconnectedDelegate OnReconnected;


        #endregion

        #region Events handlers (6)

        internal void RaiseOnConnected()
        {
            IsConnected = true;

            TaskManager.RunOnMainThread(() =>
            {
              //  Debug.Log("Ortc.OnConnected");
                
                var ev = OnConnected;

                if (ev == null)
                    return;

                ev();
            });
        }

        internal void RaiseOnDisconnected()
        {
            // this is called on resume messing up reconnect
            if (IsConnected || IsConnecting)
            {
                IsConnected = false;
                IsConnecting = false;

                TaskManager.RunOnMainThread(() =>
                {
                  //  Debug.Log("Ortc.OnDisconnected");

                    var ev = OnDisconnected;

                    if (ev != null)
                    {
                        ev();
                    }
                });
            }
        }

        internal void RaiseOnReconnecting()
        {
            TaskManager.RunOnMainThread(() =>
            {
              //  Debug.Log("Ortc.OnReconnecting");
                IsConnected = false;
                IsConnecting = true;

                var ev = OnReconnecting;

                if (ev != null)
                {
                    ev();
                }
            });
        }

        internal void RaiseOnReconnected()
        {
            TaskManager.RunOnMainThread(() =>
            {
              //  Debug.Log("Ortc.OnReconnected");
                IsConnected = true;
                IsConnecting = false;

                var ev = OnReconnected;

                if (ev != null)
                {
                    ev();
                }
            });
        }

        internal void RaiseOnSubscribed(string channel)
        {
            TaskManager.RunOnMainThread(() =>
            {
               // Debug.Log("Ortc.OnSubscribed "+channel);
                if (_subscribedChannels.ContainsKey(channel))
                {
                    _subscribedChannels[channel].IsSubscribing = false;
                    _subscribedChannels[channel].IsSubscribed = true;

                    var ev = OnSubscribed;

                    if (ev != null)
                    {
                        ev(channel);
                    }
                }

               
            });
        }

        internal void RaiseOnUnsubscribed(string channel)
        {
            TaskManager.RunOnMainThread(() =>
            {
               // Debug.Log("Ortc.OnUnsubscribed " + channel);
                _subscribedChannels.Remove(channel);

                var ev = OnUnsubscribed;

                if (ev != null)
                {

                    ev(channel);
                }
            });
        }

        internal void RaiseOnException(string args)
        {
            RaiseOnException(new Exception(args));
        }

        internal void RaiseOnException(Exception args)
        {
            TaskManager.RunOnMainThread(() =>
            {
                //Debug.LogError("Ortc.OnException " + args.Message);
                var ev = OnException;

                if (ev != null)
                {
                    ev(args);
                }
            });
        }

        internal void RaiseOnMessage(string c, string m)
        {
            TaskManager.RunOnMainThread(() =>
            {
               // Debug.Log("Ortc.OnMessage " + c+" "+m);

                if (_subscribedChannels.ContainsKey(c))
                {
                    var channel = _subscribedChannels[c];

                    if (channel.IsSubscribed && channel.OnMessage != null)
                    {
                        _subscribedChannels[c].OnMessage(c, m);
                    }
                }
            });
        }

        #endregion

        #region fields

        private Dictionary<string, ChannelSubscription> _subscribedChannels = new Dictionary<string, ChannelSubscription>();
        private AndroidJavaClass _javaClass;
        private AndroidJavaObject _javaObject;
        private bool _isConnecting;
        private string _url;
        private string _clusterurl;
        private string _announcmentChannel;
        private string _connectionMetadata;
        #endregion

        #region properties

        public override string Url
        {
            get
            {
                return _url;
            }
            set
            {
                IsCluster = false;
                _url = value;
                _javaObject.Call("setUrl", value);
            }
        }

        public override string ClusterUrl
        {
            get
            {
                return _clusterurl;
            }
            set
            {
                IsCluster = true;
                _clusterurl = value;
                _javaObject.Call("setClusterUrl", value);
            }
        }

        public override int ConnectionTimeout
        {
            get
            {
                return _javaObject.Call<int>("getConnectionTimeout");
            }
            set
            {
                _javaObject.Call("setConnectionTimeout", value);
            }
        }

        public override string ConnectionMetadata
        {
            get
            {
                return _connectionMetadata;
            }
            set
            {
                _connectionMetadata = value;
                _javaObject.Call("setConnectionMetadata", value);
            }
        }

        public override string AnnouncementSubChannel
        {
            get
            {
                return _announcmentChannel;
            }
            set
            {
                _announcmentChannel = value;
                _javaObject.Call("setAnnouncementSubChannel", value);
            }
        }

        public override bool HeartbeatActive
        {
            get
            {
                return _javaObject.Call<bool>("getHeartbeatActive");
            }
            set
            {
                _javaObject.Call("setHeartbeatActive", value);
            }
        }

        public override int HeartbeatTime
        {
            get
            {
                return _javaObject.Call<int>("getHeartbeatTime");
            }
            set
            {
                _javaObject.Call("setHeartbeatTime", value);
            }
        }

        public override int HeartbeatFails
        {
            get { return _javaObject.Call<int>("getHeartbeatFails"); }
            set { _javaObject.Call("setHeartbeatFails", value); }
        }

        public new string Id
        {
            get { return _javaObject.Call<int>("getId").ToString(); }
            set { throw new NotImplementedException();}
        }
        #endregion

        #region ctor

       

        public AndroidOrtcClient()
        {
            if (!TaskManager.IsMainThread)
                throw new Exception("Create Ortc must be called from the main thread.");

            _javaClass = new AndroidJavaClass("ibt.ortc.AndroidBridge");
            _javaObject = _javaClass.CallStatic<AndroidJavaObject>("GetInstance");
        }
        #endregion

        public override void Connect(string appKey, string authToken)
        {
            #region Sanity Checks
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Internet not reachable"));
            //}
            //else 
            if (IsConnected)
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Already connected"));
            }
            else if (_isConnecting)
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Already trying to connect"));
            }
            else if (String.IsNullOrEmpty(ClusterUrl) && String.IsNullOrEmpty(Url))
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "URL and Cluster URL are null or empty"));
            }
            else if (String.IsNullOrEmpty(appKey))
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Application Key is null or empty"));
            }
            else if (String.IsNullOrEmpty(authToken))
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Authentication ToKen is null or empty"));
            }
            else if (!IsCluster && !Url.OrtcIsValidUrl())
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Invalid URL"));
            }
            else if (IsCluster && !ClusterUrl.OrtcIsValidUrl())
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Invalid Cluster URL"));
            }
            else if (AnnouncementSubChannel != null && !AnnouncementSubChannel.OrtcIsValidInput())
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, "Announcement Subchannel has invalid characters"));
            }
            else if (!String.IsNullOrEmpty(ConnectionMetadata) && ConnectionMetadata.Length > MAX_CONNECTION_METADATA_SIZE)
            {
                RaiseOnException(new OrtcException(OrtcExceptionReason.InvalidArguments, String.Format("Connection metadata size exceeds the limit of {0} characters", MAX_CONNECTION_METADATA_SIZE)));
            }
            else

            #endregion

            {
                TaskManager.RunOnMainThread(() =>
                {
                    _javaObject.Call("Connect", appKey, authToken);
                });
            }

        }

        public override void Disconnect()
        {
            _isConnecting = false;

            _subscribedChannels.Clear();

            if (!IsConnected && !IsConnecting)
            {
                RaiseOnException(new OrtcException("Not connected"));
            }
            else
            {
                TaskManager.RunOnMainThread(() =>
                {
                    _javaObject.Call("Disconnect");
                });

                // Make sure this is called same frame for application pause
                RaiseOnDisconnected();
            }
        }

        public override void Subscribe(string channel, OnMessageDelegate onMessage)
        {
            #region Sanity Checks

            bool sanityChecked = true;

            if (!IsConnected)
            {
                RaiseOnException(new OrtcException("Not connected"));
                sanityChecked = false;
            }
            else if (String.IsNullOrEmpty(channel))
            {
                RaiseOnException(new OrtcException("Channel is null or empty"));
                sanityChecked = false;
            }
            else if (!channel.OrtcIsValidInput())
            {
                RaiseOnException(new OrtcException("Channel has invalid characters"));
                sanityChecked = false;
            }
            else if (_subscribedChannels.ContainsKey(channel))
            {
                ChannelSubscription channelSubscription = null;
                _subscribedChannels.TryGetValue(channel, out channelSubscription);

                if (channelSubscription != null)
                {
                    if (channelSubscription.IsSubscribing)
                    {
                        RaiseOnException(new OrtcException(String.Format("Already subscribing to the channel {0}", channel)));
                        sanityChecked = false;
                    }
                    else if (channelSubscription.IsSubscribed)
                    {
                        RaiseOnException(new OrtcException(String.Format("Already subscribed to the channel {0}", channel)));
                        sanityChecked = false;
                    }
                }
            }
            else
            {
                byte[] channelBytes = Encoding.UTF8.GetBytes(channel);

                if (channelBytes.Length > MAX_CHANNEL_SIZE)
                {
                    if (_subscribedChannels.ContainsKey(channel))
                    {
                        ChannelSubscription channelSubscription = null;
                        _subscribedChannels.TryGetValue(channel, out channelSubscription);

                        if (channelSubscription != null)
                        {
                            channelSubscription.IsSubscribing = false;
                        }
                    }

                    RaiseOnException(new OrtcException(String.Format("Channel size exceeds the limit of {0} characters", MAX_CHANNEL_SIZE)));
                    sanityChecked = false;
                }
            }

            #endregion

            if (sanityChecked)
            {
                if (!_subscribedChannels.ContainsKey(channel))
                {
                    _subscribedChannels.Add(channel,
                        new ChannelSubscription
                        {
                            IsSubscribing = true,
                            IsSubscribed = false,
                            SubscribeOnReconnected = true,
                            OnMessage = onMessage
                        });
                }

                if (_subscribedChannels.ContainsKey(channel))
                {
                    ChannelSubscription channelSubscription = null;

                    _subscribedChannels.TryGetValue(channel, out channelSubscription);

                    channelSubscription.IsSubscribing = true;
                    channelSubscription.IsSubscribed = false;
                    channelSubscription.SubscribeOnReconnected = true;
                    channelSubscription.OnMessage = onMessage;
                }

                TaskManager.RunOnMainThread(() =>
                {
                    _javaObject.Call("Subscribe", channel, true);
                });

            }
        }

        public override void Unsubscribe(string channel)
        {
            #region Sanity Checks

            bool sanityChecked = true;

            if (!IsConnected)
            {
                RaiseOnException(new OrtcException("Not connected"));
                sanityChecked = false;
            }
            else if (String.IsNullOrEmpty(channel))
            {
                RaiseOnException(new OrtcException("Channel is null or empty"));
                sanityChecked = false;
            }
            else if (!channel.OrtcIsValidInput())
            {
                RaiseOnException(new OrtcException("Channel has invalid characters"));
                sanityChecked = false;
            }
            else if (!_subscribedChannels.ContainsKey(channel))
            {
                RaiseOnException(new OrtcException(String.Format("Not subscribed to the channel {0}", channel)));
                sanityChecked = false;
            }
            else if (_subscribedChannels.ContainsKey(channel))
            {
                ChannelSubscription channelSubscription = null;
                _subscribedChannels.TryGetValue(channel, out channelSubscription);

                if (channelSubscription != null && !channelSubscription.IsSubscribed)
                {
                    RaiseOnException(new OrtcException(String.Format("Not subscribed to the channel {0}", channel)));
                    sanityChecked = false;
                }
            }
            else
            {
                byte[] channelBytes = Encoding.UTF8.GetBytes(channel);

                if (channelBytes.Length > MAX_CHANNEL_SIZE)
                {
                    RaiseOnException(new OrtcException(String.Format("Channel size exceeds the limit of {0} characters", MAX_CHANNEL_SIZE)));
                    sanityChecked = false;
                }
            }

            #endregion

            if (sanityChecked)
            {
                TaskManager.RunOnMainThread(() =>
                {
                    _javaObject.Call("Unsubscribe", channel);
                });
            }
        }

        public override void Send(string channel, string message)
        {
            #region Sanity Checks

            if (!IsConnected)
            {
                RaiseOnException(new OrtcException("Not connected"));
            }
            else if (String.IsNullOrEmpty(channel))
            {
                RaiseOnException(new OrtcException("Channel is null or empty"));
            }
            else if (!channel.OrtcIsValidInput())
            {
                RaiseOnException(new OrtcException("Channel has invalid characters"));
            }
            else if (String.IsNullOrEmpty(message))
            {
                RaiseOnException(new OrtcException("Message is null or empty"));
            }
            else
            #endregion
            {
                TaskManager.RunOnMainThread(() =>
                {
                    _javaObject.Call("SendMessage", channel, message);
                });
            }

        }

        public override bool IsSubscribed(string channel)
        {
            if (!IsConnected)
            {
                RaiseOnException(new OrtcException("Not connected"));
                return false;
            }
            if (String.IsNullOrEmpty(channel))
            {
                RaiseOnException(new OrtcException("Channel is null or empty"));
                return false;
            }
            if (!channel.OrtcIsValidInput())
            {
                RaiseOnException(new OrtcException("Channel has invalid characters"));
                return false;
            }
            return _subscribedChannels.ContainsKey(channel);

        }



        public override void Dispose()
        {
            AndroidOrtcClientFactory.DestroyClient(this);

            if (IsConnected)
                Disconnect();

            _javaObject.Dispose();
        }
    }
}

#endif