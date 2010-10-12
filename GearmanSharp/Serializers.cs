using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Twingly.Gearman
{
    public delegate byte[] DataSerializer<T>(T data) where T : class;
    public delegate T DataDeserializer<T>(byte[] data) where T : class;

    public static class Serializers
    {
        public static byte[] UTF8StringSerialize(string data)
        {
            if (data == null)
                return null;

            return Encoding.UTF8.GetBytes(data);
        }

        public static string UTF8StringDeserialize(byte[] data)
        {
            if (data == null)
                return null;

            return Encoding.UTF8.GetString(data);
        }

        public static byte[] JsonSerialize<T>(T data) where T : class
        {
            if (data == null)
                return null;

            var jsonStr = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(jsonStr);
        }

        public static T JsonDeserialize<T>(byte[] data) where T : class
        {
            if (data == null)
                return null;

            var jsonStr = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}
