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
using ChessLogic.Moves;

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
            if(IsMenuOnScreen())
            {
                return;
            }

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
                if(move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPos, move.ToPos);
                }
                else
                {

                    HandleMove(move);
                }
            }
        }

        private void HandlePromotion(Position from, Position to)
        {
            pieceImages[to.Row, to.Column].Source = Images.GetImage(gameStatus.CurrentPlayer, PieceType.Pawn);
            pieceImages[from.Row, to.Column].Source = null;

            PromotionMenu promotionMenu=new PromotionMenu(gameStatus.CurrentPlayer);
            MenuContainer.Content = promotionMenu;
            promotionMenu.PieceSelected += type =>
            {
                MenuContainer.Content = null;
                Move promMove = new PawnPromotion(from, to, type);
                HandleMove(promMove);
            };
        }

        private void HandleMove(Move move)
        {
            gameStatus.MakeMove(move);
            DrawBoard(gameStatus.Board);
            SetCursor(gameStatus.CurrentPlayer);

            HighlightCheckPosition();

            if (gameStatus.IsGameOver())
            {
                ShowGameOver();
            }
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
        private void HighlightCheckPosition()
        {
            // Ellenőrizzük, hogy a jelenlegi játékos királya sakkban van-e
            Position checkPosition = gameStatus.GetCheckedKingPosition();
            if (checkPosition != null)
            {
                // Piros színnel kiemeljük a sakkban lévő király mezőjét
                Color checkColor = Color.FromArgb(200, 255, 0, 0); // Világos piros
                highlights[checkPosition.Row, checkPosition.Column].Fill = new SolidColorBrush(checkColor);
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
            Position checkPosition = gameStatus.GetCheckedKingPosition();
            if (checkPosition != null)
            {
                highlights[checkPosition.Row, checkPosition.Column].Fill = Brushes.Transparent;
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
        private bool IsMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }

        private void ShowGameOver()
        {
            GameOverMenu gameOverMenu = new GameOverMenu(gameStatus);
            MenuContainer.Content = gameOverMenu;

            gameOverMenu.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    MenuContainer.Content = null;
                    RestartGame();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            };
        }

        private void RestartGame()
        {
            HideHighlights();
            moveCache.Clear();
            gameStatus = new GameStatus(Player.White, Board.Intial());
            DrawBoard(gameStatus.Board);
            SetCursor(gameStatus.CurrentPlayer);
        }
    }
}