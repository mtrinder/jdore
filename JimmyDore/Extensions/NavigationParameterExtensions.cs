using System;
using Newtonsoft.Json;
using Prism.Navigation;

namespace JimmyDore.Extensions
{
    internal static class NavigationParameterExtensions
    {
        internal static bool IsBackNavigation(this INavigationParameters source) =>
            source.GetNavigationMode() == NavigationMode.Back;
        internal static bool IsNewNavigation(this INavigationParameters source) =>
            source.GetNavigationMode() == NavigationMode.New;

        internal static INavigationParameters Add<T>(this INavigationParameters source, NavigationParameterKeys key, T value)
        {
            if (value != null)
            {
                source.Add(key.ToString("G"), value);
                return source;

            }

            return source;
        }

        internal static T Get<T>(this INavigationParameters source, NavigationParameterKeys key)
        {
            try
            {
                if (source.ContainsKey(key.ToString("G")))
                {
                    return source.GetValue<T>(key.ToString("G"));
                }
            }
            catch (Exception e)
            {
                // Ignore errors
                System.Diagnostics.Debug.WriteLine(e);
            }
            return default(T);
        }

        internal static bool ContainsKey(this INavigationParameters source, NavigationParameterKeys key)
        {
            return source.ContainsKey(key.ToString("G"));
        }

        internal static INavigationParameters SerialiseAndAdd<T>(this INavigationParameters source, NavigationParameterKeys key,
            T value)
        {
            if (value != null)
            {
                var serialisedValue = JsonConvert.SerializeObject(value);

                return SetStringValue(source, key.ToString("G"), serialisedValue);
            }

            return source;
        }

        internal static T DeserialiseAndGet<T>(this INavigationParameters source, NavigationParameterKeys key)
        {
            var serialisedValue = GetStringValue(source, key.ToString("G"));

            try
            {
                if (serialisedValue != null) return JsonConvert.DeserializeObject<T>(serialisedValue);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            return default(T);
        }

        internal static INavigationParameters SetStringValue(INavigationParameters source, string key, string value)
        {
            source.Add(key, value);
            return source;
        }

        internal static string GetStringValue(INavigationParameters source, string key)
        {
            if (source.ContainsKey(key))
            {
                return source.GetValue<string>(key);
            }
            return string.Empty;
        }
    }

    internal enum NavigationParameterKeys
    {
        VideoId,
        PodcastLink
    }

}
