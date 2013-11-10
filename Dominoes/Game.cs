using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Dominoes.Players;

namespace Dominoes
{
    //this is dumb and buggy. Should either know all the types in Domones.Players or call something that declares them all known
    [KnownType("KnownTypes")]
    [DataContract(Name="Game",Namespace="Dominoes")]
    public class Game
    {
        [DataMember]
        public Guid Id { get; private set; }
        [DataMember]
        List<IPlayer> _players;

        
        HumanPlayer _you { get { return _players.Single(prop => prop.Name().Equals(_youID)) as HumanPlayer; } }
        [DataMember]
        string _youID;


        IEnumerator<IPlayer> _player;
        [DataMember]
        string _currentPlayer;
        private void AdvanceToCurrent()
        {
            var target = _currentPlayer; //save this 
            do {
                Circle();
            } while (!_currentPlayer.Equals(target));
        }

        
        [DataMember]
        Tiles _tiles = new Tiles();
        [DataMember]
        GameGraph _graph;
        

        //should this be a delegate? oh well should go away when we get a real ui.
        public Action<string> Painter { get; private set; }

        static Game _singleton = null;
        public static Game NewGame(Action<string> paint)
        {
           _singleton = new Game(paint);
            return _singleton;
        }
        static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Game));
        public static Game Load(System.IO.Stream stream, Action<string> paint)
        {
            
            var game = serializer.ReadObject(stream) as Game;
            //not supper happy about these but hard to serialize wihtout a bunch of redundancy.
            game.Painter = paint;
            game.AdvanceToCurrent();
            game._graph.Players = _singleton._players;
            return game;
        }

        public static void Swap(Game game)
        {
            var old = _singleton;
            _singleton = game;
            old.Kill();
            _singleton.Paint();
        }

        public void Save(System.IO.Stream stream)
        {
            serializer.WriteObject(stream, this);
        }

        public static Game Instance()
        {
            if (_singleton == null)
            {
                throw new NullReferenceException();
            }
            return _singleton;
        }

        private  Game(Action<string> paint)
        {
            var you = new HumanPlayer(_tiles);
            _players = new List<IPlayer> { 
                    new Fool(_tiles), 
                    new Moocher(_tiles), 
                    new Dumper(_tiles), 
                    new Boring(_tiles), 
                    new Boring(_tiles), 
                    new KingOfFools(_tiles),
                    new Mexican(),
                    you
            };
            _players.Add(you);
            _youID = you.Name();
            _graph = new GameGraph(_players);
            Painter = paint;
            Start();
            Paint();
            Id = Guid.NewGuid();
        }

        private bool _kill = false;
        public void Kill()
        {
            _kill = true;
            _you.Kill();
        }

        public static IEnumerable<Type> KnownTypes()
        {
            return RobotPlayer.Types().Concat(RobotStratedies.Types()).Concat(new[] { typeof(HumanPlayer), typeof(Mexican) });
        }

        public void Paint()
        {
            Painter(_graph.Paint(_you) + "\n\n" + _you.Paint());
        }

        async public Task<bool> Play()
        {
            try
            {
                bool winner = false;
                while (!winner && !_kill)
                {
                   winner = await _player.Current.Play(_graph, _tiles);
                   Circle();
                   Paint();                       
                }
                if (winner)
                    Global.Logger.Comment(_player.Current.Name() + " Wins");
            }
            catch (OutOfTiles)
            {
                Global.Logger.Comment("Out of tiles!");
                Paint();   
            }
            return _kill;            
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
                        Circle(); //next player goes
                        Paint();
                        return;
                    }
                } 
            }

            Global.Logger.Comment("Drawing since no one had a double");
            foreach (var p in _players) p.Draw(_tiles);

            //Try again;
            Start();
        }

        public IPlayer Circle()
        {
            if (_player == null)
            {
                _player = _players.GetEnumerator();
            }

            if (!_player.MoveNext())
            {
                _player = _players.GetEnumerator();
                _player.MoveNext();
            }
            _currentPlayer = _player.Current.Name();
            return _player.Current;
        }

        public void Input(Windows.System.VirtualKey keyPress) { 
            _you.Input(keyPress);
        }
    }

}
