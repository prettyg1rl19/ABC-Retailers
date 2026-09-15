using System.Text.Json;

namespace ST10438767_CLDV6212.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

/*
StackOverflow, 2021. Inject SessionExtensions Asp.net Core. [Online]. Available at:
https://stackoverflow.com/questions/60989945/inject-sessionextensions-asp-net-core [Accessed 13 November 2025]
*/

