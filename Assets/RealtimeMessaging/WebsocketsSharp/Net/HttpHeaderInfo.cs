// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
namespace Realtime.Messaging.WebsocketsSharp.Net
{
  internal class HttpHeaderInfo
  {
    #region Private Fields

    private HttpHeaderType _type;

    #endregion

    #region Public Constructors

    public HttpHeaderInfo ()
    {
    }

    #endregion

    #region Internal Properties

    internal bool IsMultiValueInRequest {
      get {
        return (_type & HttpHeaderType.MultiValueInRequest) == HttpHeaderType.MultiValueInRequest;
      }
    }

    internal bool IsMultiValueInResponse {
      get {
        return (_type & HttpHeaderType.MultiValueInResponse) == HttpHeaderType.MultiValueInResponse;
      }
    }

    #endregion

    #region Public Properties

    public bool IsRequest {
      get {
        return (_type & HttpHeaderType.Request) == HttpHeaderType.Request;
      }
    }

    public bool IsResponse {
      get {
        return (_type & HttpHeaderType.Response) == HttpHeaderType.Response;
      }
    }

    public string Name {
      get; set;
    }

    public HttpHeaderType Type {
      get {
        return _type;
      }

      set {
        _type = value;
      }
    }

    #endregion

    #region Public Methods

    public bool IsMultiValue (bool response)
    {
      return (_type & HttpHeaderType.MultiValue) != HttpHeaderType.MultiValue
             ? response
               ? IsMultiValueInResponse
               : IsMultiValueInRequest
             : response
               ? IsResponse
               : IsRequest;
    }

    public bool IsRestricted (bool response)
    {
      return (_type & HttpHeaderType.Restricted) != HttpHeaderType.Restricted
             ? false
             : response
               ? IsResponse
               : IsRequest;
    }

    #endregion
  }
}

#endif