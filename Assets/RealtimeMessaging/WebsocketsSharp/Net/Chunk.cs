// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))


using System;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  internal class Chunk
  {
    #region Private Fields

    private byte [] _data;
    private int     _offset;

    #endregion

    #region Public Constructors

    public Chunk (byte [] data)
    {
      _data = data;
    }

    #endregion

    #region Public Properties

    public int ReadLeft {
      get {
        return _data.Length - _offset;
      }
    }

    #endregion

    #region Public Methods

    public int Read (byte [] buffer, int offset, int size)
    {
      var left = _data.Length - _offset;
      if (left == 0)
        return left;

      if (size > left)
        size = left;

      Buffer.BlockCopy (_data, _offset, buffer, offset, size);
      _offset += size;

      return size;
    }

    #endregion
  }
}

#endif