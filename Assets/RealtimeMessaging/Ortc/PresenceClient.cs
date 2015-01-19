// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Realtime.Http;
using Realtime.Tasks;

namespace Realtime.Messaging.Ortc
{

    /// <summary>
    /// Presence info, such as total subscriptions and metadata.
    /// </summary>
    public class Presence
    {
        /// <summary>
        /// Gets the subscriptions value.
        /// </summary>
        public long Subscriptions { get; set; }

        /// <summary>
        /// Gets the first 100 unique metadata.
        /// </summary>
        public Dictionary<String, long> Metadata { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresenceClient"/> class.
        /// </summary>
        public Presence()
        {
            Subscriptions = 0;
            Metadata = new Dictionary<String, long>();
        }
    }

    /// <summary>
    /// Static client for accessing presence data
    /// </summary>
    public class PresenceClient
    {
        private const string SubscriptionsPattern = "^{\"subscriptions\":(?<subscriptions>\\d*),\"metadata\":{(?<metadata>.*)}}$";
        private const string MetadataPattern = "\"([^\"]*|[^:,]*)*\":(\\d*)";
        private const string MetadataDetailPattern = "\"(.*)\":(\\d*)";

        private static HttpServiceClient _client;
        static HttpServiceClient HttpClient
        {
            get { return _client ?? (_client = new HttpServiceClient {ContentType = "application/x-www-form-urlencoded"}); }
        }


        /// <summary>
        /// Deserializes the specified json string to a presence object.
        /// </summary>
        /// <param name="message">Json string to deserialize.</param>
        /// <returns></returns>
        public static Presence Deserialize(string message)
        {
            var result = new Presence();

            if (String.IsNullOrEmpty(message))
                return result;

            var json = message.Replace("\\\\\"", @"""");
            json = Regex.Unescape(json);

            var presenceMatch = Regex.Match(json, SubscriptionsPattern, RegexOptions.None);

            int subscriptions;

            if (int.TryParse(presenceMatch.Groups["subscriptions"].Value, out subscriptions))
            {
                var metadataContent = presenceMatch.Groups["metadata"].Value;

                var metadataRegex = new Regex(MetadataPattern, RegexOptions.None);
                foreach (Match metadata in metadataRegex.Matches(metadataContent))
                {
                    if (metadata.Groups.Count <= 1)
                        continue;

                    var metadataDetailMatch = Regex.Match(metadata.Groups[0].Value, MetadataDetailPattern, RegexOptions.None);

                    int metadataSubscriptions;

                    if (int.TryParse(metadataDetailMatch.Groups[2].Value, out metadataSubscriptions))
                    {
                        result.Metadata.Add(metadataDetailMatch.Groups[1].Value, metadataSubscriptions);
                    }
                }
            }

            result.Subscriptions = subscriptions;

            return result;
        }

        /// <summary>
        /// Gets the subscriptions in the specified channel and if active the first 100 unique metadata.
        /// </summary>
        /// <param name="url">Server containing the presence service.</param>
        /// <param name="isCluster">Specifies if url is cluster.</param>
        /// <param name="applicationKey">Application key with access to presence service.</param>
        /// <param name="authenticationToken">Authentication token with access to presence service.</param>
        /// <param name="channel">Channel with presence data active.</param>
        public static Task<Presence> GetPresenceAsync(String url, bool isCluster, String applicationKey, String authenticationToken, String channel)
        {
            return Task.Run(() => GetPresence(url, isCluster, applicationKey, authenticationToken, channel));
        }

        /// <summary>
        /// Enables presence for the specified channel with first 100 unique metadata if metadata is set to true.
        /// </summary>
        /// <param name="url">Server containing the presence service.</param>
        /// <param name="isCluster">Specifies if url is cluster.</param>
        /// <param name="applicationKey">Application key with access to presence service.</param>
        /// <param name="privateKey">The private key provided when the ORTC service is purchased.</param>
        /// <param name="channel">Channel to activate presence.</param>
        /// <param name="metadata">Defines if to collect first 100 unique metadata.</param>
        public static Task<string> EnablePresenceAsync(String url, bool isCluster, String applicationKey, String privateKey, String channel, bool metadata)
        {
            return Task.Run(() => EnablePresence(url, isCluster, applicationKey, privateKey, channel, metadata));
        }

        /// <summary>
        /// Disables presence for the specified channel.
        /// </summary>
        /// <param name="url">Server containing the presence service.</param>
        /// <param name="isCluster">Specifies if url is cluster.</param>
        /// <param name="applicationKey">Application key with access to presence service.</param>
        /// <param name="privateKey">The private key provided when the ORTC service is purchased.</param>
        /// <param name="channel">Channel to disable presence.</param>
        public static Task<string> DisablePresenceAsync(String url, bool isCluster, String applicationKey, String privateKey, String channel)
        {
            return Task.Run(() =>DisablePresence(url, isCluster, applicationKey, privateKey, channel));
        }

        /// <summary>
        /// Gets the subscriptions in the specified channel and if active the first 100 unique metadata.
        /// </summary>
        /// <param name="url">Server containing the presence service.</param>
        /// <param name="isCluster">Specifies if url is cluster.</param>
        /// <param name="applicationKey">Application key with access to presence service.</param>
        /// <param name="authenticationToken">Authentication token with access to presence service.</param>
        /// <param name="channel">Channel with presence data active.</param>
        public static Presence GetPresence(String url, bool isCluster, String applicationKey, String authenticationToken, String channel)
        {
            var btask = BalancerClient.GetServerUrl(url, isCluster, applicationKey);
            
            var server = btask;
            var presenceUrl = String.IsNullOrEmpty(server) ? server : server[server.Length - 1] == '/' ? server : server + "/";
            presenceUrl = String.Format("{0}presence/{1}/{2}/{3}", presenceUrl, applicationKey, authenticationToken, channel);

            var htask = HttpClient.GetAsync(presenceUrl);

            htask.Wait();

            if (htask.IsFaulted)
            {
                throw htask.Exception;
            }

            return Deserialize(htask.Content);

        }

        /// <summary>
        /// Enables presence for the specified channel with first 100 unique metadata if metadata is set to true.
        /// </summary>
        /// <param name="url">Server containing the presence service.</param>
        /// <param name="isCluster">Specifies if url is cluster.</param>
        /// <param name="applicationKey">Application key with access to presence service.</param>
        /// <param name="privateKey">The private key provided when the ORTC service is purchased.</param>
        /// <param name="channel">Channel to activate presence.</param>
        /// <param name="metadata">Defines if to collect first 100 unique metadata.</param>
        public static string EnablePresence(String url, bool isCluster, String applicationKey, String privateKey, String channel, bool metadata)
        {
            var btask = BalancerClient.GetServerUrl(url, isCluster, applicationKey);
            
            var server = btask;
            var presenceUrl = String.IsNullOrEmpty(server) ? server : server[server.Length - 1] == '/' ? server : server + "/";
            presenceUrl = String.Format("{0}presence/enable/{1}/{2}", presenceUrl, applicationKey, channel);

            var content = String.Format("privatekey={0}", privateKey);

            if (metadata)
            {
                content = String.Format("{0}&metadata=1", content);
            }

            var htask = HttpClient.PostAsync(presenceUrl, content);

            htask.Wait();

            if (htask.IsFaulted)
                throw htask.Exception;

            return htask.Content;
        }

        /// <summary>
        /// Disables presence for the specified channel.
        /// </summary>
        /// <param name="url">Server containing the presence service.</param>
        /// <param name="isCluster">Specifies if url is cluster.</param>
        /// <param name="applicationKey">Application key with access to presence service.</param>
        /// <param name="privateKey">The private key provided when the ORTC service is purchased.</param>
        /// <param name="channel">Channel to disable presence.</param>
        public static string DisablePresence(String url, bool isCluster, String applicationKey, String privateKey, String channel)
        {
            var btask = BalancerClient.GetServerUrl(url, isCluster, applicationKey);

            var server = btask;
            var presenceUrl = String.IsNullOrEmpty(server) ? server : server[server.Length - 1] == '/' ? server : server + "/";
            presenceUrl = String.Format("{0}presence/disable/{1}/{2}", presenceUrl, applicationKey, channel);
            var content = String.Format("privatekey={0}", privateKey);

            var htask = HttpClient.PostAsync(presenceUrl, content);

            htask.Wait();

            if (htask.IsFaulted)
                throw htask.Exception;

            return htask.Content;
        }
    }
}
