using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class EnPassant : Move
    {
        public override MoveType Type => MoveType.EnPassant;

        public override Position FromPos { get; }

        public override Position ToPos { get; }

        private readonly Position capturePos;

        public EnPassant(Position fromPos, Position toPos)
        {
            FromPos = fromPos;
            ToPos = toPos;
            capturePos=new Position(fromPos.Row, toPos.Column);
        }

        public override void Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);
            board[capturePos] = null;
        }
    }
}
