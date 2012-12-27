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
    class Game
    {
        [DataMember]
        Guid _id;
        [DataMember]
        List<IPlayer> _players;
        
        //should both of these just be unique names to look up players
        HumanPlayer _you;
        IEnumerator<IPlayer> _player;
        
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
            _singleton = serializer.ReadObject(stream) as Game;
            _singleton.Painter = paint;
            return _singleton;
        }

        public static void Save(System.IO.Stream stream)
        {
            serializer.WriteObject(stream, Game.Instance());
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

            _you = new HumanPlayer(_tiles);
            _players = new List<IPlayer> { new Fool(_tiles), new Moocher(_tiles), new Dumper(_tiles), new Boring(_tiles), new Fool(_tiles) };
            _players.Add(new Mexican());
            _players.Add(_you);
            _graph = new GameGraph(_players);
            Painter = paint;
            Start();
            Paint();
            _id = Guid.NewGuid();
        }

        public static IEnumerable<Type> KnownTypes()
        {
            return RobotPlayer.Types().Concat(RobotStratedies.Types()).Concat(new [] { typeof(HumanPlayer), typeof(Mexican) } );
        }

        public void Paint()
        {
            Painter(_graph.Paint(_you) + "\n\n" + _you.Paint());
        }

        async public Task Play()
        {
            try
            {
                bool winner = false;
                while (!winner)
                {
                   winner = await  Circle().Play(_graph, _tiles);
                   Paint();                       
                }
                Global.Logger.Comment(_player.Current.Name() + " Wins");
            }
            catch (OutOfTiles)
            {
                Global.Logger.Comment("Out of tiles!");
            }
            finally
            {
                Paint();   
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
                        Paint();   
                        return;
                    }
                } 
            }

            Global.Logger.Comment("Drawing since noone had a double");
            foreach (var p in _players) p.Draw(_tiles);

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

        public void Input(Windows.System.VirtualKey keyPress) { 
            _you.Input(keyPress);
        }
    }

}
