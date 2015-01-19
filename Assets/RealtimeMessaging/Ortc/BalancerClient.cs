// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Text.RegularExpressions;
using Realtime.Http;
using Realtime.Tasks;

namespace Realtime.Messaging.Ortc
{

    /// <summary>
    /// A static class containing all the methods to communicate with the Ortc Balancer 
    /// </summary>
    public static class BalancerClient
    {
        const String BalancerServerPattern = "^var SOCKET_SERVER = \"(?<server>http.*)\";$";

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
        /// Retrieves an Ortc Server url from the Ortc Balancer
        /// </summary>
        /// <param name="balancerUrl">The Ortc Balancer url.</param>
        /// <param name="applicationKey"></param>
        /// <remarks></remarks>
        public static Task<string> GetServerFromBalancerAsync(String balancerUrl, String applicationKey)
        {
            return new Task<string>(GetServerFromBalance(balancerUrl, applicationKey));
        }

        /// <summary>
        /// Retrieves an Ortc Server url from the Ortc Balancer
        /// </summary>
        /// <param name="balancerUrl">The Ortc Balancer url.</param>
        /// <param name="applicationKey"></param>
        /// <remarks></remarks>
        public static string GetServerFromBalance(String balancerUrl, String applicationKey)
        {
            var parsedUrl = String.IsNullOrEmpty(applicationKey) ? balancerUrl : balancerUrl + "?appkey=" + applicationKey;

            var task = Client.GetAsync(parsedUrl);

            task.Wait();

            if (task.IsFaulted)
            {
                throw task.Exception;
            }

            return ParseBalancerResponse(task.Content);
        }

        /// <summary>
        /// Retrieves an Ortc Server url from the Ortc Balancer
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isCluster"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static Task<string> GetServerUrlAsync(String url, bool isCluster, String applicationKey)
        {
            return new Task<string>(GetServerUrl(url,isCluster, applicationKey));
        }

        /// <summary>
        /// Retrieves an Ortc Server url from the Ortc Balancer
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isCluster"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static string GetServerUrl(String url, bool isCluster, String applicationKey)
        {
            if (!String.IsNullOrEmpty(url) && isCluster)
            {
                return GetServerFromBalance(url, applicationKey);
            }

            return url;
        }

        // Private Methods (1) 

        private static String ParseBalancerResponse(string responseBody)
        {
            var match = Regex.Match(responseBody, BalancerServerPattern);

            return match.Success ? match.Groups["server"].Value : string.Empty;
        }
        
    }
}
