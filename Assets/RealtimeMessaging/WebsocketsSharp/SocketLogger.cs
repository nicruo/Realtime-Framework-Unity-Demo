// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))

namespace Realtime.Messaging.WebsocketsSharp
{
    /// <summary>
    /// Provides a set of methods and properties for logging.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   If you output a log with lower than the <see cref="SocketLogger.Level"/>,
    ///   it cannot be outputted.
    ///   </para>
    ///   <para>
    ///   If you would like to use the custom output action, you should set the
    ///   <see cref="SocketLogger.Output"/> to any <c>Action&lt;LogData, string&gt;</c>.
    ///   </para>
    /// </remarks>
    public class SocketLogger
    {
        /// <summary>
        /// Enable Logging
        /// </summary>
        public bool Verbose = false;

        #region Public Methods

        /// <summary>
        /// Outputs <paramref name="message"/> as a log with <see cref="LogLevel.Debug"/>.
        /// </summary>
        /// <remarks>
        /// If the current logging level is higher than <see cref="LogLevel.Debug"/>, this method
        /// doesn't output <paramref name="message"/> as a log.
        /// </remarks>
        /// <param name="message">
        /// A <see cref="string"/> that represents the message to output as a log.
        /// </param>
        public void Debug(string message)
        {
            if (Verbose)
                UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// Outputs <paramref name="message"/> as a log with <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <remarks>
        /// If the current logging level is higher than <see cref="LogLevel.Error"/>, this method
        /// doesn't output <paramref name="message"/> as a log.
        /// </remarks>
        /// <param name="message">
        /// A <see cref="string"/> that represents the message to output as a log.
        /// </param>
        public void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// Outputs <paramref name="message"/> as a log with <see cref="LogLevel.Fatal"/>.
        /// </summary>
        /// <param name="message">
        /// A <see cref="string"/> that represents the message to output as a log.
        /// </param>
        public void Fatal(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// Outputs <paramref name="message"/> as a log with <see cref="LogLevel.Info"/>.
        /// </summary>
        /// <remarks>
        /// If the current logging level is higher than <see cref="LogLevel.Info"/>, this method
        /// doesn't output <paramref name="message"/> as a log.
        /// </remarks>
        /// <param name="message">
        /// A <see cref="string"/> that represents the message to output as a log.
        /// </param>
        public void Info(string message)
        {
            if (Verbose)
                UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// Outputs <paramref name="message"/> as a log with <see cref="LogLevel.Trace"/>.
        /// </summary>
        /// <remarks>
        /// If the current logging level is higher than <see cref="LogLevel.Trace"/>, this method
        /// doesn't output <paramref name="message"/> as a log.
        /// </remarks>
        /// <param name="message">
        /// A <see cref="string"/> that represents the message to output as a log.
        /// </param>
        public void Trace(string message)
        {
            if (Verbose)
                UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// Outputs <paramref name="message"/> as a log with <see cref="LogLevel.Warn"/>.
        /// </summary>
        /// <remarks>
        /// If the current logging level is higher than <see cref="LogLevel.Warn"/>, this method
        /// doesn't output <paramref name="message"/> as a log.
        /// </remarks>
        /// <param name="message">
        /// A <see cref="string"/> that represents the message to output as a log.
        /// </param>
        public void Warn(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        #endregion
    }
}
#endif
