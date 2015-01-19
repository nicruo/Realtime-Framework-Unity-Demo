// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using Realtime.Messaging.WebsocketsSharp.Net.WebSockets;
using System;
using System.Security.Principal;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  /// <summary>
  /// Provides a set of methods and properties used to access the HTTP request and response
  /// information used by the <see cref="HttpListener"/>.
  /// </summary>
  /// <remarks>
  /// The HttpListenerContext class cannot be inherited.
  /// </remarks>
  public sealed class HttpListenerContext
  {
#region Private Fields

    private HttpConnection       _connection;
    private string               _error;
    private int                  _errorStatus;
    private HttpListenerRequest  _request;
    private HttpListenerResponse _response;
    private IPrincipal           _user;

    #endregion

#region Internal Fields

    internal HttpListener Listener;

    #endregion

#region Internal Constructors

    internal HttpListenerContext (HttpConnection connection)
    {
      _connection = connection;
      _errorStatus = 400;
      _request = new HttpListenerRequest (this);
      _response = new HttpListenerResponse (this);
    }

    #endregion

#region Internal Properties

    internal HttpConnection Connection {
      get {
        return _connection;
      }
    }

    internal string ErrorMessage {
      get {
        return _error;
      }

      set {
        _error = value;
      }
    }

    internal int ErrorStatus {
      get {
        return _errorStatus;
      }

      set {
        _errorStatus = value;
      }
    }

    internal bool HasError {
      get {
        return _error != null;
      }
    }

    #endregion

#region Public Properties

    /// <summary>
    /// Gets the HTTP request information from a client.
    /// </summary>
    /// <value>
    /// A <see cref="HttpListenerRequest"/> that represents the HTTP request.
    /// </value>
    public HttpListenerRequest Request {
      get {
        return _request;
      }
    }

    /// <summary>
    /// Gets the HTTP response information used to send to the client.
    /// </summary>
    /// <value>
    /// A <see cref="HttpListenerResponse"/> that represents the HTTP response to send.
    /// </value>
    public HttpListenerResponse Response {
      get {
        return _response;
      }
    }

    /// <summary>
    /// Gets the client information (identity, authentication, and security roles).
    /// </summary>
    /// <value>
    /// A <see cref="IPrincipal"/> that represents the client information.
    /// </value>
    public IPrincipal User {
      get {
        return _user;
      }
    }

    #endregion

#region Internal Methods

    internal void SetUser (
      AuthenticationSchemes scheme,
      string realm,
      Func<IIdentity, NetworkCredential> credentialsFinder)
    {
      var authRes = AuthenticationResponse.Parse (_request.Headers ["Authorization"]);
      if (authRes == null)
        return;

      var id = authRes.ToIdentity ();
      if (id == null)
        return;

      NetworkCredential cred = null;
      try {
        cred = credentialsFinder (id);
      }
      catch {
      }

      if (cred == null)
        return;

      var valid = scheme == AuthenticationSchemes.Basic
                  ? ((HttpBasicIdentity) id).Password == cred.Password
                  : scheme == AuthenticationSchemes.Digest
                    ? ((HttpDigestIdentity) id).IsValid (
                        cred.Password, realm, _request.HttpMethod, null)
                    : false;

      if (valid)
        _user = new GenericPrincipal (id, cred.Roles);
    }

    #endregion

#region Public Methods

    /// <summary>
    /// Accepts a WebSocket connection request.
    /// </summary>
    /// <returns>
    /// A <see cref="HttpListenerWebSocketContext"/> that represents the WebSocket connection
    /// request.
    /// </returns>
    /// <param name="protocol">
    /// A <see cref="string"/> that represents the subprotocol used in the WebSocket connection.
    /// </param>
    /// <param name="logger">
    /// A <see cref="SocketLogger"/> that provides the logging functions used in the WebSocket attempts.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="protocol"/> is empty.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="protocol"/> contains an invalid character.
    ///   </para>
    /// </exception>
    public HttpListenerWebSocketContext AcceptWebSocket (string protocol, SocketLogger logger)
    {
      if (protocol != null) {
        if (protocol.Length == 0)
          throw new ArgumentException ("An empty string.", "protocol");

        if (!protocol.IsToken ())
          throw new ArgumentException ("Contains an invalid character.", "protocol");
      }

      return new HttpListenerWebSocketContext (this, protocol, logger ?? new SocketLogger ());
    }

    #endregion
  }
}

#endif