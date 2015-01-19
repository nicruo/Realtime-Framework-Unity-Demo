// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using System;

namespace Realtime.Messaging.WebsocketsSharp
{
  /// <summary>
  /// Contains the event data associated with a <see cref="WebSocket.OnError"/> event.
  /// </summary>
  /// <remarks>
  /// A <see cref="WebSocket.OnError"/> event occurs when the <see cref="WebSocket"/> gets an error.
  /// If you want to get the error message, you access the <see cref="ErrorEventArgs.Message"/> property.
  /// </remarks>
  public class ErrorEventArgs : EventArgs
  {
    #region Internal Constructors

    internal ErrorEventArgs (string message)
    {
      Message = message;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that contains an error message.
    /// </value>
    public string Message { get; private set; }

    #endregion
  }
}

#endif