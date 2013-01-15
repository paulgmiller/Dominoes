using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Dominoes
{
    public class AzureGameEntity : ITableEntity
    {
        public Game GameState { get; private set; }

        public AzureGameEntity(Game g)
        {
            GameState = g;
        }

        public string ETag { get; set; }
        public string PartitionKey { get { return GameState.Id.ToString(); } set { } }
        public string RowKey { get { return GameState.Id.ToString(); } set { } }
        public DateTimeOffset Timestamp { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(properties["state"].StringValue);
            MemoryStream stream = new MemoryStream(byteArray);
            GameState = Game.Load(stream, null); //need to fix this.

        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            MemoryStream m = new MemoryStream();
            GameState.Save(m);
            byte[] json = m.ToArray();
            string state = Encoding.UTF8.GetString(json, 0, json.Length);
            var dict = new Dictionary<string, EntityProperty>();
            dict["state"] = new EntityProperty(state);
            //entity property should be able to take a guid.
            dict["id"] = new EntityProperty(GameState.Id.ToString());
            return dict;
        }

        public static async void Upload(Game g)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=dominoes;AccountKey=ehWYwN/r31V98QHRywp10U36U9bcsa6j7xBH6CnC/Mjg0QlCZfp3JEP1+tGAN1bDghOZ7VnQRtZeFhyOwB9qVA==");
            var tableClient = storageAccount.CreateCloudTableClient();

            try
            {
                CloudTable gamesTable = tableClient.GetTableReference("TrialGames");
                if (await gamesTable.CreateIfNotExistsAsync())
                    Global.Logger.Comment("created table:" + gamesTable.Name);

                TableOperation insertOperation = TableOperation.Insert(new AzureGameEntity(g));
                TableResult res = await gamesTable.ExecuteAsync(insertOperation);

                Global.Logger.Comment("Saved game to " + gamesTable.Uri + " " + res.ToString());
            }
            catch (Exception error)
            {
                Global.Logger.Comment("Failed to save " + error.ToString());
            }
        }
    }
}
