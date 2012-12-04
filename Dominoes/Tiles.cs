using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    public class Tiles
    {
        private Queue<Domino> _tiles;
        private static readonly Random rnd = new Random();

        public Tiles()
        {
            var dotrange = Enumerable.Range(Domino.MIN_DOTS, Domino.MAX_DOTS+1);
            var initial = dotrange.SelectMany(f => Enumerable.Range(Domino.MIN_DOTS, f+1).Select(s => new Domino(f, s)));
            var shuffled = initial.OrderBy(d => rnd.Next());
            _tiles = new Queue<Domino>(shuffled);
            Global.Logger.LogDebug(string.Format("Shuffled {0} dominoes", _tiles.Count));
            
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
