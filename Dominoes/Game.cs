using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominoes.Players;

namespace Dominoes
{
    class Game
    {
        List<IPlayer> _players;
        IEnumerator<IPlayer> _player;
        Tiles _tiles = new Tiles();
        GameGraph _graph;
        string _result;
        Action<string> _paint; 

        public Game(Action<string> paint)
        {
            _players = new List<IPlayer> { new Fool(_tiles), new Moocher(_tiles), new Dumper(_tiles), new Boring(_tiles), new Fool(_tiles) };
            _players.Add(new Mexican());
            _graph = new GameGraph(_players);
            _paint = paint;
            Start();
            _paint(_graph.ToString());           
        }

        async public Task Play()
        {
            try
            {
                bool winner = false;
                while (!winner)
                {
                    winner = Circle().Play(_graph);
                    _paint(_graph.ToString());
                    await Task.Delay(300);
                }
                Global.Logger.Comment(_player.Current.Name() + " Wins");
            }
            catch
            {
                Global.Logger.Comment("Out of tiles!");
            }
            finally
            {
                _paint(_graph.ToString());
            }
        }
       

        void Start()
        {

            for (int startValue = Domino.MAX_DOTS; startValue >= Domino.MIN_DOTS; --startValue)
            {
                _player = _players.GetEnumerator();
                while (_player.MoveNext()) 
                {
                    if (_player.Current.Start(startValue, _graph))
                    {
                        _paint(_graph.ToString());
                        return;
                    }
                } 
            }

            Global.Logger.Comment("Drawing since noone had a double");
            foreach (var p in _players) p.Draw();

            //Try again;
            Start();
        }

        public IPlayer Circle()
        {
            if (!_player.MoveNext())
            {
                _player = _players.GetEnumerator();
                _player.MoveNext();
            }
            return _player.Current;
        }

        
    }

}
