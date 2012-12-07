using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    public interface ILogger
    {
        void Comment(string statement); 
        void Debug(string statement);
        void Error(string statement);
    }

    public class Global
    {
        public static ILogger Logger { get; internal set;  }
    }
}
