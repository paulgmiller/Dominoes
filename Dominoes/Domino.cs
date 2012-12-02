using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes
{
    class Domino
    {
        public int First { get; private set; }
        public int Second { get; private set; }
        public readonly static int MAX_DOTS;
        public readonly static int MIN_DOTS;

        public Domino(int one, int two)
        {
            First = one;
            Second = two;

            if (First > MAX_DOTS || First < 0)
                throw new ArgumentOutOfRangeException(string.Format("first isn't a valid domino: {0}", First));
            if (Second > MAX_DOTS || Second < 0)
                throw new ArgumentOutOfRangeException(string.Format("second isn't a valid domino: {0}", Second));
        }

        public string ToString()
        {
            return string.Format("[{0}|{1}]", First, Second);
        }

        public int GetHashCode()
        {
            //good chance this is buggy
            return (MAX_DOTS+1)*First + Second;
        }

        public bool IsDouble()
        {
            return First == Second;
        }
            
    }
}
