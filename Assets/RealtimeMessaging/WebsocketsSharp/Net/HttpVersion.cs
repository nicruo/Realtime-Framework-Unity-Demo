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
  /// Provides the HTTP version numbers.
  /// </summary>
  public class HttpVersion
  {
    #region Public Static Fields

    /// <summary>
    /// Provides a <see cref="Version"/> instance for HTTP 1.0.
    /// </summary>
    public static readonly Version Version10 = new Version (1, 0);

    /// <summary>
    /// Provides a <see cref="Version"/> instance for HTTP 1.1.
    /// </summary>
    public static readonly Version Version11 = new Version (1, 1);

    #endregion

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpVersion"/> class.
    /// </summary>
    public HttpVersion ()
    {
    }

    #endregion
  }
}
#endif
