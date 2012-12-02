using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    interface IPlayer
    {
        string Name();
        bool IsOpen();
        bool Play();
    }
}
