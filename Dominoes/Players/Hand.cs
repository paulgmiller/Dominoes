using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.Players
{
    public class Hand : List<Domino>
    {
        public Hand() { }
        private Hand(IEnumerable<Domino> tiles) : base(tiles) { }
        
        public Hand Match(Node end) 
        { 
            return new Hand(this.Where(dom => dom.Matches(end.End))); 
        }
        
        public override string ToString() 
        { 
            return String.Join(" ", this.Select(d => d.ToString())); 
        }
    }
}
