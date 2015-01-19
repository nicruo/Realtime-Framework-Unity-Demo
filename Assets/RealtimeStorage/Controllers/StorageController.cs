// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using Realtime.Storage.DataAccess;
using Realtime.Storage.Models;
using Realtime.Tasks;

namespace Realtime.Storage.Controllers
{
    /// <summary>
    /// Adds Update notification to Item and Storage Ref's using the ORTC messenger 
    /// </summary>
    public class StorageController
    {
        #region props / fields
        /// <summary>
        /// Application Key
        /// </summary>
        public string ApplicationKey { get; private set; }
        /// <summary>
        /// Authentication token for the user
        /// </summary>
        public string AuthenticationToken { get; set; }
        /// <summary>
        /// Private key for the application
        /// </summary>
        public string PrivateKey { get; private set; }

        protected UriPrototype MessengerUrl;

        /// <summary>
        /// Metadata Instance
        /// </summary>
        public StorageMetadata Metadata { get; private set; }
        /// <summary>
        /// Repository Instance
        /// </summary>
        public StorageRepository Repository { get; private set; }
        /// <summary>
        /// Messenger Instance
        /// </summary>
        public StorageMessenger Messenger { get; private set; }
        /// <summary>
        /// Current Realtime Storage Settings
        /// </summary>
        protected RealtimeStorageSettings Settings
        {
            get { return RealtimeStorageSettings.Instance; }
        }

        readonly IDictionary<string, List<Action<string>>> _listeners = new Dictionary<string, List<Action<string>>>();
        #endregion

        #region ctor


        /// <summary>
        /// Creates a new storage controller with custom settings
        /// </summary>
        /// <param name="applicationKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="storageurl"></param>
        /// <param name="messengerUrl"></param>
        public StorageController(string applicationKey, string privateKey, UriPrototype storageurl, UriPrototype messengerUrl)
        {
            Init(applicationKey, privateKey, storageurl, messengerUrl);
        }

        /// <summary>
        /// Creates a new storage controller using default settings with a connection
        /// </summary>
        public StorageController(string authToken)
        {
            AuthenticationToken = authToken;

            Init(Settings.ApplicationKey, Settings.PrivateKey,
                new UriPrototype
                {
                    Url = Settings.StorageUrl,
                    IsSecure = Settings.StorageSSL,
                    IsCluster = Settings.StorageIsCluster,
                },
                 new UriPrototype
                 {
                     Url = Settings.MessengerUrl,
                     IsSecure = Settings.MessengerSSL,
                     IsCluster = Settings.MessengerIsCluster,
                 }
                );
        }

        /// <summary>
        /// Creates a new storage controller using default settings without a connection
        /// </summary>
        public StorageController()
        {
            Init(Settings.ApplicationKey, Settings.PrivateKey,
                new UriPrototype
                {
                    Url = Settings.StorageUrl,
                    IsSecure = Settings.StorageSSL,
                    IsCluster = Settings.StorageIsCluster,
                },
                 new UriPrototype
                 {
                     Url = Settings.MessengerUrl,
                     IsSecure = Settings.MessengerSSL,
                     IsCluster = Settings.MessengerIsCluster,
                 }
                );
        }

        void Init(string applicationKey, string privateKey, UriPrototype storageurl, UriPrototype messengerUrl)
        {
            ApplicationKey = applicationKey;
            PrivateKey = privateKey;
            MessengerUrl = messengerUrl;
            Metadata = new StorageMetadata();
            Repository = new StorageRepository(applicationKey, PrivateKey, storageurl);
            
            Connect();
        }
        #endregion

        #region Notifications

        private void OnMessageCallback(string channel, string message)
        {
            if (_listeners[channel] != null)
            {
                var notifications = _listeners[channel];
                foreach (var notification in notifications)
                {
                    notification(message);
                }
            }
        }

        /// <summary>
        /// Registers a update notification callback
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        public void AddNotification(string channel, Action<string> handler)
        {
            if (Messenger != null)
            {
                Messenger.Subscribe(channel, OnMessageCallback);

                if (!_listeners.ContainsKey(channel))
                    _listeners.Add(channel, new List<Action<string>>());

                var things = _listeners[channel];

                if (!things.Contains(handler))
                    _listeners[channel].Add(handler);

            }
        }

        /// <summary>
        /// Removes an update notification callback
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        public void RemoveNotification(string channel, Action<string> handler)
        {
            if (!_listeners.ContainsKey(channel))
                return;

            var things = _listeners[channel];

            things.Remove(handler);
        }

        #endregion

        #region public tasks

        /// <summary>
        /// Creates a new table reference.
        /// </summary>
        /// <returns></returns>
        public Task<TableRef<T>> Table<T>() where T : class
        {
            return Task.Run(() =>
            {
                var self = new TableRef<T>(this);

                var task = Repository.GetTable(self.TableName);

                task.Wait();

                if (task.IsFaulted)
                    throw new Exception(task.Result.error.message);

                self.TableMeta = task.Result.data;

                return self;
            });
        }

        /// <summary>
        /// Is connected to the Update message gateway
        /// </summary>
        public bool IsConnected
        {
            get { return Messenger != null && Messenger.IsConnected; }
        }

        /// <summary>
        /// Connects the controller for the receipt of Storage update notification
        /// </summary>
        /// <returns></returns>
        public Task Connect()
        {
            if (string.IsNullOrEmpty(AuthenticationToken))
            {
                return new Task(new Exception("Authentication oken is required."));
            }

            if (Messenger == null)
            {
                Messenger = new StorageMessenger(MessengerUrl, ApplicationKey);
            }

            return Messenger.Connect(AuthenticationToken);
        }

        /// <summary>
        /// Disconnects the controller for the receipt of Storage update notification
        /// </summary>
        public void Disconnect()
        {
            if (Messenger != null)
            {
                Messenger.Disconnect();
            }
        }


        /// <summary>
        /// Coroutine for waiting for connection to be made.
        /// </summary>
        /// <returns></returns>
        public IEnumerator WaitForConnect()
        {
            if (IsConnected)
                yield break;

            yield return 1;
        }

        #endregion
    }
}