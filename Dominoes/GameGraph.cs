using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    public class Node
    {
        private Node(Domino v, Node parent)
        {
             Value  = v;
             Available  = Value.IsDouble() ? 3 : 1;
             Children = new List<Node>();
             if (parent.End == Value.First)
                 End = Value.Second;
             else if (parent.End == Value.Second)
                 End = Value.First;
             else
                 throw new ArgumentException("Dominoes don't match");
             //Need to know who's player I am.
        }

        //used to start the game
        internal  Node(Domino v, IEnumerable<string> players)
        {
             if (!v.IsDouble())
                throw new ArgumentException("Must start game graph with double");
            
             Value  = v;
             Available  = players.Count(); //+1 for mexican?
             Children = new List<Node>();
             End = Value.First;
        }
        public int Available { get; private set; }
        
        public Domino Value { get; private set; }
        public int End { get; private set; }
        public List<Node> Children { get; private  set; }
        
        public void AddChild(Domino child)
        {
           if (Available <= 0) 
               throw new InvalidOperationException("No open plays on this domino:" + Value.ToString());
           Children.Add(new Node(child, this));
           --Available;
        }

        public IEnumerable<Node> Ends()
        {
            var ends =  Children.SelectMany(c => c.Ends());
            if (Available > 0)
            {
                ends = ends.Concat(new [] { this });
            }
            return ends;
        }
    }

    class GameGraph
    {
        private Node _root;
        
        public GameGraph(Domino starter)
        {
            if (!starter.IsDouble())
                throw new ArgumentException("Must start game graph with double");
            _root = new Node(starter, new [] { "fred", "wilma", "barney", "betty" } );
        }

        public IEnumerable<Node> Ends()
        {
            return _root.Ends();
        }

        public  void Add(Node end, Domino d)
        {
            if (!Ends().Contains(end))
            {
                throw new ArgumentException("Must use a valid end!");
            }
            end.AddChild(d);
        }
    }
}
