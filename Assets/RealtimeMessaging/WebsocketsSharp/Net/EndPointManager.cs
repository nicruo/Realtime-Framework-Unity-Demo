// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  internal sealed class EndPointManager
  {
    #region Private Static Fields

    private static Dictionary<IPAddress, Dictionary<int, EndPointListener>> _ipToEndpoints =
      new Dictionary<IPAddress, Dictionary<int, EndPointListener>> ();

    #endregion

    #region Private Constructors

    private EndPointManager ()
    {
    }

    #endregion

    #region Private Methods

    private static void addPrefix (string uriPrefix, HttpListener httpListener)
    {
      var prefix = new ListenerPrefix (uriPrefix);
      if (prefix.Path.IndexOf ('%') != -1)
        throw new HttpListenerException (400, "Invalid path."); // TODO: Code?

      if (prefix.Path.IndexOf ("//", StringComparison.Ordinal) != -1)
        throw new HttpListenerException (400, "Invalid path."); // TODO: Code?

      // Always listens on all the interfaces, no matter the host name/ip used.
      var epListener = getEndPointListener (
        IPAddress.Any, prefix.Port, httpListener, prefix.Secure);

      epListener.AddPrefix (prefix, httpListener);
    }

    private static EndPointListener getEndPointListener (
      IPAddress address, int port, HttpListener httpListener, bool secure)
    {
      Dictionary<int, EndPointListener> endpoints = null;
      if (_ipToEndpoints.ContainsKey (address)) {
        endpoints = _ipToEndpoints [address];
      }
      else {
        endpoints = new Dictionary<int, EndPointListener> ();
        _ipToEndpoints [address] = endpoints;
      }

      EndPointListener epListener = null;
      if (endpoints.ContainsKey (port)) {
        epListener = endpoints [port];
      }
      else {
        epListener = new EndPointListener (
          address,
          port,
          secure,
          httpListener.CertificateFolderPath,
          httpListener.DefaultCertificate);

        endpoints [port] = epListener;
      }

      return epListener;
    }

    private static void removePrefix (string uriPrefix, HttpListener httpListener)
    {
      var prefix = new ListenerPrefix (uriPrefix);
      if (prefix.Path.IndexOf ('%') != -1)
        return;

      if (prefix.Path.IndexOf ("//", StringComparison.Ordinal) != -1)
        return;

      var epListener = getEndPointListener (
        IPAddress.Any, prefix.Port, httpListener, prefix.Secure);

      epListener.RemovePrefix (prefix, httpListener);
    }

    #endregion

    #region Public Methods

    public static void AddListener (HttpListener httpListener)
    {
      var added = new List<string> ();
      lock (((ICollection) _ipToEndpoints).SyncRoot) {
        try {
          foreach (var prefix in httpListener.Prefixes) {
            addPrefix (prefix, httpListener);
            added.Add (prefix);
          }
        }
        catch {
          foreach (var prefix in added)
            removePrefix (prefix, httpListener);

          throw;
        }
      }
    }

    public static void AddPrefix (string uriPrefix, HttpListener httpListener)
    {
      lock (((ICollection) _ipToEndpoints).SyncRoot)
        addPrefix (uriPrefix, httpListener);
    }

    public static void RemoveEndPoint (EndPointListener epListener, IPEndPoint endpoint)
    {
      lock (((ICollection) _ipToEndpoints).SyncRoot) {
        var endpoints = _ipToEndpoints [endpoint.Address];
        endpoints.Remove (endpoint.Port);
        if (endpoints.Count == 0)
          _ipToEndpoints.Remove (endpoint.Address);

        epListener.Close ();
      }
    }

    public static void RemoveListener (HttpListener httpListener)
    {
      lock (((ICollection) _ipToEndpoints).SyncRoot)
        foreach (var prefix in httpListener.Prefixes)
          removePrefix (prefix, httpListener);
    }

    public static void RemovePrefix (string uriPrefix, HttpListener httpListener)
    {
      lock (((ICollection) _ipToEndpoints).SyncRoot)
        removePrefix (uriPrefix, httpListener);
    }

    #endregion
  }
}

#endif