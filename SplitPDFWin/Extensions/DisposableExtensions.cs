using System;
using System.Collections.Generic;

namespace SplitPDFWin.Extensions
{
    public static class DisposableExtensions
    {
        private static readonly Dictionary<object, List<IDisposable>> disposableElements = [];

        public static void RegisterSubscription(this IDisposable disposable, object key)
        {
            if (!disposableElements.TryGetValue(key, out var disposablesByKey))
            {
                disposablesByKey = [];
                disposableElements[key] = disposablesByKey;
            }

            disposablesByKey.Add(disposable);
        }

        public static void Unsubscribe(this object key)
        {
            if (disposableElements.TryGetValue(key, out var disposablesByKey))
            {
                foreach (var disposable in disposablesByKey)
                {
                    disposable.Dispose();
                }
                disposableElements.Remove(key);
            }
        }
    }
}
