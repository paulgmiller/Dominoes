using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.Players
{
    public class Hand : List<Domino>
    {
        public override string ToString() { return String.Join(" ", this.Select(d => d.ToString())); }        
    }
    
    public class RobotPlayer : IPlayer
    {
        //should be in config class.
        public static readonly int InitialDraw = 7;
        
        private bool _isOpen = false;
        private Tiles _tiles;
        private Hand _hand;
        private string _name;

        static string[] _names = new[] { "fred", "wilma", "barney", "betty", "wintanabot" };
        static int _robotCount = 0;

        public RobotPlayer(Tiles t)
        {
            _tiles = t;
            _name = _names[_robotCount]; // +" " + _robotCount.ToString();
            ++_robotCount;
            _hand = new Hand();
            while (_hand.Count < InitialDraw)
            {
                Draw();
            }   
            
            Global.Logger.LogComment(string.Format("{0} drew {1}", Name(), _hand));
        }

        

        public void Draw()
        {
            var domino = _tiles.Next();
            Global.Logger.LogDebug(string.Format("{0} drew a {1}", Name(), domino));
            _hand.Add(domino);
        }

        public string Name()
        {
            return _name;
        }

        public override string ToString()
        {
            return Name();
        }

        
        public bool IsOpen()
        {
            return _isOpen;
        }

        public bool Play(GameGraph game)
        {
            if (!LookFormatch(game))
            {
                Draw();
                if (!LookFormatch(game)&& !_isOpen)
                {
                    Global.Logger.LogComment(string.Format("{0}'s line opened", Name()));
                    _isOpen = true;
                }
            }

            
            return _hand.Count < 1;
            
        }

        public bool Start(int startValue, GameGraph g)
        {
            Domino match = _hand.Where(d => d.IsDouble()).FirstOrDefault(d => d.Matches(startValue));
            if (match == null) return false ;
            Global.Logger.LogComment(string.Format("{0} started {1}", Name(), match));
            _hand.Remove(match);
            g.Start(match);
            return true;
        }

        private bool LookFormatch(GameGraph game)
        {
            foreach (Node end in game.Ends(this))
            {
                Domino match = _hand.FirstOrDefault(dom => dom.Matches(end.End));
                if (match != null)
                {
                    game.Add(end, match, this);
                    _hand.Remove(match);
                    Global.Logger.LogComment(string.Format("{0} played {1} and has {2} left", Name(), match, _hand.Count));
                    if (_isOpen && end.Owner == this)
                    {
                        Global.Logger.LogComment(string.Format("{0}'s line closed", Name()));
                        _isOpen = false;
                    }
                    return true;
                }
            }
            
            return false;
        }



    }
}
