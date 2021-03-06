﻿using Newtonsoft.Json;
using System;

namespace DeepLearningDataProvider.JsonConverters
{
    // redundant ?
    public class GenericJsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<T>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.Serialize(writer, value);
        }
    }
}
