using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Dominoes.Players
{
    //thins in common between human and robot player
    [DataContract(Name = "Tiles", Namespace = "Dominoes")]
    abstract public class BasePlayer : IPlayer
    {
        //should be in config class.
        public static readonly int InitialDraw = 7;
        
        [DataMember]
        protected Hand _hand;

        protected BasePlayer(Tiles t)
        {
            Open = false;
            _hand = new Hand();
            while (_hand.Count < InitialDraw)
            {
                Draw(t);
            }
            Global.Logger.Debug(string.Format("{0, 7} drew {1}", Name(), _hand));
        }
       
        [DataMember]
        public bool Open {  get; protected set; }

        public void Draw(Tiles tiles)
        {
            var domino = tiles.Next();
            Global.Logger.Debug(string.Format("{0} drew a {1}", Name(), domino));
            _hand.Add(domino);
        }

        abstract public string Name();

        public override string ToString()
        {
            return Name();
        }

        abstract public Task<bool> Play(GameGraph g, Tiles t);
        
        public bool Start(int startValue, GameGraph g)
        {
            Domino match = _hand.Where(d => d.IsDouble()).FirstOrDefault(d => d.Matches(startValue));
            if (match == null) return false;
            Global.Logger.Comment(string.Format("{0} started {1}", Name(), match));
            _hand.Remove(match);
            g.Start(match);
            return true;
        }

    }
}
