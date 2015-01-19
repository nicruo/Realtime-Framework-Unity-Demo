// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using System;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  [Flags]
  internal enum HttpHeaderType
  {
    Unspecified = 0,
    Request = 1,
    Response = 1 << 1,
    Restricted = 1 << 2,
    MultiValue = 1 << 3,
    MultiValueInRequest = 1 << 4,
    MultiValueInResponse = 1 << 5
  }
}

#endif