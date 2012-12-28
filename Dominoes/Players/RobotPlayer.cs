using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes.Players
{
    [DataContract]
    public class RobotPlayer : BasePlayer
    {
        [DataMember]
        private string _name;
        [DataMember]
        protected IRobotStratedgy _strat;

        
        public RobotPlayer(Tiles t, IRobotStratedgy strat, string name) : base(t)
        {
            _strat = strat;
            _name = name;            
        }
        
        public override string Name()
        {
            return _name;
        }


        public override async Task<bool> Play(GameGraph game, Tiles tiles)
        {
            await Task.Delay(300);
            if (!LookFormatch(game))
            {
                Draw(tiles);
                if (!LookFormatch(game)&& !Open)
                {
                    Global.Logger.Comment(string.Format("{0}'s line opened", Name()));
                    Open = true;
                }
            }
            return !_hand.Any();
        }

        private bool LookFormatch(GameGraph game)
        {
            Match pick = _strat.Choose(_hand, game.Ends(this));            
            if (pick != null)
            {
                //use a class instead of this tuple?
                game.Add(pick.end, pick.domino, this);
                _hand.Remove(pick.domino);
                Global.Logger.Comment(string.Format("{0} played {1} on {2}'s line and has {3} left", Name(), pick.domino, pick.end.Owner, _hand.Count));
                if (Open && pick.end.Owner.Equals(this.Name()))
                {
                    Global.Logger.Comment(string.Format("{0}'s line closed", Name()));
                    Open = false;
                }
                return true;
            }                        
            return false;
        }

        public static IEnumerable<Type> Types()
        {
            return new Type[] { typeof(Moocher), typeof(Boring), typeof(Fool), typeof(Dumper) };
        }
    }


    [DataContract]
    public class Moocher : RobotPlayer
    {
        public Moocher(Tiles t) : base(t, null, "Mooch") 
        {
            _strat = new MoocherStratedgy(Name());
        }
    }

    [DataContract]
    public class Boring : RobotPlayer
    {
        private static string[] _names = new[] { "fred", "wilma", "barney", "betty", "ted", "robert", "fanny" };
        private static uint nameCount = 0;  
        private static string GenerateName() 
        {
            return _names[nameCount++%_names.Length]; 
        }
        public Boring(Tiles t ) : base(t, new FirstTileStratedgy(), GenerateName()) { }
    }

    [DataContract]
    public class Fool : RobotPlayer
    {
        public Fool(Tiles t): base (t, new Dumbness(new FirstTileStratedgy(), 10), "Pinky") {}
    }

    [DataContract]
    public class Dumper : RobotPlayer
    {
        public Dumper(Tiles t) : base(t, new BiggestTileStatedgy(), "Denny") { }
    }
}
