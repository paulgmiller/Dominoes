using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes
{
    [DataContract(Name = "Domino", Namespace = "Dominoes")]
    public class Domino : IComparable<Domino>
    {
        
        public int First { get { return values[0]; } }
        public int Second { get { return values[1]; } }
        
        //idealluy this is just serialized as n|m but even if we implement iserialable that seems hard to do. 
        //Could have Tiles, Hand, and node all do it manually but that seems dumb.
        [DataMember(Name="v")]
        private int[] values;

        public readonly static int MAX_DOTS = 12; 
        public readonly static int MIN_DOTS = 0;

        public Domino(int one, int two)
        {
            values = new int[] { one, two };
            

            if (First > MAX_DOTS || First < 0)
                throw new ArgumentOutOfRangeException(string.Format("first isn't a valid domino: {0}", First));
            if (Second > MAX_DOTS || Second < 0)
                throw new ArgumentOutOfRangeException(string.Format("second isn't a valid domino: {0}", Second));
        }

        public override string ToString()
        {
            return string.Format("[{0,2}|{1,2}]", First, Second);
        }

        public string ToBackwardsString()
        {
            return string.Format("[{0,2}|{1,2}]", Second, First);
        }

        public override int GetHashCode()
        {
            //good chance this is buggy
            return (MAX_DOTS+1)*First + Second;
        }

        public bool IsDouble()
        {
            return First == Second;
        }

        public bool Matches(int end)
        {
            return end == First || end == Second;
        }

        public int Score { get { return First + Second; } }

        public int CompareTo(Domino other)
        {
            return Score.CompareTo(other.Score);
        }
    }
}
