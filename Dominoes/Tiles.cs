using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes
{

    public class OutOfTiles : Exception { }

    [DataContract(Name = "Tiles", Namespace = "Dominoes")]
    public class Tiles
    {
        //this should just be a queue but a queue serializes very stragely so we're simplifying to list.
        [DataMember()]
        private List<Domino> _tiles;

        private static readonly Random rnd = new Random();

        public Tiles()
        {
            var dotrange = Enumerable.Range(Domino.MIN_DOTS, Domino.MAX_DOTS+1);
            var initial = dotrange.SelectMany(f => Enumerable.Range(Domino.MIN_DOTS, f+1).Select(s => new Domino(f, s)));
            var shuffled = initial.OrderBy(d => rnd.Next());
            _tiles = new List<Domino>(shuffled);
            Global.Logger.Debug(string.Format("Shuffled {0} dominoes", _tiles.Count));
            
        }

        public bool Empty()
        {
            return !_tiles.Any();
        }

        public Domino Next()
        {
            if (_tiles.Count == 0) 
                throw new OutOfTiles();
            var tile = _tiles.First();
            _tiles.RemoveAt(0);
            return tile;
        }

    }
}
