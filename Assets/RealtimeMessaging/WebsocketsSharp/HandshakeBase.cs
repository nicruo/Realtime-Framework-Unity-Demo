// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using Realtime.Messaging.WebsocketsSharp.Net;
using System;
using System.Collections.Specialized;
using System.Text;

namespace Realtime.Messaging.WebsocketsSharp
{
  internal abstract class HandshakeBase
  {
    #region Private Fields

    private byte []             _entity;
    private NameValueCollection _headers;
    private Version             _version;

    #endregion

    #region Protected Const Fields

    protected const string CrLf = "\r\n";

    #endregion

    #region Protected Constructors

    protected HandshakeBase ()
    {
    }

    #endregion

    #region Internal Properties

    internal byte [] EntityBodyData {
      get {
        return _entity;
      }

      set {
        _entity = value;
      }
    }

    #endregion

    #region Public Properties

    public string EntityBody {
      get {
        return _entity != null && _entity.LongLength > 0
               ? getEncoding (_headers ["Content-Type"]).GetString (_entity)
               : String.Empty;
      }
    }

    public NameValueCollection Headers {
      get {
        return _headers ?? (_headers = new NameValueCollection ());
      }

      protected set {
        _headers = value;
      }
    }

    public Version ProtocolVersion {
      get {
        return _version ?? (_version = HttpVersion.Version11);
      }

      protected set {
        _version = value;
      }
    }

    #endregion

    #region Private Methods

    private static Encoding getEncoding (string contentType)
    {
      if (contentType == null || contentType.Length == 0)
        return Encoding.UTF8;

      var i = contentType.IndexOf ("charset=", StringComparison.Ordinal);
      if (i == -1)
        return Encoding.UTF8;

      var charset = contentType.Substring (i + 8);
      i = charset.IndexOf (';');
      if (i != -1)
        charset = charset.Substring (0, i);

      return Encoding.GetEncoding (charset);
    }

    #endregion

    #region Public Methods

    public byte [] ToByteArray ()
    {
      return Encoding.UTF8.GetBytes (ToString ());
    }
    
    #endregion
  }
}

#endif