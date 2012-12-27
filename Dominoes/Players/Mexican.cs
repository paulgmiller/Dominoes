using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes.Players
{
    [DataContract]
    class Mexican : IPlayer
    {
        public string Name()
        {
 	        return "Mexi";
        }

        public override string ToString()
        {
            return Name();
        }

        public bool Open 
        {
            get { return true; }
        }

        public async Task<bool> Play(GameGraph game, Tiles tiles)
        {
 	        //this is a really silly way to just say return false;
            return await Task.Factory.StartNew(() => false);
        }

        public bool Start(int startValue, GameGraph game)
        {
            return false;
        }

        public void Draw(Tiles t)  {}
    }
}
