using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class GameStatus
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public GameStatus(Player player,Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }
    }
}
