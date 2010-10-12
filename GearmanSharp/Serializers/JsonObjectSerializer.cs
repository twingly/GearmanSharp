using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twingly.GearmanApi.Serializers
{
    /// <summary>
    /// Serializes/deserializes to/from JSON using Json.NET.
    /// The JSON string is the serialized to a byte[] using UTF-8 encoding.
    /// </summary>
    public class JsonSerializer<T> : IDataSerializer<T> where T : class
    {
        public byte[] Serialize(T data)
        {
            if (data == null)
                return null;

            var jsonStr = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(jsonStr);
        }

        public T Deserialize(byte[] data)
        {
            if (data == null)
                return null;

            var jsonStr = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}