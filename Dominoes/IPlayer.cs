using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    public interface IPlayer
    {
        string Name();
        bool IsOpen();
        bool Play(GameGraph game);
        bool Start(int startVale, GameGraph graph); 
        void Draw();
    }
}
