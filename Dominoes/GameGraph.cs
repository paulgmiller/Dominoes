using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes
{
    [DataContract(Name = "Domino", Namespace = "Dominoes")]
    public class Node 
    {
        //used by King of fools otherwise it would be private.
        internal Node(Domino v, Node parent)
        {
             Value  = v;
             Children = new List<Node>();
             if (parent.End == Value.First)
                 End = Value.Second;
             else if (parent.End == Value.Second)
                 End = Value.First;
             else
                 throw new ArgumentException("Dominoes don't match");
             Owner = parent.Owner;
            
        }

        //should only be used to start lines.
        internal  Node(Domino v, string owner)
        {
             if (!v.IsDouble())
                throw new ArgumentException("Must start game graph with double");
            
             Value  = v;
             Children = new List<Node>();
             End = Value.First;
             Owner = owner;
        }

        public IEnumerable<Domino> Dominoes()
        {
            return new [] { Value }.Concat( Children.SelectMany( c => c.Dominoes()));
        }
        
        public int Available { 
            get {
                return (Value.IsDouble() ? 3 : 1) - Children.Count;        
            }
        }

        [DataMember]
        public Domino Value { get; private set; }
        
        //if we want to save on storage size could generate this at deserialization. 
        [DataMember]
        public int End { get; private set; }

        [DataMember]
        public List<Node> Children { get; private  set; }

        //would rather this just be a refernce but that doesn't serialize well.
        [DataMember]
        public string Owner { get; private set; }

        
        public void AddChild(Domino child)
        {
           if (Available <= 0) 
               throw new InvalidOperationException("No open plays on this domino:" + Value.ToString());
           Children.Add(new Node(child, this));
        }

        public void RemoveChild(Domino child)
        {
            var c = Children.Find(n => n.Value.Equals(child));
            if (c == null) throw new ArgumentOutOfRangeException("child");
            Children.Remove(c);
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


    [DataContract(Name = "GameGraph", Namespace = "Dominoes")]
    public class GameGraph
    {
        [DataMember()]
        private Domino _root = null;
        [DataMember]
        private List<Node> _lines;
        //only really one game to set this.
        public IEnumerable<IPlayer> Players { get; internal set; }

        public GameGraph(IEnumerable<IPlayer> players)
        {
            Players = players;
        }

        public void Start(Domino starter)
        {
            if (!starter.IsDouble())
                throw new ArgumentException("Must start game graph with double");
            
            if (_root != null)
                throw new InvalidOperationException("Graph already started");

            _root = starter;
            _lines = Players.Select(p => new Node(starter, p.Name())).ToList();
        }

        public IEnumerable<Node> Ends(IPlayer player)
        {
            //boo using a global variable here.
            var openlines = _lines.Where(l => l.Owner == player.Name() || Players.Single(p => p.Name().Equals(l.Owner)).Open);
            return openlines.SelectMany(l => l.Ends());
        }

        public string Paint(IPlayer viewer)
        {
            var open = Ends(viewer);
            var builder = new StringBuilder(); 
            builder.AppendLine(_root.ToString());
            int openCount = 0;
            foreach (var line in _lines)
            {
                builder.AppendFormat("{0,-7}", line.Owner);
                if (line.Children.Any())
                {
                    openCount = PrintNode(line.Children.First(), builder, 1, openCount, open);
                }
                else
                {
                    if (open.Contains(line))
                    {
                        builder.Append(string.Format(" ({0})", ++openCount));
                    }
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }

        private int PrintNode(Node node, StringBuilder sb, int indent, int openCount, IEnumerable<Node> open)
        {
            if (node.End == node.Value.Second)
                sb.Append(node.Value);
            else
                sb.Append(node.Value.ToBackwardsString());

            if (!node.Children.Any())
            {
                if (open.Contains(node))
                   sb.Append(string.Format(" ({0})", ++openCount));
                sb.AppendLine();
                return openCount;
            }
            openCount = PrintNode(node.Children.First(), sb, indent+1, openCount, open);
            foreach (var child in node.Children.Skip(1))
            {
                foreach ( var i in Enumerable.Range(0,indent+1)) sb.Append("       ");
                openCount =PrintNode(child, sb, indent + 2, openCount, open);
            }
            //refactor this and the if statement above together.
            int remaining = node.Available - 1;
            if (remaining > 0 && open.Contains(node))
            {
                foreach (int i in Enumerable.Range(0,remaining))
                {
                    foreach (var j in Enumerable.Range(0, indent)) sb.Append("       ");
                    sb.AppendLine(string.Format(" ({0})", ++openCount));
                }
            }

            
            return openCount;
        }

        public  void Add(Node end, Domino d, IPlayer player)
        {
            if (_root == null)
                throw new InvalidOperationException("Graph has not been started");

            var possibleEnds = Ends(player);
            if (!possibleEnds.Contains(end))
            {
                throw new ArgumentException("Must use a valid end!");
            }
            end.AddChild(d);
        }
    }
}
