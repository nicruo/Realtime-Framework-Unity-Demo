// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
#if (!UNITY_WEBPLAYER && (UNITY_PRO_LICENSE ||  UNITY_EDITOR ||  !(UNITY_IOS || UNITY_ANDROID)))
using System;
using System.Diagnostics;
using System.Text;

namespace Realtime.Messaging.WebsocketsSharp
{
  /// <summary>
  /// Represents a log data used by the <see cref="SocketLogger"/> class.
  /// </summary>
  public class LogData
  {
    #region Private Fields

    private StackFrame _caller;
    private DateTime   _date;
    private LogLevel   _level;
    private string     _message;

    #endregion

    #region Internal Constructors

    internal LogData (LogLevel level, StackFrame caller, string message)
    {
      _level = level;
      _caller = caller;
      _message = message ?? String.Empty;
      _date = DateTime.Now;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the information of the logging method caller.
    /// </summary>
    /// <value>
    /// A <see cref="StackFrame"/> that provides the information of the logging method caller.
    /// </value>
    public StackFrame Caller {
      get {
        return _caller;
      }
    }

    /// <summary>
    /// Gets the date and time when the log data was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> that represents the date and time when the log data was created.
    /// </value>
    public DateTime Date {
      get {
        return _date;
      }
    }

    /// <summary>
    /// Gets the logging level of the log data.
    /// </summary>
    /// <value>
    /// One of the <see cref="LogLevel"/> enum values, indicates the logging level of the log data.
    /// </value>
    public LogLevel Level {
      get {
        return _level;
      }
    }

    /// <summary>
    /// Gets the message of the log data.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the message of the log data.
    /// </value>
    public string Message {
      get {
        return _message;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a <see cref="string"/> that represents the current <see cref="LogData"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents the current <see cref="LogData"/>.
    /// </returns>
    public override string ToString ()
    {
      var header = String.Format ("{0}|{1,-5}|", _date, _level);
      var method = _caller.GetMethod ();
      var type = method.DeclaringType;
#if DEBUG
      var lineNum = _caller.GetFileLineNumber ();
      var headerAndCaller = String.Format (
        "{0}{1}.{2}:{3}|", header, type.Name, method.Name, lineNum);
#else
      var headerAndCaller = String.Format ("{0}{1}.{2}|", header, type.Name, method.Name);
#endif

      var messages = _message.Replace ("\r\n", "\n").TrimEnd ('\n').Split ('\n');
      if (messages.Length <= 1)
        return String.Format ("{0}{1}", headerAndCaller, _message);

      var log = new StringBuilder (
        String.Format ("{0}{1}\n", headerAndCaller, messages [0]), 64);

      var space = header.Length;
      var format = String.Format ("{{0,{0}}}{{1}}\n", space);
      for (var i = 1; i < messages.Length; i++)
        log.AppendFormat (format, "", messages [i]);

      log.Length--;
      return log.ToString ();
    }

    #endregion
  }
}

#endif