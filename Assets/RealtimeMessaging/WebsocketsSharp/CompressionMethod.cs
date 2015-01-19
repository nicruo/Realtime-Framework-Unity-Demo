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
  /// Contains the values of the compression method used to compress the message on the WebSocket
  /// connection.
  /// </summary>
  /// <remarks>
  /// The values of the compression method are defined in
  /// <see href="http://tools.ietf.org/html/draft-ietf-hybi-permessage-compression-09">Compression
  /// Extensions for WebSocket</see>.
  /// </remarks>
  public enum CompressionMethod : byte
  {
    /// <summary>
    /// Indicates non compression.
    /// </summary>
    None,
    /// <summary>
    /// Indicates using DEFLATE.
    /// </summary>
    Deflate
  }
}
#endif
