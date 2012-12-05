using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.Players
{
    public class Hand : List<Domino>
    {
        public Hand() {}
        private Hand(IEnumerable<Domino> tiles) : base(tiles) { }
        public Hand Match(Node end) { return new Hand(this.Where(dom => dom.Matches(end.End))) ; }
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
        protected IRobotStratedgy _strat;

        
        public RobotPlayer(Tiles t, IRobotStratedgy strat, string name)
        {
            _tiles = t;
            _strat = strat;
            _hand = new Hand();
            _name = name;
            while (_hand.Count < InitialDraw)
            {
                Draw();
            }               
            Global.Logger.LogDebug(string.Format("{0, 7} drew {1}", Name(), _hand));
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
            Match pick = _strat.Choose(_hand, game.Ends(this));            
            if (pick != null)
            {
                //use a class instead of this tuple?
                game.Add(pick.end, pick.domino, this);
                _hand.Remove(pick.domino);
                Global.Logger.LogComment(string.Format("{0} played {1} on {2}'s line and has {3} left", Name(), pick.domino, pick.end.Owner, _hand.Count));
                if (_isOpen && pick.end.Owner == this)
                {
                    Global.Logger.LogComment(string.Format("{0}'s line closed", Name()));
                    _isOpen = false;
                }
                return true;
            }                        
            return false;
        }
    }

    public class Moocher : RobotPlayer
    {
        public Moocher(Tiles t) : base(t, null, "Mooch") 
        {
            _strat = new MoocherStratedgy(this);
        }
    }

    public class Fool : RobotPlayer
    {
        private static string[] _names = new[] { "fred", "wilma", "barney", "betty", "ted", "robert", "fanny" };
        private static int nameCount = 0;  
        private static string GenerateName() { return _names[nameCount++]; }
        public Fool(Tiles t) : base(t, new FirstTileStratedgy(), GenerateName()) { }
    }

    public class Dumper : RobotPlayer
    {
        public Dumper(Tiles t) : base(t, new BiggestTileStatedgy(), "Denny") { }
    }
}
