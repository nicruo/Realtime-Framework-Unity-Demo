// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Net;
using Realtime.Tasks;

namespace Realtime.Http
{
    public class HttpTask<T> : Task<T>
    {
        /// <summary>
        /// Computed from WebResponse
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public HttpTask()
        {
            Strategy = TaskStrategy.Custom;
        }
        
        public HttpTask(Exception ex)
        {

            Strategy = TaskStrategy.Custom;
            Exception = ex;
            Status = TaskStatus.Faulted;
            StatusCode = HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Called after the task is complete
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public HttpTask<T> ContinueWith(Action<HttpTask<T>> action)
        {
            if (IsCompleted)
            {
                action(this);
            }
            else
            {
                OnComplete.Add(action);
            }
            return this;
        }
    }

    /// <summary>
    /// Return result of the HttpServiceClient
    /// </summary>
    public class HttpTask : Task
    {
        /// <summary>
        /// Computed from WebResponse
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public HttpTask()
        {
            Strategy = TaskStrategy.Custom;
        }

        public HttpTask(Exception ex)
        {

            Strategy = TaskStrategy.Custom;
            Exception = ex;
            Status = TaskStatus.Faulted;
            StatusCode = HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Called after the task is complete
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public HttpTask ContinueWith(Action<HttpTask> action)
        {
            if (IsCompleted)
            {
                action(this);
            }
            else
            {
                OnComplete.Add(action);
            }
            return this;
        }
    }
}