using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;


namespace Dominoes
{
    class TextBlockLogger : ILogger
    {
        private TextBlock _block;
        private ScrollViewer _scroll;

        public TextBlockLogger(TextBlock tb, ScrollViewer sv)
        {
            _block = tb;
            _scroll = sv;

        }

        public void Comment(string statement)
        {
            _block.Text += "\n";            
            _block.Text += statement;
            _scroll.ScrollToVerticalOffset(_block.ActualHeight);
        }

        public void Debug(string statement)
        {
            //do nothing right now
        }

        public void Error(string statement)
        {
            throw new NotImplementedException();
        }
    }
}
