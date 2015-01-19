// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (UNITY_IOS && !UNITY_PRO_LICENSE)

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Realtime.Messaging.Ortc.Ios
{
    public class IosOrtcClient : OrtcClient
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
            SessionId = IosOrtcClientFactory.GetSessionId(_id);

            IsConnected = true;

            var ev = OnConnected;

            if (ev == null)
                return;

            ev();
        }

        internal void RaiseOnDisconnected()
        {
            IsConnected = false;
            IsConnecting = false;

            var ev = OnDisconnected;

            if (ev != null)
            {
                ev();
            }
        }

        internal void RaiseOnReconnecting()
        {
            IsConnected = false;
            IsConnecting = true;

            var ev = OnReconnecting;

            if (ev != null)
            {
                ev();
            }
        }

        internal void RaiseOnReconnected()
        {

            IsConnected = true;
            IsConnecting = false;
            
            var ev = OnReconnected;

            if (ev != null)
            {
                ev();
            }
        }

        internal void RaiseOnSubscribed(string channel)
        {
            if (_subscribedChannels.ContainsKey(channel))
            {
                _subscribedChannels[channel].IsSubscribing = false;
                _subscribedChannels[channel].IsSubscribed = true;
            }

            var ev = OnSubscribed;

            if (ev != null)
            {
                ev(channel);
            }
        }

        internal void RaiseOnUnsubscribed(string channel)
        {
            _subscribedChannels.Remove(channel);

            var ev = OnUnsubscribed;

            if (ev != null)
            {

                ev(channel);
            }
        }

        internal void RaiseOnException(string args)
        {
            RaiseOnException(new Exception(args));
        }

        internal void RaiseOnException(Exception args)
        {
            Debug.LogException(args);

            var ev = OnException;

            if (ev != null)
            {
                ev(args);
            }
        }

        internal void RaiseOnMessage(string c, string m)
        {
            if (_subscribedChannels.ContainsKey(c))
            {
                var channel = _subscribedChannels[c];

                if (channel.IsSubscribed && channel.OnMessage != null)
                {
                    _subscribedChannels[c].OnMessage(c, m);
                }
            }
        }

#endregion

#region fields

        private Dictionary<string, ChannelSubscription> _subscribedChannels = new Dictionary<string, ChannelSubscription>();
        private bool _isConnecting;
        private string _url;
        private string _clusterurl;
        private string _announcmentChannel;
        private string _connectionMetadata;
        private int _id;
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
                IosOrtcClientFactory.SetUrl(_id, value);
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
                IosOrtcClientFactory.SetClusterUrl(_id, value);
            }
        }

        public override int ConnectionTimeout
        {
            get
            {
                return IosOrtcClientFactory.GetConnectionTimeout(_id);
            }
            set
            {
                IosOrtcClientFactory.SetConnectionTimeout(_id, value);
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
                IosOrtcClientFactory.SetConnectionMetadata(_id, value);
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
                IosOrtcClientFactory.SetAnnouncementSubChannel(_id, value);
            }
        }

        public override bool HeartbeatActive
        {
            get
            {
				return false;
              //  return IosOrtcClientFactory.GetHeartbeatActive(_id);;
            }
            set
            {
               // IosOrtcClientFactory.SetHeartbeatActive(_id, value);
            }
        }

        public override int HeartbeatTime
        {
            get
            {
                return IosOrtcClientFactory.GetHeartbeatTime(_id);
            }
            set
            {
                IosOrtcClientFactory.SetHeartbeatTime(_id, value);
            }
        }

        public override int HeartbeatFails
        {
            get
            {
                return IosOrtcClientFactory.GetHeartbeatFails(_id);
            }
            set
            {
                // Hearbeat not complete on IOS
            }
        }
        
		public string GetSession(){
		
			return IosOrtcClientFactory.GetSessionId (_id);
		}

#endregion

#region ctor

        public IosOrtcClient(int id)
        {
            _id = id;
            Id = id.ToString();
        }
#endregion

        public override void Connect(string appKey, string authToken)
        {
#region Sanity Checks

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
                IosOrtcClientFactory.Connect(_id, appKey, authToken);
            }

        }

        public override void Disconnect()
        {
            _isConnecting = false;
            _subscribedChannels.Clear();

            if (!IsConnected)
            {
                RaiseOnException(new OrtcException("Not connected"));
            }
            else
            {
                IosOrtcClientFactory.Disconnect(_id);
            }
        }

        public override void Subscribe(string channel,  OnMessageDelegate onMessage)
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

				Debug.Log("Sending to XCode");

                IosOrtcClientFactory.Subscribe(_id,channel);

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
                IosOrtcClientFactory.Unsubscribe(_id, channel);
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
                IosOrtcClientFactory.SendMessage(_id, channel, message);
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
    }
}
#endif