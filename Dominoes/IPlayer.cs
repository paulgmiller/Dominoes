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
        //todo either name should be always unique or we should have a seperate id field.
        string Name();
        bool Open {  get; }
        Task<bool> Play(GameGraph game, Tiles tiles);
        bool Start(int startVale, GameGraph graph); 
        void Draw(Tiles tiles);
    }
}
