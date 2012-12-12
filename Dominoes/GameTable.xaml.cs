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
            Global.Logger = new TextBlockLogger(Log, ScrollLog);
            Game.NewGame(asciipaint => Table.Text = asciipaint);
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
            Table.Focus(Windows.UI.Xaml.FocusState.Programmatic);
           
            while (true)
            {
                await Game.Instance().Play();
                await Task.Delay(10000);
                Game.NewGame(asciipaint => Table.Text = asciipaint);
            }
            
        }

        void GameTable_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            Game.Instance().Input(e.Key);
        }
    }
}
