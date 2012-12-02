using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    class Tiles
    {
        private Queue<Domino> _tiles;
        static readonly Random rnd = new Random();

        public Tiles()
        {
            var dotrange = Enumerable.Range(Domino.MIN_DOTS, Domino.MAX_DOTS);
            var initial = dotrange.SelectMany(f => Enumerable.Range(Domino.MIN_DOTS, f).Select(s => new Domino(f, s)));
            var shuffled = initial.OrderBy(d => rnd.Next());
            _tiles = new Queue<Domino>(shuffled);
            
        }

        public bool Empty()
        {
            return !_tiles.Any();
        }

        public Domino Next()
        {
            return _tiles.Dequeue();
        }

    }
}
