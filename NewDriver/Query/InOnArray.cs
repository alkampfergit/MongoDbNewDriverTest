using CommonTestClasses;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDriver.Query
{
    [TestFixture]
    class InOnArray
    {
        IMongoCollection<Post> _collection;

        [SetUp]
        public void SetUp()
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            client.DropDatabase(db.DatabaseNamespace.DatabaseName);
            _collection = db.GetCollection<Post>("Post");

        }

        /// <summary>
        /// This test failed, because any does not work as expectd on old driver, in new driver it correctly
        /// returns error 
        /// </summary>
        [Test]
        public void verify_in_query()
        {
            Post post1 = new Post()
            {
                Id = "post1",
                Title = "This is post 1",
                Tags = new String[] { "Tag1", "Tag2" }
            };
            _collection.InsertOne(post1);
            Post post2 = new Post()
            {
                Id = "post2",
                Title = "This is post 2",
                Tags = new String[] { "Tag2" }
            };
            _collection.InsertOne(post2);

            //this does not compile anymore
            //var result = _collection.Find(
            //    Builders<Post>.Filter.In(p => p.Tags, new[] { "Tag1" }))
            //    .ToList();

            var result = _collection.Find(
               Builders<Post>.Filter.In("Tags", new[] { "Tag1" }))
               .ToList();
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo("post1"));
        }

        [Test]
        public void verify_in_query_correct_way()
        {
            Post post1 = new Post()
            {
                Id = "post1",
                Title = "This is post 1",
                Tags = new String[] { "Tag1", "Tag2" }
            };
            _collection.InsertOne(post1);
            Post post2 = new Post()
            {
                Id = "post2",
                Title = "This is post 2",
                Tags = new String[] { "Tag2" }
            };
            _collection.InsertOne(post2);

            //this does not compile anymore
            //var result = _collection.Find(
            //    Builders<Post>.Filter.In(p => p.Tags, new[] { "Tag1" }))
            //    .ToList();

            var result = _collection.Find(
               Builders<Post>.Filter.AnyIn(p => p.Tags, new[] { "Tag1" }))
               .ToList();
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo("post1"));
        }
    }
}
