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
  /// <summary>
  /// Contains the values of the schemes for authentication.
  /// </summary>
  [Flags]
  public enum AuthenticationSchemes
  {
    /// <summary>
    /// Indicates that no authentication is allowed.
    /// </summary>
    None,
    /// <summary>
    /// Indicates digest authentication.
    /// </summary>
    Digest = 1,
    /// <summary>
    /// Indicates basic authentication.
    /// </summary>
    Basic = 8,
    /// <summary>
    /// Indicates anonymous authentication.
    /// </summary>
    Anonymous = 0x8000
  }
}

#endif