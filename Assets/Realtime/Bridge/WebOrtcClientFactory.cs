// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------

#if UNITY_WEBPLAYER

using Realtime.Messaging.Client;

namespace Realtime.Messaging.Bridge
{
    /// <summary>
    /// 
    /// </summary>
    public class WebOrtcClientFactory
    {
        /// <summary>
        /// creates a new Android Client
        /// </summary>
        /// <returns></returns>
        static public DotNetClient CreateClient()
        {
            return new DotNetClient();
        }
    }
}
#endif