using CommonTestClasses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using NewDriver.Serializer.Fixes;
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
    public class DictionarySerializationTest
    {
        IMongoDatabase _db;
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
      
            BsonSerializer.RegisterSerializer(typeof(GroupId), new TypedEventStoreIdentityBsonSerializer<GroupId>());
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            _db = client.GetDatabase(url.DatabaseName);
            client.DropDatabase(_db.DatabaseNamespace.DatabaseName);
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
            _collDic.InsertOne(obj);
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
            _collDicSer.InsertOne(obj);
        }


        [Test]
        public void verify_serialization_of_object_with_serializer()
        {
            ObjectWithDictionaryProperties obj = new ObjectWithDictionaryProperties();
            obj.Properties.Add("test1", new GroupId(1));
            obj.Properties.Add("test2", "a string");
            var serialized = obj.ToJson();
            //the very same test passes with old driver, because the old driver uses the concept of
            //nominal type, and when it is time to serialize GroupId new driver does not check if there
            //is a specific serializer.
            Assert.AreEqual("{ \"Properties\" : { \"test1\" : \"Group_1\", \"test2\" : \"a string\" } }", serialized);
        }

        [Test]
        public void verify_serialization_of_object_with_serializer_fixed()
        {
            FixedObjectWithDictionaryProperties obj = new FixedObjectWithDictionaryProperties();
            obj.Properties.Add("test1", new GroupId(1));
            obj.Properties.Add("test2", "a string");
            var serialized = obj.ToJson();
            //the very same test passes with old driver, because the old driver uses the concept of
            //nominal type, and when it is time to serialize GroupId new driver does not check if there
            //is a specific serializer.
            Assert.AreEqual("{ \"Properties\" : { \"test1\" : \"Group_1\", \"test2\" : \"a string\" } }", serialized);
        }

        #region Auxiliary classes

        public class FixedObjectWithDictionaryProperties
        {
            public FixedObjectWithDictionaryProperties()
            {
                Properties = new Dictionary<string, object>();
            }

            [BsonSerializer(typeof(CustomDictionaryAsObjectSerializer<Dictionary<String, Object>, String, Object>))]
            public Dictionary<String, Object> Properties { get; set; }
        }

        #endregion
    }
}
