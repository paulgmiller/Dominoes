using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Dominoes
{
    class TextBlockLogger : ILogger
    {
        private TextBlock _block;
        private ScrollViewer _scroll;
        private StorageFile _file;

        private TextBlockLogger(TextBlock tb, ScrollViewer sv)
        {
            _block = tb;
            _scroll = sv;
        }

        public static async Task<TextBlockLogger> Create(TextBlock tb, ScrollViewer sv)
        {
            var logger = new TextBlockLogger(tb, sv);
            var local = ApplicationData.Current.LocalFolder;
            var filename = string.Format("game_{0}.log", DateTime.Now.ToFileTime());
            logger._file = await local.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            logger._block.Text = "log location: " + logger._file.Path;
            return logger;
        }

        public void Comment(string statement)
        {
            _block.Text = statement + "\n" + _block.Text;
            //why o why can't I use scrollToBottom
            _scroll.ScrollToVerticalOffset(0);
            //this seems strange
            Windows.Storage.FileIO.AppendTextAsync(_file, string.Format("COMMENT:{0}\r\n",statement));
        }

        public void Debug(string statement)
        {
            Windows.Storage.FileIO.AppendTextAsync(_file, string.Format("DEBUG:{0}\r\n", statement));
        }

        public void Error(string statement)
        {
            throw new NotImplementedException();
        }
    }
}
