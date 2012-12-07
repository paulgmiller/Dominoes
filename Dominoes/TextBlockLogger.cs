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

        public TextBlockLogger(TextBlock tb)
        {
            _block = tb;
        }

        public void Comment(string statement)
        {
            _block.Text += "\n";            
            _block.Text += statement;
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
