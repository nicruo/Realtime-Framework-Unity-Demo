// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))


namespace Realtime.Messaging.WebsocketsSharp.Net
{
  internal class ReadBufferState
  {
    #region Public Constructors

    public ReadBufferState (
      byte [] buffer, int offset, int count, HttpStreamAsyncResult asyncResult)
    {
      Buffer = buffer;
      Offset = offset;
      Count = count;
      InitialCount = count;
      AsyncResult = asyncResult;
    }

    #endregion

    #region Public Properties

    public HttpStreamAsyncResult AsyncResult {
      get; set;
    }

    public byte [] Buffer {
      get; set;
    }

    public int Count {
      get; set;
    }

    public int InitialCount {
      get; set;
    }

    public int Offset {
      get; set;
    }

    #endregion
  }
}

#endif