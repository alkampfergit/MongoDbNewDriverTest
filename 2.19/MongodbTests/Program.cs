using Elsa.Models;
using Elsa.Persistence.MongoDb.Serializers;
using Elsa.Persistence.MongoDb.Stores;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.WorkflowDefinitions;
using Elsa.Services;
using MongoDB.Driver;

namespace MongodbTests
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            BsonSerializer.RegisterSerializer(new ObjectSerializer());
            var mongoUrl = new MongoUrl("mongodb://localhost:20017/test");

            var settings = MongoClientSettings.FromUrl(mongoUrl);

            //Uncommenting the following line will make the code NOT throwing
            //Exception demonstrating that the new LINQ3 provider introduces a regression.
            //settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V2;

            var client = new MongoClient(settings);
            var db = client.GetDatabase("test");

            var collection = db.GetCollection<WorkflowDefinition>("WorkflowDefinition");

            client.DropDatabase("test");

            var store = new MongoDbWorkflowDefinitionStore(collection, new IdGenerator());

            var specification = new VersionOptionsSpecification(VersionOptions.LatestOrPublished);
            var finalSpec = specification.And(new TenantSpecification<WorkflowDefinition>(null));
            var count = await store.CountAsync(finalSpec, CancellationToken.None);
            Console.WriteLine(count.ToString());

            Console.WriteLine("Press a key to continue");
            Console.ReadKey();
        }
    }
}