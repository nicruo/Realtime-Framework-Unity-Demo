// -------------------------------------
//  Domain		: IBT / Realtime.co
//  Author		: Nicholas Ventimiglia
//  Product		: Messaging and Storage
//  Published	: 2014
//  -------------------------------------
using System;
using System.Collections;

namespace Realtime.Tasks
{

    /// <summary>
    /// System.Threading.Tasks.Task implementation
    /// </summary>
    public partial class Task
    {
        #region Task
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task Run(Action action)
        {
            var task = new Task(action);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnMain(Action action)
        {
            var task = new Task(action, TaskStrategy.MainThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnCurrent(Action action)
        {
            var task = new Task(action, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task Run<TP>(Action<TP> action, TP param)
        {
            var task = new Task(action, param, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnMain<TP>(Action<TP> action, TP param)
        {
            var task = new Task(action, param, TaskStrategy.MainThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnCurrent<TP>(Action<TP> action, TP param)
        {
            var task = new Task(action, param, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        #endregion

        #region Coroutine

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunCoroutine(IEnumerator function)
        {
            var task = new Task(function);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunCoroutine(Func<IEnumerator> function)
        {
            var task = new Task(function());
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunCoroutine(Func<Task, IEnumerator> function)
        {
            var task = new Task();
            task.Strategy = TaskStrategy.Coroutine;
            task._routine = function(task);
            task.Start();
            return task;
        }
        #endregion

        #region Task With Result
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var task = new Task<TResult>(function);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> Run<TParam, TResult>(Func<TParam, TResult> function, TParam param)
        {
            var task = new Task<TResult>(function, param);
            task.Start();
            return task;
        }
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnMain<TResult>(Func<TResult> function)
        {
            var task = new Task<TResult>(function, TaskStrategy.MainThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnMain<TParam, TResult>(Func<TParam, TResult> function, TParam param)
        {
            var task = new Task<TResult>(function, param, TaskStrategy.MainThread);
            task.Start();
            return task;
        } 
        
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnCurrent<TResult>(Func<TResult> function)
        {
            var task = new Task<TResult>(function, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnCurrent<TParam, TResult>(Func<TParam, TResult> function, TParam param)
        {
            var task = new Task<TResult>(function, param, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunCoroutine<TResult>(IEnumerator function)
        {
            var task = new Task<TResult>(function);
            task.Start();
            return task;
        }


        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunCoroutine<TResult>(Func<IEnumerator> function)
        {
            var task = new Task<TResult>(function());
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a task which passes the task as a parameter
        /// </summary>
        public static Task<TResult> RunCoroutine<TResult>(Func<Task<TResult>, IEnumerator> function)
        {
            var task = new Task<TResult>();
            task.Strategy = TaskStrategy.Coroutine;
            task.Paramater = task;
            task._routine = function(task);
            task.Start();
            return task;
        }
        #endregion

    }
}
