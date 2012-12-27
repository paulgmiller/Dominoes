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
        private IPlayer _player;
        public MoocherStratedgy(IPlayer p) 
        {
            _player = p;
        }

        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            var matches = Match.Find(hand, ends);
            return matches.OrderBy(m => m.end.Owner == _player ? 0 : 1 ).ThenByDescending(m => m.domino).FirstOrDefault();
        }
    }

    //This is actually my personal strategy. Use your hand to construct the longest line(s) on your end(s). Play tiles not on these lines first then play them in order.
    /*
     [DataContract]
    public class KingOFFools : IRobotStratedgy
    {
        private IPlayer _player;
        public MoocherStratedgy(IPlayer p)
        {
            _player = p;
        }

        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            var matches = Match.Find(hand, ends);
            return matches.OrderBy(m => m.end.Owner == _player ? 0 : 1).ThenByDescending(m => m.domino).FirstOrDefault();
        }
    }*/
}
