// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using UnityEngine;

namespace Realtime.Storage.DataAccess
{

    /// <summary>
    /// Default Settings for the Storage API
    /// </summary>
    public class RealtimeStorageSettings : ScriptableObject
    {
        #region singleton

        private static RealtimeStorageSettings _instance;

        /// <summary>
        /// Access for the Network Manager
        /// </summary>
        public static RealtimeStorageSettings Instance
        {
            get
            {
                InitService();
                return _instance;
            }
        }

        /// <summary>
        /// Instantiates the RealtimeStorageSettings from Resources
        /// </summary>
        public static void InitService()
        {
            if (_instance != null)
                return;

            _instance = Resources.Load<RealtimeStorageSettings>("RealtimeStorageSettings");

            if (_instance == null)
            {
                Debug.LogWarning("StorageSettings Not Found. Please Use The Menu Command 'Realtime/Storage Settings'");
            }
        }

        #endregion

        /// <summary>
        /// REPLACE WITH UOUR APPLICATION KEY
        /// </summary>
        public string ApplicationKey;

        /// <summary>
        /// OPTIONAL : REPLACE WITH UOUR PRIVATE KEY.
        /// REQUIRED FOR AUTHENTICATION AND PRESENCE
        /// </summary>
        public string PrivateKey;

        /// <summary>
        /// service URL
        /// </summary>
        public string StorageUrl = "storage-balancer.realtime.co";

        /// <summary>
        /// SERVICE URL IS CLUSTER
        /// </summary>
        public bool StorageIsCluster = true;

        /// <summary>
        /// SERVICE URL IS HTTPS
        /// </summary>
        public bool StorageSSL = false;

        /// <summary>
        /// service URL
        /// </summary>
        public string MessengerUrl = "ortc-storage.realtime.co";

        /// <summary>
        /// SERVICE URL IS CLUSTER
        /// </summary>
        public bool MessengerIsCluster = true;

        /// <summary>
        /// SERVICE URL IS HTTPS
        /// </summary>
        public bool MessengerSSL = false;

        /// <summary>
        /// Sets all Settings to default
        /// </summary>
        public void ResetToDefault(){

            MessengerUrl = "ortc-storage.realtime.co";
            StorageUrl = "storage-balancer.realtime.co";
            MessengerIsCluster =  StorageIsCluster = true;
            StorageSSL = MessengerSSL = false;
            ApplicationKey = PrivateKey = string.Empty;
        }

    }
}