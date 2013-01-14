using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes.Players
{
    public class Match
    {
        public Domino domino;
        public Node end;
        public static IEnumerable<Match> Find(IEnumerable<Domino> hand, IEnumerable<Node> ends)
        {
            return ends.SelectMany(end =>
            {
                return hand.Where(d => d.Matches(end.End)).Select(d => new Match { domino = d, end = end });
            });
        }
    }

    public class RobotStratedies
    {
        //this seems error prone how do we autoenumerate types in here?
        public static IEnumerable<Type> Types()
        {
            return new Type[] { typeof(Dumbness),typeof(FirstTileStratedgy), typeof(BiggestTileStatedgy), typeof(MoocherStratedgy) };
        }
    }


    public interface IRobotStratedgy
    {
        Match Choose(Hand hand, IEnumerable<Node> ends);
    }

    [DataContract]
    public class Dumbness : IRobotStratedgy
    {
        private static Random _rnd = new Random();
        [DataMember]
        private int _missPercent;
        [DataMember]
        private IRobotStratedgy _underlying;
        public Dumbness(IRobotStratedgy underlying, int misspercent)
        {
            _missPercent = misspercent;
            _underlying = underlying;
            if ( misspercent < 0 || misspercent > 100)
            {
                throw new ArgumentOutOfRangeException("missperceent", "must be between 0 and 100 percent");
            }
        }
           
        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            //miss some ends. Maybe they drank too much.
           return  _underlying.Choose(hand, ends.Where(e => _rnd.Next(100) > _missPercent));
        }
    }

    
    //just take the first match you find
    [DataContract]
    public class FirstTileStratedgy  : IRobotStratedgy
    {
        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            return Match.Find(hand, ends).FirstOrDefault();
        }
    }

    //Play the biggest match you have
    [DataContract]
    public class BiggestTileStatedgy : IRobotStratedgy
    {
        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            var matches = Match.Find(hand, ends);
            //failed to use max here
            return matches.OrderByDescending(m => m.domino).FirstOrDefault();
        }
    }

    //Always plays other peoples lines then his biggest.
    [DataContract]
    public class MoocherStratedgy : IRobotStratedgy
    {
        [DataMember]
        private string _playerToMooch;
        public MoocherStratedgy(string player) 
        {
            _playerToMooch = player;
        }

        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            var matches = Match.Find(hand, ends);
            //duplicative of biggest tile stratedgy
            return matches.OrderBy(m => m.end.Owner == _playerToMooch ? 0 : 1).ThenByDescending(m => m.domino).FirstOrDefault();
        }
    }

    //This is actually my personal strategy. Use your hand to construct the longest line(s) on your end(s). Play tiles not on these lines first then play them in order.
    
     [DataContract]
    public class KingOFFoolishness : IRobotStratedgy
    {
        [DataMember]
        private string _me;
        public KingOFFoolishness(string me)
        {
            _me = me;
        }

        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            var matches = Match.Find(hand, ends.Where(e => e.Owner.Equals(_me)));
            if (!matches.Any())
            {
                return new BiggestTileStatedgy().Choose(hand, ends);
            }
          
            IEnumerable<Hand> chains = matches.SelectMany(m => FindChains(hand.Except( new[] { m.domino }), new Node(m.domino, m.end)));
            var chain = chains.Max(); 
            if (Game.Instance().GetPlayer(_me).Open)
            {
                var excess = hand.Except(chain);
                var notmine = Match.Find(excess, ends); //ends should except the one chain wants to use
                if (notmine.Any())
                    return new BiggestTileStatedgy().Choose(new Hand(excess), ends);
            }

            return Match.Find(new[] { chain.First() }, ends).First();
            

        }
         
         private IEnumerable<Hand> FindChains(IEnumerable<Domino> pool, Node chain)
         {
           
             var matches = Match.Find(pool, chain.Ends()); 
             if (!matches.Any())
             {
                 return new[] { new Hand(chain.Dominoes()) };
             }
             else
             {
                 return matches.SelectMany(m =>
                 {
                     m.end.AddChild(m.domino);
                     var chains = FindChains(pool.Except(new[] { m.domino }), chain);
                     m.end.RemoveChild(m.domino);
                     return chains;
                 });
             }
         }
        
    }
}