// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))

using System;
using System.Net;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  internal sealed class ListenerPrefix
  {
    #region Private Fields

    IPAddress [] _addresses;
    string       _host;
    string       _original;
    string       _path;
    ushort       _port;
    bool         _secure;

    #endregion

    #region Public Fields

    public HttpListener Listener;

    #endregion

    #region Public Constructors

    // Must be called after calling ListenerPrefix.CheckUriPrefix.
    public ListenerPrefix (string uriPrefix)
    {
      _original = uriPrefix;
      parse (uriPrefix);
    }

    #endregion

    #region Public Properties

    public IPAddress [] Addresses {
      get {
        return _addresses;
      }

      set {
        _addresses = value;
      }
    }

    public string Host {
      get {
        return _host;
      }
    }

    public string Path {
      get {
        return _path;
      }
    }

    public int Port {
      get {
        return (int) _port;
      }
    }

    public bool Secure {
      get {
        return _secure;
      }
    }

    #endregion

    #region Private Methods

    private void parse (string uriPrefix)
    {
      var defaultPort = uriPrefix.StartsWith ("https://") ? 443 : 80;
      if (defaultPort == 443)
        _secure = true;

      var length = uriPrefix.Length;
      var startHost = uriPrefix.IndexOf (':') + 3;
      var colon = uriPrefix.IndexOf (':', startHost, length - startHost);
      int root;
      if (colon > 0) {
        root = uriPrefix.IndexOf ('/', colon, length - colon);
        _host = uriPrefix.Substring (startHost, colon - startHost);
        _port = (ushort) Int32.Parse (uriPrefix.Substring (colon + 1, root - colon - 1));
        _path = uriPrefix.Substring (root);
      }
      else {
        root = uriPrefix.IndexOf ('/', startHost, length - startHost);
        _host = uriPrefix.Substring (startHost, root - startHost);
        _port = (ushort) defaultPort;
        _path = uriPrefix.Substring (root);
      }

      if (_path.Length > 1)
        _path = _path.Substring (0, _path.Length - 1);
    }

    #endregion

    #region public Methods

    public static void CheckUriPrefix (string uriPrefix)
    {
      if (uriPrefix == null)
        throw new ArgumentNullException ("uriPrefix");

      if (!uriPrefix.StartsWith ("http://") && !uriPrefix.StartsWith ("https://"))
        throw new ArgumentException ("Only 'http' and 'https' schemes are supported.");

      var length = uriPrefix.Length;
      var startHost = uriPrefix.IndexOf (':') + 3;
      if (startHost >= length)
        throw new ArgumentException ("No host specified.");

      var colon = uriPrefix.IndexOf (':', startHost, length - startHost);
      if (startHost == colon)
        throw new ArgumentException ("No host specified.");

      int root;
      if (colon > 0) {
        root = uriPrefix.IndexOf ('/', colon, length - colon);
        if (root == -1)
          throw new ArgumentException ("No path specified.");

        int port;
        if (!Int32.TryParse (uriPrefix.Substring (colon + 1, root - colon - 1), out port) ||
            (port <= 0 || port >= 65536))
          throw new ArgumentException ("Invalid port.");
      }
      else {
        root = uriPrefix.IndexOf ('/', startHost, length - startHost);
        if (root == -1)
          throw new ArgumentException ("No path specified.");
      }

      if (uriPrefix [uriPrefix.Length - 1] != '/')
        throw new ArgumentException ("The URI prefix must end with '/'.");
    }

    // Equals and GetHashCode are required to detect duplicates in HttpListenerPrefixCollection.
    public override bool Equals (object obj)
    {
      var other = obj as ListenerPrefix;
      return other != null
             ? _original == other._original
             : false;
    }

    public override int GetHashCode ()
    {
      return _original.GetHashCode ();
    }

    public override string ToString ()
    {
      return _original;
    }

    #endregion
  }
}

#endif