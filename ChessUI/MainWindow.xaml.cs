using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessLogic;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position,Move> moveCache=new Dictionary<Position, Move>();

        private GameStatus gameStatus;
        private Position selectedPos = null;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            gameStatus = new GameStatus(Player.White, Board.Intial());
            DrawBoard(gameStatus.Board);
            SetCursor(gameStatus.CurrentPlayer);
        }
        public void InitializeBoard()
        {
            for(int i = 0;i<8;i++)
            {
                for(int j=0;j<8;j++)
                {
                    Image img= new Image();
                    pieceImages[i,j] = img;
                    PieceGrid.Children.Add(img);

                    Rectangle highlight=new Rectangle();
                    highlights[i, j] = highlight;
                    HighlightGrid.Children.Add(highlight);
                }
            }
        }

        private void DrawBoard(Board board)
        {
            for (int i = 0; i<8;i++)
            {
                for(int j=0 ; j<8;j++)
                {
                    Piece piece = board[i, j];
                    pieceImages[i,j].Source = Images.GetImage(piece);
                }
            }
        }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point=e.GetPosition(BoardGrid);
            Position pos=ToSquarePosition(point);
            if(selectedPos==null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPositionSelected(pos);
            }
        }

        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameStatus.LegalMovesForPiece(pos);
            if(moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighlights();
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighlights();

            if(moveCache.TryGetValue(pos, out Move move))
            {
                HandleMove(move);
            }
        }

        private void HandleMove(Move move)
        {
            gameStatus.MakeMove(move);
            DrawBoard(gameStatus.Board);
            SetCursor(gameStatus.CurrentPlayer);
        }

        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row=(int)(point.Y/squareSize);
            int col=(int)(point.X/squareSize);
            return new Position(row, col);
        }

        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();
            foreach (Move move in moves)
            {
                moveCache[move.ToPos] = move;
            }
        }

        private void ShowHighlights()
        {
            Color color;

            if (gameStatus.CurrentPlayer == Player.White)
            {
                color = Color.FromArgb(150, 255, 51, 51); // Piros, ha a fehér lép
            }
            else
            {
                color = Color.FromArgb(150, 0, 255, 255); // Kék, ha a fekete lép
            }

            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHighlights()
        {
            foreach(Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
            }
        }

        private void SetCursor(Player player)
        {
            if(player==Player.White)
            {
                Cursor = ChessCursors.WhiteCursor;
            }
            else
            {
                Cursor = ChessCursors.BlackCursor;
            }
        }

    }
}