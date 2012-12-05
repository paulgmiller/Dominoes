﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public interface IRobotStratedgy
    {
        Match Choose(Hand hand, IEnumerable<Node> ends);
    }

    
    //just take the first match you find
    public class FirstTileStratedgy  : IRobotStratedgy
    {
        public Match Choose(Hand hand, IEnumerable<Node> ends)
        {
            return Match.Find(hand, ends).FirstOrDefault();
        }
    }

    //Play the biggest match you have
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
    /*public class KingOFFools : IRobotStratedgy
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