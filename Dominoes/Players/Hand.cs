using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes.Players
{
    [CollectionDataContract(Name = "Hand", Namespace = "Dominoes")]
    public class Hand : List<Domino>
    {
        public Hand() { }
        public Hand(IEnumerable<Domino> tiles) : base(tiles) { }

        public Hand Match(Node end)
        {
            return new Hand(this.Where(dom => dom.Matches(end.End)));
        }

        public override string ToString()
        {
            return String.Join(" ", this.Select(d => d.ToString()));
        }

        public int Total
        {
            get
            {
                return this.Aggregate<Domino, int>(0, (score, d) => score + d.Score);
            }
        }
    }
}
