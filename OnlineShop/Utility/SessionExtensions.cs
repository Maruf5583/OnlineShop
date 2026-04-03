using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace OnlineShop.Utility
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }

        // Convenience aliases matching usage in controllers
        public static void Set<T>(this ISession session, string key, T value)
            => SetObject(session, key, value);

        public static T? Get<T>(this ISession session, string key)
            => GetObject<T>(session, key);
    }
}
