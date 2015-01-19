// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using System;
using Realtime.LITJson;
using Realtime.Messaging.WebsocketsSharp;
using Realtime.Tasks;

namespace Realtime.Messaging.Ortc.DotNet
{
    internal class WebSocketConnection
    {
        #region Attributes (1)

        WebSocket _websocket;

        #endregion

        #region Methods - Public (3)

        public void Connect(string url)
        {
            Uri uri;

            var connectionId = Strings.RandomString(8);
            var serverId = Strings.RandomNumber(1, 1000);

            try
            {
                uri = new Uri(url);
            }
            catch (Exception)
            {
                throw new OrtcException(OrtcExceptionReason.InvalidArguments, String.Format("Invalid URL: {0}", url));
            }

            var prefix = "https".Equals(uri.Scheme) ? "wss" : "ws";

            var connectionUrl = new Uri(String.Format("{0}://{1}:{2}/broadcast/{3}/{4}/websocket", prefix, uri.DnsSafeHost, uri.Port, serverId, connectionId));

            //
            // NOTE: For wss connections, must have a valid installed certificate
            // See: http://www.runcode.us/q/c-iphone-push-server
            //

            _websocket = new WebSocket(connectionUrl.AbsoluteUri);

            _websocket.OnOpen += _websocket_OnOpen;
            _websocket.OnError += _websocket_OnError;
            _websocket.OnClose += _websocket_OnClose;
            _websocket.OnMessage += _websocket_OnMessage;


            _websocket.Connect();
        }


        public void Close()
        {
            if (_websocket != null)
            {
                if (_websocket.IsConnected)
                {
                    _websocket.Close();
                }
            }
        }

        public void Send(string message)
        {
            if (_websocket != null)
            {
                Task.RunOnMain(() => _websocket.Send(Serialize(message)));
            }
        }

        #endregion

        #region Methods - Private (1)

        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private string Serialize(object data)
        {
            // NV
            // Note does this ever serialize full Json Objects ?
            // Might be able to remove this dependency
            return JsonMapper.ToJson(data);
        }

        #endregion

        #region Delegates (4)

        public delegate void OnOpenedDelegate();
        public delegate void OnClosedDelegate();
        public delegate void OnErrorDelegate(string error);
        public delegate void OnMessageReceivedDelegate(string message);

        #endregion

        #region Events (4)

        public event OnOpenedDelegate OnOpened;
        public event OnClosedDelegate OnClosed;
        public event OnErrorDelegate OnError;
        public event OnMessageReceivedDelegate OnMessageReceived;

        #endregion

        #region Events Handles (4)

        void _websocket_OnMessage(object sender, MessageEventArgs e)
        {
            var ev = OnMessageReceived;

            if (ev != null)
            {
                Run(() => ev.Invoke(e.Data));
            }
        }

        void _websocket_OnClose(object sender, CloseEventArgs e)
        {
            var ev = OnClosed;

            if (ev != null)
            {
                Run(ev.Invoke);
            }
        }

        void _websocket_OnError(object sender, ErrorEventArgs e)
        {
            var ev = OnError;

            if (ev != null)
            {
                Run(() => ev.Invoke(e.Message));
            }
        }

        void _websocket_OnOpen(object sender, EventArgs e)
        {
            var ev = OnOpened;

            if (ev != null)
            {
                Run(ev.Invoke);
            }
        }

        void Run(Action a)
        {
            a();
        }

        #endregion
    }
}

#endif