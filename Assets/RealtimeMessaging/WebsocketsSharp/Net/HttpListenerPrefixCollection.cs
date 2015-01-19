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

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  /// <summary>
  /// Provides the collection used to store the URI prefixes for the <see cref="HttpListener"/>.
  /// </summary>
  /// <remarks>
  /// The <see cref="HttpListener"/> responds to the request which has a requested URI that
  /// the prefixes most closely match.
  /// </remarks>
  public class HttpListenerPrefixCollection
    : ICollection<string>, IEnumerable<string>, IEnumerable
  {
    #region Private Fields

    private HttpListener _listener;
    private List<string> _prefixes;

    #endregion

    #region Private Constructors

    private HttpListenerPrefixCollection ()
    {
      _prefixes = new List<string> ();
    }

    #endregion

    #region Internal Constructors

    internal HttpListenerPrefixCollection (HttpListener listener)
      : this ()
    {
      _listener = listener;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the number of prefixes contained in the <see cref="HttpListenerPrefixCollection"/>.
    /// </summary>
    /// <value>
    /// An <see cref="int"/> that represents the number of prefixes.
    /// </value>
    public int Count {
      get {
        return _prefixes.Count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the access to the <see cref="HttpListenerPrefixCollection"/>
    /// is read-only.
    /// </summary>
    /// <value>
    /// Always returns <c>false</c>.
    /// </value>
    public bool IsReadOnly {
      get {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the access to the <see cref="HttpListenerPrefixCollection"/>
    /// is synchronized.
    /// </summary>
    /// <value>
    /// Always returns <c>false</c>.
    /// </value>
    public bool IsSynchronized {
      get {
        return false;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds the specified <paramref name="uriPrefix"/> to
    /// the <see cref="HttpListenerPrefixCollection"/>.
    /// </summary>
    /// <param name="uriPrefix">
    /// A <see cref="string"/> that represents the URI prefix to add. The prefix must be
    /// a well-formed URI prefix with http or https scheme, and must be terminated with
    /// a <c>"/"</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="uriPrefix"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="uriPrefix"/> is invalid.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="HttpListener"/> associated with
    /// this <see cref="HttpListenerPrefixCollection"/> is closed.
    /// </exception>
    public void Add (string uriPrefix)
    {
      _listener.CheckDisposed ();
      ListenerPrefix.CheckUriPrefix (uriPrefix);
      if (_prefixes.Contains (uriPrefix))
        return;

      _prefixes.Add (uriPrefix);
      if (_listener.IsListening)
        EndPointManager.AddPrefix (uriPrefix, _listener);
    }

    /// <summary>
    /// Removes all URI prefixes from the <see cref="HttpListenerPrefixCollection"/>.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="HttpListener"/> associated with
    /// this <see cref="HttpListenerPrefixCollection"/> is closed.
    /// </exception>
    public void Clear ()
    {
      _listener.CheckDisposed ();
      _prefixes.Clear ();
      if (_listener.IsListening)
        EndPointManager.RemoveListener (_listener);
    }

    /// <summary>
    /// Returns a value indicating whether the <see cref="HttpListenerPrefixCollection"/> contains
    /// the specified <paramref name="uriPrefix"/>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the <see cref="HttpListenerPrefixCollection"/> contains
    /// <paramref name="uriPrefix"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="uriPrefix">
    /// A <see cref="string"/> that represents the URI prefix to test.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="uriPrefix"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="HttpListener"/> associated with
    /// this <see cref="HttpListenerPrefixCollection"/> is closed.
    /// </exception>
    public bool Contains (string uriPrefix)
    {
      _listener.CheckDisposed ();
      if (uriPrefix == null)
        throw new ArgumentNullException ("uriPrefix");

      return _prefixes.Contains (uriPrefix);
    }

    /// <summary>
    /// Copies the contents of the <see cref="HttpListenerPrefixCollection"/> to
    /// the specified <see cref="Array"/>.
    /// </summary>
    /// <param name="array">
    /// An <see cref="Array"/> that receives the URI prefix strings in
    /// the <see cref="HttpListenerPrefixCollection"/>.
    /// </param>
    /// <param name="offset">
    /// An <see cref="int"/> that represents the zero-based index in <paramref name="array"/>
    /// at which copying begins.
    /// </param>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="HttpListener"/> associated with
    /// this <see cref="HttpListenerPrefixCollection"/> is closed.
    /// </exception>
    public void CopyTo (Array array, int offset)
    {
      _listener.CheckDisposed ();
      ((ICollection) _prefixes).CopyTo (array, offset);
    }

    /// <summary>
    /// Copies the contents of the <see cref="HttpListenerPrefixCollection"/> to
    /// the specified array of <see cref="string"/>.
    /// </summary>
    /// <param name="array">
    /// An array of <see cref="string"/> that receives the URI prefix strings in
    /// the <see cref="HttpListenerPrefixCollection"/>.
    /// </param>
    /// <param name="offset">
    /// An <see cref="int"/> that represents the zero-based index in <paramref name="array"/>
    /// at which copying begins.
    /// </param>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="HttpListener"/> associated with
    /// this <see cref="HttpListenerPrefixCollection"/> is closed.
    /// </exception>
    public void CopyTo (string [] array, int offset)
    {
      _listener.CheckDisposed ();
      _prefixes.CopyTo (array, offset);
    }

    /// <summary>
    /// Gets the enumerator used to iterate through the <see cref="HttpListenerPrefixCollection"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerator{string}"/> instance used to iterate
    /// through the <see cref="HttpListenerPrefixCollection"/>.
    /// </returns>
    public IEnumerator<string> GetEnumerator ()
    {
      return _prefixes.GetEnumerator ();
    }

    /// <summary>
    /// Removes the specified <paramref name="uriPrefix"/> from the list of prefixes in
    /// the <see cref="HttpListenerPrefixCollection"/>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <paramref name="uriPrefix"/> is successfully found and removed;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <param name="uriPrefix">
    /// A <see cref="string"/> that represents the URI prefix to remove.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="uriPrefix"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The <see cref="HttpListener"/> associated with
    /// this <see cref="HttpListenerPrefixCollection"/> is closed.
    /// </exception>
    public bool Remove (string uriPrefix)
    {
      _listener.CheckDisposed ();
      if (uriPrefix == null)
        throw new ArgumentNullException ("uriPrefix");

      var result = _prefixes.Remove (uriPrefix);
      if (result && _listener.IsListening)
        EndPointManager.RemovePrefix (uriPrefix, _listener);

      return result;
    }

    #endregion

    #region Explicit Interface Implementation

    /// <summary>
    /// Gets the enumerator used to iterate through the <see cref="HttpListenerPrefixCollection"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator"/> instance used to iterate through
    /// the <see cref="HttpListenerPrefixCollection"/>.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return _prefixes.GetEnumerator ();
    }

    #endregion
  }
}

#endif