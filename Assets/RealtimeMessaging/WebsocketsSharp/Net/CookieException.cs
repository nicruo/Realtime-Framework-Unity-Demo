// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))

using System;
using System.Runtime.Serialization;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  /// <summary>
  /// The exception that is thrown when a <see cref="Cookie"/> gets an error.
  /// </summary>
  [Serializable]
  public class CookieException : FormatException, ISerializable
  {
    #region Internal Constructors

    internal CookieException (string message)
      : base (message)
    {
    }

    internal CookieException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    #endregion

    #region Protected Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieException"/> class from
    /// the specified <see cref="SerializationInfo"/> and <see cref="StreamingContext"/>.
    /// </summary>
    /// <param name="serializationInfo">
    /// A <see cref="SerializationInfo"/> that contains the serialized object data.
    /// </param>
    /// <param name="streamingContext">
    /// A <see cref="StreamingContext"/> that specifies the source for the deserialization.
    /// </param>
    protected CookieException (
      SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base (serializationInfo, streamingContext)
    {
    }

    #endregion

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CookieException"/> class.
    /// </summary>
    public CookieException ()
      : base ()
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Populates the specified <see cref="SerializationInfo"/> with the data needed to serialize
    /// the current <see cref="CookieException"/>.
    /// </summary>
    /// <param name="serializationInfo">
    /// A <see cref="SerializationInfo"/> that holds the serialized object data.
    /// </param>
    /// <param name="streamingContext">
    /// A <see cref="StreamingContext"/> that specifies the destination for the serialization.
    /// </param>
   
    public override void GetObjectData (
      SerializationInfo serializationInfo, StreamingContext streamingContext)
    {
      base.GetObjectData (serializationInfo, streamingContext);
    }

    #endregion

    #region Explicit Interface Implementation

    /// <summary>
    /// Populates the specified <see cref="SerializationInfo"/> with the data needed to serialize
    /// the current <see cref="CookieException"/>.
    /// </summary>
    /// <param name="serializationInfo">
    /// A <see cref="SerializationInfo"/> that holds the serialized object data.
    /// </param>
    /// <param name="streamingContext">
    /// A <see cref="StreamingContext"/> that specifies the destination for the serialization.
    /// </param>

    void ISerializable.GetObjectData (SerializationInfo serializationInfo, StreamingContext streamingContext)
    {
      base.GetObjectData (serializationInfo, streamingContext);
    }

    #endregion
  }
}
#endif