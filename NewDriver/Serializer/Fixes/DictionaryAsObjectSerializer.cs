using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDriver.Serializer.Fixes
{
    public class CustomDictionaryAsObjectSerializer<TDictionary, TKey, TValue> : DictionarySerializerBase<TDictionary, TKey, TValue>
          where TDictionary : class, IDictionary<TKey, TValue>
    {
        private static IBsonSerializer<TKey> GetKeySerializer()
        {
            return BsonSerializer.LookupSerializer<TKey>();
        }

        private static IBsonSerializer<TValue> GetValueSerializer()
        {
            return new ValueClassSerializer();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryInterfaceImplementerSerializer{TDictionary, TKey, TValue}"/> class.
        /// </summary>
        public CustomDictionaryAsObjectSerializer()
            : base(
                    MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document,
                    GetKeySerializer(),
                    GetValueSerializer())
        {

        }

        protected override TDictionary CreateInstance()
        {
            return Activator.CreateInstance<TDictionary>();
        }

        private class ValueClassSerializer : IBsonSerializer<TValue>
        {
            IBsonSerializer<TValue> _base;

            public ValueClassSerializer()
            {
                _base = BsonSerializer.LookupSerializer<TValue>();
            }
            public Type ValueType
            {
                get
                {
                    return typeof(TValue);
                }
            }

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            {
                IBsonSerializer specificSerializer = GetSpecificSerializer(value.GetType());
                if (specificSerializer != null)
                {
                    specificSerializer.Serialize(context, args, value);
                }
                else
                {
                    _base.Serialize(context, args, value);
                }
            }

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value)
            {
                IBsonSerializer specificSerializer = GetSpecificSerializer(value.GetType());
                if (specificSerializer != null)
                {
                    specificSerializer.Serialize(context, args, value);
                }
                else
                {
                    _base.Serialize(context, args, value);
                }
            }

            private static ConcurrentDictionary<Type, IBsonSerializer> _serializers = new ConcurrentDictionary<Type, IBsonSerializer>();

            private static IBsonSerializer GetSpecificSerializer(Type value)
            {
                IBsonSerializer serializer;
                if (_serializers.TryGetValue(value, out serializer))
                    return serializer;

                serializer = BsonSerializer.LookupSerializer(value);
                _serializers[value] = serializer;
                return serializer;
            }

            public TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                return _base.Deserialize(context, args);
            }

            object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                return _base.Deserialize(context, args);
            }
        }
    }
}
