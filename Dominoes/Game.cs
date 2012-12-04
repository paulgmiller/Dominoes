﻿using System;
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


        public Game()
        {
            _players  = Enumerable.Range(0, 4).Select(c => new RobotPlayer(_tiles)).Cast<IPlayer>().ToList();
            _players.Add(new Mexican());
            _graph = new GameGraph(_players);
            Start();
            //being too 
            bool winner = false;
            while (!winner) 
            {
                winner = Circle().Play(_graph);
            }
            Global.Logger.LogComment(_player.Current.Name() + " Wins");
            
        }   

        void Start()
        {

            for (int startValue = Domino.MAX_DOTS; startValue >= Domino.MIN_DOTS; --startValue)
            {
                _player = _players.GetEnumerator();
                while (_player.MoveNext()) 
                {
                    if (_player.Current.Start(startValue, _graph))
                        return;                    
                } 
            }

            //nobody had a double.
            foreach (var p in _players) p.Draw();

            //Try again;
            Start();
        }

        public IPlayer Circle()
        {
            //Assert we have some players?

            if (!_player.MoveNext())
            {
                _player = _players.GetEnumerator();
                _player.MoveNext();
            }
            return _player.Current;
        }

        
    }

}
