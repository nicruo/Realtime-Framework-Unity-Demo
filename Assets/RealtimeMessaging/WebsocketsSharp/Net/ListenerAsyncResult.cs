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
  internal class ListenerAsyncResult : IAsyncResult
  {
    #region Private Fields

    private AsyncCallback       _callback;
    private bool                _completed;
    private HttpListenerContext _context;
    private Exception           _exception;
    private ManualResetEvent    _waitHandle;
    private object              _state;
    private object              _sync;
    private bool                _syncCompleted;

    #endregion

    #region Internal Fields

    internal bool EndCalled;
    internal bool InGet;

    #endregion

    #region Public Constructors

    public ListenerAsyncResult (AsyncCallback callback, object state)
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
        return _syncCompleted;
      }
    }

    public bool IsCompleted {
      get {
        lock (_sync)
          return _completed;
      }
    }

    #endregion

    #region Private Methods

    private static void invokeCallback (object state)
    {
      try {
        var ares = (ListenerAsyncResult) state;
        ares._callback (ares);
      }
      catch {
      }
    }

    #endregion

    #region Internal Methods

    internal void Complete (Exception exception)
    {
      _exception = InGet && (exception is ObjectDisposedException)
                   ? new HttpListenerException (500, "Listener closed.")
                   : exception;

      lock (_sync) {
        _completed = true;
        if (_waitHandle != null)
          _waitHandle.Set ();

        if (_callback != null)
          ThreadPool.QueueUserWorkItem(invokeCallback, this);
      }
    }

    internal void Complete (HttpListenerContext context)
    {
      Complete (context, false);
    }

    internal void Complete (HttpListenerContext context, bool syncCompleted)
    {
      var listener = context.Listener;
      var scheme = listener.SelectAuthenticationScheme (context);
      if (scheme == AuthenticationSchemes.None) {
        context.Response.Close (HttpStatusCode.Forbidden);
        listener.BeginGetContext (this);

        return;
      }

      var header = context.Request.Headers ["Authorization"];
      if (scheme == AuthenticationSchemes.Basic &&
          (header == null || !header.StartsWith ("basic", StringComparison.OrdinalIgnoreCase))) {
        context.Response.CloseWithAuthChallenge (
          AuthenticationChallenge.CreateBasicChallenge (listener.Realm).ToBasicString ());

        listener.BeginGetContext (this);
        return;
      }

      if (scheme == AuthenticationSchemes.Digest &&
          (header == null || !header.StartsWith ("digest", StringComparison.OrdinalIgnoreCase))) {
        context.Response.CloseWithAuthChallenge (
          AuthenticationChallenge.CreateDigestChallenge (listener.Realm).ToDigestString ());

        listener.BeginGetContext (this);
        return;
      }

      _context = context;
      _syncCompleted = syncCompleted;

      lock (_sync) {
        _completed = true;
        if (_waitHandle != null)
          _waitHandle.Set ();

        if (_callback != null)
            ThreadPool.QueueUserWorkItem(invokeCallback, this);
      }
    }

    internal HttpListenerContext GetContext ()
    {
      if (_exception != null)
        throw _exception;

      return _context;
    }

    #endregion
  }
}

#endif