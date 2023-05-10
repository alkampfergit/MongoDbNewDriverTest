using CommonTestClasses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;


namespace OldDriver.Serializer
{
    [TestFixture]
    public class AnyOnDictionaryTests
    {
        MongoCollection<ClassWithMetadata> _collection;

        [SetUp]
        public void SetUp()
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetServer().GetDatabase(url.DatabaseName);
            db.Drop();
            _collection = db.GetCollection<ClassWithMetadata>("ObjectWithArrayOfIdentities");
           
        }

        /// <summary>
        /// This test failed, because any does not work as expectd on old driver, in new driver it correctly
        /// returns error 
        /// </summary>
        [Test]
        public void verify_where_query()
        {
            ClassWithMetadata test1 = new ClassWithMetadata() { Id = "1", Metadata = new Dictionary<string, Metadata>() };
            ClassWithMetadata test2 = new ClassWithMetadata() { Id = "2", Metadata = new Dictionary<string, Metadata>() };

            test2.Metadata.Add("doc1", new Metadata() { Key = "a", Value = "b" });

            _collection.Insert(test1);
            _collection.Insert(test2);

            var result = _collection.Find(Query<ClassWithMetadata>.Where(c => c.Metadata.Any())).ToList();
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }
}
