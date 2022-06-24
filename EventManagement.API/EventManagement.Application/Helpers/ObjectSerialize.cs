using System.Text;
using Newtonsoft.Json;

namespace EventManagement.Application.Helpers
{
    public static class ObjectSerialize
    {
        public static byte[] Serailize<T>(this T obj)
        {
            if (obj == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T Deserialize<T>(this byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
