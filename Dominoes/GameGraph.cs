﻿using System;
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
            
        }

        //should only be used to start lines.
        internal  Node(Domino v)
        {
             if (!v.IsDouble())
                throw new ArgumentException("Must start game graph with double");
            
             Value  = v;
             Available  = 1;
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

    

    public class GameGraph
    {
        
        private class Line
        {
            public IPlayer Owner; 
            public Node Start;
        }

        private Domino _root = null;
        private List<Line> _lines;

        public GameGraph(IEnumerable<IPlayer> players)
        {
           _lines = players.Select(p => new Line { Owner = p }).ToList();
        }

        public void Start(Domino starter)
        {
            if (!starter.IsDouble())
                throw new ArgumentException("Must start game graph with double");
            
            if (_root != null)
                throw new InvalidOperationException("Graph already started");

            _root = starter;
            foreach (var line in _lines)
            {
                line.Start = new Node(starter);
            }
        }

        public IEnumerable<Node> Ends(IPlayer player)
        {
            var openlines = _lines.Where(l => l.Owner == player || l.Owner.IsOpen());
            return openlines.SelectMany(l => l.Start.Ends());
        }

        public override string ToString()
        {
            var builder = new StringBuilder(); 
            builder.AppendLine(_root.ToString());
            foreach (var line in _lines)
            {
                builder.AppendFormat("{0,-7}", line.Owner);
                if (line.Start.Children.Any())
                    PrintNode(line.Start.Children.First(), builder, 1);
                else
                    builder.AppendLine();
            }
            return builder.ToString();
        }

        private void PrintNode(Node node, StringBuilder sb, int indent)
        {
            if (node.End == node.Value.Second)
                sb.Append(node.Value);
            else
                sb.Append(node.Value.ToBackwardsString());

            if (!node.Children.Any())
            {
                sb.AppendLine();
                return;
            }
            PrintNode(node.Children.First(), sb, indent+1);
            foreach (var child in node.Children.Skip(1))
            {
                foreach ( var i in Enumerable.Range(0,indent)) sb.Append("       ");
                PrintNode(node.Children.First(), sb, indent+2);
            }
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
