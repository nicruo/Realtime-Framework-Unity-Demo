// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))

using System;
using System.Threading;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  internal class HttpStreamAsyncResult : IAsyncResult
  {
    #region Private Fields

    private AsyncCallback    _callback;
    private bool             _completed;
    private object           _state;
    private object           _sync;
    private ManualResetEvent _waitHandle;

    #endregion

    #region Internal Fields

    internal byte []   Buffer;
    internal int       Count;
    internal Exception Error;
    internal int       Offset;
    internal int       SyncRead;

    #endregion

    #region Public Constructors

    public HttpStreamAsyncResult (AsyncCallback callback, object state)
    {
      _callback = callback;
      _state = state;
      _sync = new object ();
    }

    #endregion

    #region Public Properties

    public object AsyncState {
      get {
        return _state;
      }
    }

    public WaitHandle AsyncWaitHandle {
      get {
        lock (_sync)
          return _waitHandle ?? (_waitHandle = new ManualResetEvent (_completed));
      }
    }

    public bool CompletedSynchronously {
      get {
        return SyncRead == Count;
      }
    }

    public bool IsCompleted {
      get {
        lock (_sync)
          return _completed;
      }
    }

    #endregion

    #region Public Methods

    public void Complete ()
    {
      lock (_sync) {
        if (_completed)
          return;

        _completed = true;
        if (_waitHandle != null)
          _waitHandle.Set ();

        if (_callback != null)
          _callback.BeginInvoke (this, ar => _callback.EndInvoke (ar), null);
      }
    }

    public void Complete (Exception exception)
    {
      Error = exception;
      Complete ();
    }

    #endregion
  }
}
#endif
