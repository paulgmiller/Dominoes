using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Text;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Dominoes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameTable : Page
    {
        public GameTable()
        {
            this.InitializeComponent();
            
            Table.KeyDown += GameTable_KeyDown;
            Log.KeyDown += GameTable_KeyDown;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Global.Logger = await TextBlockLogger.Create(Log, ScrollLog);
            Game.NewGame(asciipaint => Table.Text = asciipaint);

            Table.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            while (true)
            {
                var game = Game.Instance();
                bool killed = await game.Play();
                if (!killed)
                {
                    //should await a keypress instead of 10 seconds
                    await Task.Delay(10000);
                    Game.NewGame(asciipaint => Table.Text = asciipaint);
                }
            }
            
        }

        public class GameEntity : ITableEntity
        {
            public Game GameState { get; private set; }

            public GameEntity(Game g)
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

        }

        async void GameTable_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            Game.Instance().Input(e.Key);
            
            if (e.Key == Windows.System.VirtualKey.S)
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                //wipe the file?

                var file = await localFolder.CreateFileAsync("dominoes.json", CreationCollisionOption.ReplaceExisting);
                Game.Instance().Save(file.OpenStreamForWriteAsync().Result);
                Global.Logger.Comment("Saved game to " + file.Path);

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=dominoes;AccountKey=ehWYwN/r31V98QHRywp10U36U9bcsa6j7xBH6CnC/Mjg0QlCZfp3JEP1+tGAN1bDghOZ7VnQRtZeFhyOwB9qVA==");
                var tableClient = storageAccount.CreateCloudTableClient();

                
              
                try
                {
                    CloudTable gamesTable = tableClient.GetTableReference("TrialGames");
                    if (await gamesTable.CreateIfNotExistsAsync())
                        Global.Logger.Comment("created table:" + gamesTable.Name);

                    TableOperation insertOperation = TableOperation.Insert(new GameEntity(Game.Instance()));
                    TableResult res = await gamesTable.ExecuteAsync(insertOperation);

                    Global.Logger.Comment("Saved game to " + gamesTable.Uri + " " + res.ToString());
                }
                catch (Exception error)
                {
                    Global.Logger.Comment("Failed to save " + error.ToString());
                }
            }
            if (e.Key == Windows.System.VirtualKey.L)
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.GetFileAsync("dominoes.json");
                var game = Game.Load(file.OpenStreamForReadAsync().Result, asciipaint => Table.Text = asciipaint);
                Game.Swap(game);
                Global.Logger.Comment("Loaded game from  " + file.Path);
            }
        }
    }
}
