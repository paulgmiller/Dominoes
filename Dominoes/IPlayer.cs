using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes
{
    public interface IPlayer
    {
        string Name();
        bool Open {  get; }
        Task<bool> Play(GameGraph game, Tiles tiles);
        bool Start(int startVale, GameGraph graph); 
        void Draw(Tiles tiles);
    }
}
