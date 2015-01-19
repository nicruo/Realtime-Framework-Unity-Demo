// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Realtime.LITJson;
using Realtime.Tasks;
using UnityEngine;

namespace Realtime.Http
{
    /// <summary>
    /// A http client which returns HttpTasks's
    /// </summary>
    public class HttpServiceClient
    {
        /// <summary>
        /// content type Header. Default value of "application/json"
        /// </summary>
        public string ContentType = "application/json";

        /// <summary>
        /// Accept Header. Default value of "application/json"
        /// </summary>
        public string Accept = "application/json";

        /// <summary>
        /// Http Headers Collection
        /// </summary>
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        /// <summary>
        /// Begins the Http request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpTask GetAsync(string url)
        {
            var state = new HttpTask
            {
                Status = TaskStatus.Running,
            };

            TaskManager.StartRoutine(RunAsync(state, url));

            return state;
        }

        IEnumerator RunAsync(HttpTask task, string url)
        {
            var www = new WWW(url);

            yield return www;

            task.StatusCode = GetCode(www);

            if (!string.IsNullOrEmpty(www.error))
            {
                task.Exception = new Exception(www.error);
                task.Status = TaskStatus.Faulted;
            }
            else
            {
                task.Content = www.text;
                task.Status = TaskStatus.Success;
            }
        }


        /// <summary>
        /// Begins the Http request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpTask<T> GetAsync<T>(string url)
        {
            var state = new HttpTask<T>
            {
                Status = TaskStatus.Running,
            };

            TaskManager.StartRoutine(RunAsync(state, url));

            return state;
        }

        IEnumerator RunAsync<T>(HttpTask<T> task, string url)
        {
            var www = new WWW(url);

            yield return www;

            task.StatusCode = GetCode(www);

            if (!string.IsNullOrEmpty(www.error))
            {
                task.Exception = new Exception(www.error);
                task.Status = TaskStatus.Faulted;
            }
            else
            {
                task.Content = www.text;
                task.Status = TaskStatus.Success;
                task.Result = JsonMapper.ToObject<T>(www.text);
            }

        }

        /// <summary>
        /// Begins the Http request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpTask PostAsync(string url)
        {
            var state = new HttpTask
            {
                Status = TaskStatus.Running,
            };

            TaskManager.StartRoutine(PostAsync(state, url, null));

            return state;
        }

        /// <summary>
        /// Begins the Http request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public HttpTask PostAsync(string url, string content)
        {
            var state = new HttpTask
            {
                Status = TaskStatus.Running,
            };

            TaskManager.StartRoutine(PostAsync(state, url, content));

            return state;
        }

        IEnumerator PostAsync(HttpTask task, string url, string content)
        {
            if (!Headers.ContainsKey("ACCEPT"))
                Headers.Add("ACCEPT", Accept);
            if (!Headers.ContainsKey("CONTENT-TYPE"))
                Headers.Add("CONTENT-TYPE", ContentType);
            
            WWW www;
            try
            {
                www = new WWW(url, content == null ? new byte[1] : Encoding.UTF8.GetBytes(content), Headers);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                task.Exception = ex;
                task.Status = TaskStatus.Faulted;
                yield break;
            }


            yield return www;

            task.StatusCode = GetCode(www);
            
            if (!string.IsNullOrEmpty(www.error))
            {
                task.Exception = new Exception(www.error);
                task.Status = TaskStatus.Faulted;
            }
            else
            {
                task.Content = www.text;
                task.Status = TaskStatus.Success;
            }

        }

        /// <summary>
        /// Begins the Http request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpTask<T> PostAsync<T>(string url) where T : class
        {
            var state = new HttpTask<T>
            {
                Status = TaskStatus.Running,
            };

            TaskManager.StartRoutine(PostAsync(state, url, null));

            return state;
        }

        /// <summary>
        /// Begins the Http request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public HttpTask<T> PostAsync<T>(string url, string content) where T : class
        {
            var state = new HttpTask<T>
            {
                Status = TaskStatus.Running,
            };

            TaskManager.StartRoutine(PostAsync(state, url, content));

            return state;
        }

        IEnumerator PostAsync<T>(HttpTask<T> task, string url, string content) where T : class
        {
            if (!Headers.ContainsKey("ACCEPT"))
                Headers.Add("ACCEPT", Accept);
            if (!Headers.ContainsKey("CONTENT-TYPE"))
                Headers.Add("CONTENT-TYPE", ContentType);

            //           Debug.Log("POSTAsync : " + url);
            //            Debug.Log(content);

            WWW www;
            try
            {
                www = new WWW(url, content == null ? new byte[1] : Encoding.UTF8.GetBytes(content), Headers);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                task.Exception = ex;
                task.Status = TaskStatus.Faulted;
                yield break;
            }


            yield return www;

            task.StatusCode = GetCode(www);

           
            if (!string.IsNullOrEmpty(www.error))
            {
                task.Exception = new Exception(www.error);
                task.Status = TaskStatus.Faulted;
            }
            else
            {
                task.Content = www.text;
                task.Result = string.IsNullOrEmpty(task.Content) ? null : JsonMapper.ToObject<T>(task.Content);
                task.Status = TaskStatus.Success;
            }

        }

        /// <summary>
        /// Parses the HTTPStatus Code from the header status
        /// </summary>
        /// <param name="www"></param>
        /// <returns></returns>
        HttpStatusCode GetCode(WWW www)
        {
            if (!www.responseHeaders.ContainsKey("STATUS"))
            {
                return 0;
            }

            var code = www.responseHeaders["STATUS"].Split(' ')[1];
            return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), code);
        }

    }
}