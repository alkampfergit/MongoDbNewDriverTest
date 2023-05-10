using CommonTestClasses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldDriver.Serializer
{
    [TestFixture]
    public class EventStoreSerializationTests
    {
        MongoCollection<ObjectWithIdentity> _collObjectWithIdentity;
        MongoCollection<ObjectWithArrayOfIdentities> _collObjectWithArrayOfIdentities;
        MongoCollection<BsonDocument> _collGenericObject;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //BsonClassMap.RegisterClassMap<ObjectWithIdentity>(map =>
            //{
            //    map.AutoMap();
            //    map
            //        .MapProperty(x => x.GroupId)
            //        .SetSerializer(new EventStoreIdentityBsonSerializer());

            //});

            BsonSerializer.RegisterSerializer(typeof(GroupId), new EventStoreIdentityBsonSerializerOld());
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetServer().GetDatabase(url.DatabaseName);
            db.Drop();
            _collObjectWithArrayOfIdentities = db.GetCollection<ObjectWithArrayOfIdentities>("ObjectWithArrayOfIdentities");
            _collObjectWithIdentity = db.GetCollection<ObjectWithIdentity>("ObjectWithIdentity");
            _collGenericObject = db.GetCollection<BsonDocument>("ObjectWithGenericIdentity");
        }

        [Test]
        public void verify_serialization()
        {
            ObjectWithIdentity obj = new ObjectWithIdentity()
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = new GroupId(1),
            };
            _collObjectWithIdentity.Insert(obj);
        }

        [Test]
        public void verify_deserialization()
        {
            ObjectWithIdentity obj = new ObjectWithIdentity()
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = new GroupId(1),
            };
            _collObjectWithIdentity.Insert(obj);
            var deserialized = _collObjectWithIdentity.FindOneById(obj.Id);
        }

        [Test]
        public void verify_serialization_array()
        {
            ObjectWithArrayOfIdentities obj = new ObjectWithArrayOfIdentities()
            {
                Id = Guid.NewGuid().ToString(),
                Groups = new[] { new GroupId(1), new GroupId(2) }
            };
            _collObjectWithArrayOfIdentities.Insert(obj);
        }

        [Test]
        public void verify_deserialization_array()
        {
            ObjectWithArrayOfIdentities obj = new ObjectWithArrayOfIdentities()
            {
                Id = Guid.NewGuid().ToString(),
                Groups = new[] { new GroupId(1), new GroupId(2) }
            };
            _collObjectWithArrayOfIdentities.Insert(obj);
            var deserialized = _collObjectWithArrayOfIdentities.FindOneById(obj.Id);
        }

        [Test]
        public void verify_serialization_generic()
        {
            ObjectWithGenericIdentity<GroupId> obj = new ObjectWithGenericIdentity<GroupId>()
            {
                Id = Guid.NewGuid().ToString(),
                RelatedId = new GroupId(1),
            };
            var document = obj.ToBsonDocument();
            _collGenericObject.Insert(document);
        }

        [Test]
        public void verify_deserialization_generic()
        {
            ObjectWithGenericIdentity<GroupId> obj = new ObjectWithGenericIdentity<GroupId>()
            {
                Id = Guid.NewGuid().ToString(),
                RelatedId = new GroupId(1),
            };
            _collGenericObject.Insert(obj.ToBsonDocument());
            var deserialized = _collGenericObject.FindOneById(obj.Id);
        }


    }
}
