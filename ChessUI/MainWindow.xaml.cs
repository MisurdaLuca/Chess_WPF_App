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

        private GameStatus gameStatus;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            gameStatus = new GameStatus(Player.White, Board.Intial());
            DrawBoard(gameStatus.Board);
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
    }
}