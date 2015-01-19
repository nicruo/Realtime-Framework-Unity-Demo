// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
namespace Realtime.Messaging.WebsocketsSharp
{
  internal enum Mask : byte
  {
    Unmask = 0x0,
    Mask = 0x1
  }
}

#endif