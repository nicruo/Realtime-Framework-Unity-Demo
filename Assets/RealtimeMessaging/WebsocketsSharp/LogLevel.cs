// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
namespace Realtime.Messaging.WebsocketsSharp
{
  /// <summary>
  /// Contains the values of the logging level.
  /// </summary>
  public enum LogLevel
  {
    /// <summary>
    /// Indicates the bottom logging level.
    /// </summary>
    Trace,
    /// <summary>
    /// Indicates the 2nd logging level from the bottom.
    /// </summary>
    Debug,
    /// <summary>
    /// Indicates the 3rd logging level from the bottom.
    /// </summary>
    Info,
    /// <summary>
    /// Indicates the 3rd logging level from the top.
    /// </summary>
    Warn,
    /// <summary>
    /// Indicates the 2nd logging level from the top.
    /// </summary>
    Error,
    /// <summary>
    /// Indicates the top logging level.
    /// </summary>
    Fatal
  }
}

#endif