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


namespace NewDriver.Serializer
{
    [TestFixture]
    public class AnyOnDictionaryTests
    {
        IMongoCollection<ClassWithMetadata> _collection;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            client.DropDatabase(db.DatabaseNamespace.DatabaseName);
            _collection = db.GetCollection<ClassWithMetadata>("ObjectWithArrayOfIdentities");
           
        }

        [Test]
        public void verify_where_query()
        {
            ClassWithMetadata test1 = new ClassWithMetadata() { Id = "1", Metadata = new Dictionary<string, Metadata>() };
            ClassWithMetadata test2 = new ClassWithMetadata() { Id = "2", Metadata = new Dictionary<string, Metadata>() };

            test2.Metadata.Add("doc1", new Metadata() { Key = "a", Value = "b" });

            _collection.InsertOne(test1);
            _collection.InsertOne(test2);

            var result = _collection.Find(Builders<ClassWithMetadata>.Filter.Where(c => c.Metadata.Any())).ToList();
            Assert.That(result, Has.Count.EqualTo(1));
        }

      
    }
}
