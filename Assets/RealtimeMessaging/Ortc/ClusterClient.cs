// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Text.RegularExpressions;
using System.Threading;
using Realtime.Http;
using Realtime.Tasks;

namespace Realtime.Messaging.Ortc
{
    /// <summary>
    /// Http Client for resolving the cluster url
    /// </summary>
    public class ClusterClient
    {
        const int MaxConnectionAttempts = 10;
        const int RetryThreadSleep = 500;
        const string ResponsePattern = "var SOCKET_SERVER = \"(?<host>.*)\";";


        private static HttpServiceClient _client;
        static HttpServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpServiceClient();

                    _client.ContentType = "application/x-www-form-urlencoded";
                }

                return _client;
            }
        }

        /// <summary>
        /// Gets the cluster server.
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetClusterServerAsync(string url, String applicationKey)
        {
            return new Task<string>(GetClusterServer(url, applicationKey));
        }

        /// <summary>
        /// Does the get cluster server logic.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static Task<string> GetClusterServerWithRetryAsync(string url, String applicationKey)
        {
            return new Task<string>(GetClusterServerWithRetry(url,applicationKey));
        }

        /// <summary>
        /// Gets the cluster server.
        /// </summary>
        /// <returns></returns>
        public static string GetClusterServer(string url, String applicationKey)
        {

            // todo should this be cached ?

            var clusterRequestParameter = !String.IsNullOrEmpty(applicationKey) ? String.Format("appkey={0}", applicationKey) : String.Empty;
            var clusterUrl = String.Format("{0}{1}?{2}", url, !String.IsNullOrEmpty(url) && url[url.Length - 1] != '/' ? "/" : String.Empty, clusterRequestParameter);

            var hTask = Client.GetAsync(clusterUrl);

            hTask.Wait();

            if (hTask.IsFaulted)
                throw hTask.Exception;

            return ParseResponse(hTask.Content);
        }

        /// <summary>
        /// Does the get cluster server logic.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static string GetClusterServerWithRetry(string url, String applicationKey)
        {
            int currentAttempts;
            string connectionUrl = null;

            for (currentAttempts = 0; currentAttempts <= MaxConnectionAttempts && String.IsNullOrEmpty(connectionUrl);currentAttempts++)
            {

                connectionUrl = GetClusterServer(url, applicationKey).Trim();

                if (!String.IsNullOrEmpty(connectionUrl))
                    continue;

                currentAttempts++;

                if (currentAttempts > MaxConnectionAttempts)
                {
                    throw new OrtcException(OrtcExceptionReason.ConnectionError, "Unable to connect to the authentication server.");
                }

                Thread.Sleep(RetryThreadSleep);
            }
           
            return connectionUrl;

        }

        /// <summary>
        /// parses the response
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ParseResponse(string input)
        {
            var match = Regex.Match(input, ResponsePattern);

            return match.Groups["host"].Value;
        }
    }
}