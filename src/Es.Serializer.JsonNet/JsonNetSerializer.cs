﻿using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Es.Serializer
{
    public class JsonNetSerializer : SerializerBase
    {
#if DEBUG
        private static readonly Formatting _format = Formatting.Indented;
#else
        private static readonly Formatting _format = Formatting.None;
#endif

        /// <summary>
        /// JsonNetSerializer Instance
        /// </summary>
        public static JsonNetSerializer Instance = new JsonNetSerializer();

        private readonly JsonSerializerSettings _deserializeSetting;
        private readonly JsonSerializerSettings _serializeSetting;

        public JsonNetSerializer() : this(_format)
        {
        }

        public JsonNetSerializer(Formatting format = Formatting.None) : this(new JsonSerializerSettings
        {
            Converters = new JsonConverter[] { new IsoDateTimeConverter() },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = format
        })
        {
        }

        public JsonNetSerializer(JsonSerializerSettings setting) : this(setting, setting)
        {
        }

        public JsonNetSerializer(JsonSerializerSettings serializeSetting, JsonSerializerSettings deserializeSetting)
        {
            _serializeSetting = serializeSetting ?? throw new ArgumentNullException(nameof(serializeSetting));
            _deserializeSetting = deserializeSetting ?? throw new ArgumentNullException(nameof(deserializeSetting));
        }

        public override object Deserialize(TextReader reader, Type type)
        {
            JsonSerializer serializer = JsonSerializer.Create(_deserializeSetting);
            return serializer.Deserialize(reader, type);
        }

        public override object Deserialize(Stream stream, Type type)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding, true, 1024, true))
                return Deserialize(reader, type);
        }

        public override object Deserialize(byte[] data, Type type)
        {
            using (var mem = new MemoryStream(data))
            {
                return Deserialize(mem, type);
            }
        }

        public override object DeserializeFromString(string serializedText, Type type)
        {
            return JsonConvert.DeserializeObject(serializedText, type, _deserializeSetting);
        }

        public override void Serialize(object value, TextWriter writer)
        {
            JsonSerializer serializer = JsonSerializer.Create(_serializeSetting);
            serializer.Serialize(writer, value);
        }

        public override void Serialize(object value, Stream output)
        {
            using (StreamWriter sw = new StreamWriter(output, Encoding, 1024, true))
                Serialize(value, sw);
        }

        public override void Serialize(object value, out byte[] output)
        {
            using (var mem = new MemoryStream())
            {
                Serialize(value, mem);
                output = mem.ToArray();
            }
        }

        public override string SerializeToString(object value)
        {
            return JsonConvert.SerializeObject(value, _serializeSetting);
        }
    }
}