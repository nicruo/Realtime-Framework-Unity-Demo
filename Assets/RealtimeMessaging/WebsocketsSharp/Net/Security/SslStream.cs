// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using System.Net.Security;
using System.Net.Sockets;

namespace Realtime.Messaging.WebsocketsSharp.Net.Security
{
  internal class SslStream : System.Net.Security.SslStream
  {
    #region Public Constructors

    public SslStream (NetworkStream innerStream)
      : base (innerStream)
    {
    }

    public SslStream (NetworkStream innerStream, bool leaveInnerStreamOpen)
      : base (innerStream, leaveInnerStreamOpen)
    {
    }

    public SslStream (
      NetworkStream innerStream,
      bool leaveInnerStreamOpen,
      RemoteCertificateValidationCallback userCertificateValidationCallback)
      : base (innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
    {
    }

    public SslStream (
      NetworkStream innerStream,
      bool leaveInnerStreamOpen,
      RemoteCertificateValidationCallback userCertificateValidationCallback,
      LocalCertificateSelectionCallback userCertificateSelectionCallback)
      : base (
        innerStream,
        leaveInnerStreamOpen,
        userCertificateValidationCallback,
        userCertificateSelectionCallback)
    {
    }

    #endregion

    #region Public Properties

    public bool DataAvailable {
      get {
        return ((NetworkStream) InnerStream).DataAvailable;
      }
    }

    #endregion
  }
}

#endif