// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
namespace Realtime.Messaging.Ortc
{
    /// <summary>
    /// Responsible for providing the correct OrtcClient per the current Unity3d Platform
    /// </summary>
    public class OrtcClientFactory
    {
        /// <summary>
        /// creates a new Ortc client
        /// </summary>
        /// <returns></returns>
        public static OrtcClient CreateClient()
        {
            
#if  UNITY_WEBPLAYER
            return null;
#elif  UNITY_WINRT
            return null;
#elif UNITY_PRO_LICENSE || UNITY_EDITOR ||  UNITY_STANDALONE 
            return new DotNet.DotNetClient();
#elif UNITY_ANDROID 
            return Android.AndroidOrtcClientFactory.CreateClient();
#elif(UNITY_IOS) 
            return Ios.IosOrtcClientFactory.CreateClient();
#elif UNITY_EDITOR
            return new DotNet.DotNetClient();
#else
            UnityEngine.Debug.LogError("No Ortc client found");
            return null;
#endif

        }
    }
}



