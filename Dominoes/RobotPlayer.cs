using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    class RobotPlayer : IPlayer
    {
        //should be in config class.
        public static readonly int InitialDraw = 7;
        
        private bool _isOpen;
        private Tiles _tiles;
        private GameGraph _graph;
        List<Domino> _hand;

        public RobotPlayer(Tiles t, GameGraph g)
        {
            _tiles = t;
            _graph = g;
            _hand = new List<Domino>();
            while (_hand.Count < InitialDraw)
            {
                Draw();
            }   

        }

        private void Draw()
        {
            _hand.Add(_tiles.Next());
        }

        public string Name()
        {
            return "Robot";
        }

        
        public bool IsOpen()
        {
            return _isOpen;
        }

        public bool Play()
        {
            bool played = LookFormatch(); 
            if (!played)
            {
                Draw();
                played = LookFormatch();
            }
            _isOpen = played;
            return _hand.Count < 1;
            
        }

        private bool LookFormatch()
        {
            foreach (Node end in _graph.Ends())
            {
                Domino match = _hand.FirstOrDefault(dom => dom.Matches(end.End));
                if (match != null)
                {
                    _graph.Add(end, match);
                    _hand.Remove(match);
                    return true;
                }
            }
            return false;
        }

    }
}
