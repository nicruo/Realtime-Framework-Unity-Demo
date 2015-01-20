using System;

namespace Realtime.Messaging.Bridge
{
    internal class MonoPInvokeCallbackAttribute : Attribute
    {
        // ReSharper disable once InconsistentNaming
        public Type type;

        public MonoPInvokeCallbackAttribute(Type t)
        {
            type = t;
        }
    }
}