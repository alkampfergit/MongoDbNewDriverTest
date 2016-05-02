using CommonTestClasses;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDriver.Serializer
{
    [TestFixture]
    public class EventStoreSerializationTests
    {
        IMongoCollection<ObjectWithIdentity> _collObjectWithIdentity;
        IMongoCollection<ObjectWithArrayOfIdentities> _collObjectWithArrayOfIdentities;

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

            BsonSerializer.RegisterSerializer(typeof(GroupId), new EventStoreIdentityBsonSerializer());
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            _collObjectWithArrayOfIdentities = db.GetCollection<ObjectWithArrayOfIdentities>("ObjectWithArrayOfIdentities");
            _collObjectWithIdentity = db.GetCollection<ObjectWithIdentity>("ObjectWithIdentity");
        }

        [Test]
        public void verify_serialization()
        {
            ObjectWithIdentity obj = new ObjectWithIdentity()
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = new GroupId(1),
            };
            _collObjectWithIdentity.InsertOne(obj);
        }

        [Test]
        public void verify_deserialization()
        {
            ObjectWithIdentity obj = new ObjectWithIdentity()
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = new GroupId(1),
            };
            _collObjectWithIdentity.InsertOne(obj);
            var deserialized = _collObjectWithIdentity.Find(Builders<ObjectWithIdentity>.Filter.Eq("Id", obj.Id)).SingleOrDefault();
        }

        [Test]
        public void verify_serialization_array()
        {
            ObjectWithArrayOfIdentities obj = new ObjectWithArrayOfIdentities()
            {
                Id = Guid.NewGuid().ToString(),
                Groups = new[] { new GroupId(1), new GroupId(2) }
            };
            _collObjectWithArrayOfIdentities.InsertOne(obj);
        }

        [Test]
        public void verify_deserialization_array()
        {
            ObjectWithArrayOfIdentities obj = new ObjectWithArrayOfIdentities()
            {
                Id = Guid.NewGuid().ToString(),
                Groups = new[] { new GroupId(1), new GroupId(2) }
            };
            _collObjectWithArrayOfIdentities.InsertOne(obj);
            var deserialized = _collObjectWithArrayOfIdentities.Find(Builders<ObjectWithArrayOfIdentities>.Filter.Eq("Id", obj.Id)).SingleOrDefault();
        }
    }
}
