using CommonTestClasses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDriver.Serializer
{
    public class EventStoreIdentityBsonSerializer : SerializerBase<EventStoreIdentity>
    {

        public override EventStoreIdentity Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            var id = context.Reader.ReadString();

            return IdentityHelper.Parse(id);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, EventStoreIdentity value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(value.AsString());
            }
        }
    }

    public class TypedEventStoreIdentityBsonSerializer<T> : SerializerBase<T> where T : EventStoreIdentity
    {

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            var id = context.Reader.ReadString();

            return (T) IdentityHelper.Parse(id);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(value.AsString());
            }
        }
    }
}
