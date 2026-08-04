using System.Text.Json;

namespace BookReview
{
    public static class SessionExtensions
    {
        // Obyekti JSON-a çevirib Session-da saxlayır
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Session-dan JSON-u oxuyub obyektə çevirir
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
