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
  /// Contains the values that indicate whether the byte order is a Little-endian or Big-endian.
  /// </summary>
  public enum ByteOrder : byte
  {
    /// <summary>
    /// Indicates a Little-endian.
    /// </summary>
    Little,
    /// <summary>
    /// Indicates a Big-endian.
    /// </summary>
    Big
  }
}

#endif