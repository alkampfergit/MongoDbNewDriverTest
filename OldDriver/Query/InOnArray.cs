using CommonTestClasses;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldDriver.Query
{
    [TestFixture]
    class InOnArray
    {
        MongoCollection<Post> _collection;

        [SetUp]
        public void SetUp()
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["base"].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetServer().GetDatabase(url.DatabaseName);
            db.Drop();
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
            _collection.Insert(post1);
            Post post2 = new Post()
            {
                Id = "post2",
                Title = "This is post 2",
                Tags = new String[] { "Tag2" }
            };
            _collection.Insert(post2);

            var result = _collection.Find(
                Query<Post>.In(p => p.Tags, new[] { "Tag1" }))
                .ToList();
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo("post1"));
        }
    }
}
