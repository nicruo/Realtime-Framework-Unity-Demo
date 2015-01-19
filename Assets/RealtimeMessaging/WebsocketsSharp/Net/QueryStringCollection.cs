// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))

using System;
using System.Collections.Specialized;
using System.Text;

namespace Realtime.Messaging.WebsocketsSharp.Net
{
  internal sealed class QueryStringCollection : NameValueCollection
  {
    public override string ToString ()
    {
      var cnt = Count;
      if (cnt == 0)
        return String.Empty;

      var output = new StringBuilder ();
      var keys = AllKeys;
      foreach (var key in keys)
        output.AppendFormat ("{0}={1}&", key, this [key]);

      if (output.Length > 0)
        output.Length--;

      return output.ToString ();
    }
  }
}
#endif
