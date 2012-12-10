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
        HumanPlayer _you;
        IEnumerator<IPlayer> _player;
        Tiles _tiles = new Tiles();
        GameGraph _graph;
        string _result;
        Action<string> _paint; 

        public Game(Action<string> paint)
        {

            _you = new HumanPlayer(_tiles);
            _players = new List<IPlayer> { new Fool(_tiles), new Moocher(_tiles), new Dumper(_tiles), new Boring(_tiles), new Fool(_tiles) };
            _players.Add(new Mexican());
            _players.Add(_you);
            _graph = new GameGraph(_players);
            _paint = paint;
            Start();
            _paint(Paint());           
        }

        private string Paint()
        {
            return _graph.Paint(_you) + "\n\n" + _you.Paint();
        }

        async public Task Play()
        {
            try
            {
                bool winner = false;
                while (!winner)
                {
                   winner = await  Circle().Play(_graph);
                    _paint(Paint());                    
                }
                Global.Logger.Comment(_player.Current.Name() + " Wins");
            }
            catch (OutOfTiles)
            {
                Global.Logger.Comment("Out of tiles!");
            }
            finally
            {
                _paint(Paint());
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
                        _paint(Paint());
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

        public void Input(Windows.System.VirtualKey keyPress) { _you.Input(keyPress); }
    }

}
