using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.Players
{
    class Mexican : IPlayer
    {
        public string Name()
        {
 	        return "Mexican";
        }

        public bool IsOpen()
        {
 	        return true;
        }

        public bool Play(GameGraph game)
        {
 	        return false;
        }

        public bool Start(int startValue, GameGraph game)
        {
            return false;
        }

        public void Draw()  {}
    }
}
