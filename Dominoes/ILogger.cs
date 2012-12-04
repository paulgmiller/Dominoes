using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    public interface ILogger
    {
        void LogComment(string statement); 
        void LogDebug(string statement);
        void LogError(string statement);
    }

    public class Global
    {
        public static ILogger Logger { get; internal set;  }
    }
}
