using CommonTestClasses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
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
    public class DictionarySerializationTest
    {
        MongoDatabase _db;

        public class ObjectWithDictionary
        {
            public String Id { get; set; }

            public Dictionary<GroupId, String> Dictionary { get; set; }
        }

        public class ObjectWithDictionarySerialization
        {
            public String Id { get; set; }

            [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
            public Dictionary<GroupId, String> Dictionary { get; set; }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
      
            BsonSerializer.RegisterSerializer(typeof(GroupId), new EventStoreIdentityBsonSerializerOld());
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            _db = client.GetServer().GetDatabase(url.DatabaseName);
            _db.Drop();
        }

        [Test]
        public void verify_serialization()
        {
            var _collDic = _db.GetCollection<ObjectWithDictionary>("ObjectWithDictionary");
            ObjectWithDictionary obj = new ObjectWithDictionary()
            {
                Id = Guid.NewGuid().ToString(),
                Dictionary = new Dictionary<GroupId, string>()
            };
            obj.Dictionary.Add(new GroupId(1), "Test");
            _collDic.Insert(obj);
        }

        [Test]
        public void verify_serialization_array_of_object()
        {
            var _collDicSer = _db.GetCollection<ObjectWithDictionarySerialization>("ObjectWithDictionarySerialization");
            ObjectWithDictionarySerialization obj = new ObjectWithDictionarySerialization()
            {
                Id = Guid.NewGuid().ToString(),
                Dictionary = new Dictionary<GroupId, string>()
            };
            obj.Dictionary.Add(new GroupId(1), "Test");
            _collDicSer.Insert(obj);
        }


        [Test]
        public void verify_serialization_of_object_with_serializer()
        {
            ObjectWithDictionaryProperties obj = new ObjectWithDictionaryProperties();
            obj.Properties.Add("test1", new GroupId(1));
            obj.Properties.Add("test2", "a string");
            var serialized = obj.ToJson();
            //the very same test passes with old driver, because the old driver uses the concept of
            //nominal type, and when it is time to serializer
            Assert.AreEqual("{ \"Properties\" : { \"test1\" : \"Group_1\", \"test2\" : \"a string\" } }", serialized);
        }
    }
}
