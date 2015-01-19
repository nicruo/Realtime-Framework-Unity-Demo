// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using System;
using System.Text;

namespace Realtime.Messaging.WebsocketsSharp
{
  /// <summary>
  /// Contains the event data associated with a <see cref="WebSocket.OnClose"/>
  /// event.
  /// </summary>
  /// <remarks>
  /// A <see cref="WebSocket.OnClose"/> event occurs when the WebSocket connection
  /// has been closed. If you want to get the reason for closure, you access the
  /// <see cref="Code"/> or <see cref="Reason"/> property.
  /// </remarks>
  public class CloseEventArgs : EventArgs
  {
    #region Private Fields

    private bool   _clean;
    private ushort _code;
    private string _reason;

    #endregion

    #region Internal Constructors

    internal CloseEventArgs (PayloadData payload)
    {
      var data = payload.ApplicationData;
      _code = getCodeFrom (data);
      _reason = getReasonFrom (data);
      _clean = false;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the status code for closure.
    /// </summary>
    /// <value>
    /// A <see cref="ushort"/> that indicates the status code for closure if any.
    /// </value>
    public ushort Code {
      get {
        return _code;
      }
    }

    /// <summary>
    /// Gets the reason for closure.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the reason for closure if any.
    /// </value>
    public string Reason {
      get {
        return _reason;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the WebSocket connection has been closed
    /// cleanly.
    /// </summary>
    /// <value>
    /// <c>true</c> if the WebSocket connection has been closed cleanly;
    /// otherwise, <c>false</c>.
    /// </value>
    public bool WasClean {
      get {
        return _clean;
      }

      internal set {
        _clean = value;
      }
    }

    #endregion

    #region Private Methods

    private static ushort getCodeFrom (byte [] data)
    {
      return data.Length > 1
             ? data.SubArray (0, 2).ToUInt16 (ByteOrder.Big)
             : (ushort) CloseStatusCode.NoStatusCode;
    }

    private static string getReasonFrom (byte [] data)
    {
      var len = data.Length;
      return len > 2
             ? Encoding.UTF8.GetString (data.SubArray (2, len - 2))
             : String.Empty;
    }

    #endregion
  }
}

#endif