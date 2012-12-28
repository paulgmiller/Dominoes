using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using System.Runtime.Serialization;

namespace Dominoes.Players
{
    [DataContract]
    class HumanPlayer : BasePlayer
    {

        private TaskCompletionSource<VirtualKey> keyWaiter;

        public HumanPlayer(Tiles t) : base(t)
        {
        }

        //todo some unique microsoft id to differentiate human players

        public override string Name()
        {
            return "You";
        }

        //not sure if I like this pattern
        public void Input(VirtualKey keypress)
        {
            if (keyWaiter != null)
            {
                keyWaiter.TrySetResult(keypress);
            }
        }

        public string Paint()
        {
            var count = Enumerable.Range(1, _hand.Count).Select(i => string.Format("{0,7}", i));
            string painting = string.Join(" ", count);
            painting += "\n";
            painting += _hand.ToString();
            return painting;
        }

        public async Task<int?> AwaitKey()
        {
            while (true)
            {
                keyWaiter = new TaskCompletionSource<VirtualKey>();
                var key = await keyWaiter.Task;
                switch (key)
                {
                    case VirtualKey.Enter: return null;
                    case VirtualKey.Number0: return 10;
                    case VirtualKey.Number1: return 1;
                    case VirtualKey.Number2: return 2;
                    case VirtualKey.Number3: return 3;
                    case VirtualKey.Number4: return 4;
                    case VirtualKey.Number5: return 5;
                    case VirtualKey.Number6: return 6;
                    case VirtualKey.Number7: return 7;
                    case VirtualKey.Number8: return 8;
                    case VirtualKey.Number9: return 9;
                }
            }
            

        }

        private bool _kill = false;
        public void Kill()
        {
            _kill = true;
            if (keyWaiter != null)
            {
                keyWaiter.TrySetResult(VirtualKey.Enter);
            }
        }

        //Almost identical to robot player except for where the await happens
        public override async Task<bool> Play(GameGraph game, Tiles tiles)
        {
            if (! await AttemptToPlay(game) && !_kill)
            {
                Draw(tiles);
                Game.Instance().Paint();
                await Task.Delay(300);
                if (! await AttemptToPlay(game) && !Open)
                {
                    Open = true;
                }
            }
            return !_hand.Any();

        }

        private async Task<bool> AttemptToPlay(GameGraph game)
        {
            var ends = game.Ends(this).ToArray();
            while (true)
            {
                var end = await AwaitKey();
                if (end == null)
                {
                    Global.Logger.Comment("Skipping");
                    return false;
                }

                Global.Logger.Comment(string.Format("You chose {0} end", end));
                if (end > ends.Count())
                {
                    Global.Logger.Comment("Invalid end");
                    continue;
                }
                
                var domino = await AwaitKey();
                Global.Logger.Comment(string.Format("You chose {0} domino", domino));
                if (domino > _hand.Count)
                {
                    Global.Logger.Comment("Invalid domino");
                    continue;
                }
                var d =_hand.ToArray()[domino.Value-1];
                var e = ends[end.Value-1];
                
                if (d.Matches(e.End))
                {
                    e.AddChild(d);
                    _hand.Remove(d);
                    if (e.Owner == Name())
                    {
                        Open = false;
                    }

                    Global.Logger.Comment(string.Format("{0} played {1} on {2}'s line and has {3} left", Name(), d, e.Owner, _hand.Count));
                    return true;
                }
                else
                {
                    Global.Logger.Comment(string.Format("domino {0} doesn't match end {1}", d, e.End));
                }
            }            
        }
    }
}
