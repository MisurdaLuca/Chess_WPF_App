using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessLogic;

namespace ChessUI
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> whiteSources = new()
        {
            {PieceType.Pawn, LoadImage("Assets/white_pawn.png") },
            {PieceType.Bishop,LoadImage("Assets/white_bishop.png") },
            {PieceType.Rook,LoadImage("Assets/white_rook.png") },
            {PieceType.Knight,LoadImage("Assets/white_knight.png") },
            {PieceType.Queen,LoadImage("Assets/white_queen.png") },
            {PieceType.King,LoadImage("Assets/white_king.png") }
        };

        private static readonly Dictionary<PieceType, ImageSource> blackSources = new()
        {
            {PieceType.Pawn, LoadImage("Assets/black_pawn.png") },
            {PieceType.Bishop,LoadImage("Assets/black_bishop.png") },
            {PieceType.Rook,LoadImage("Assets/black_rook.png") },
            {PieceType.Knight,LoadImage("Assets/black_knight.png") },
            {PieceType.Queen,LoadImage("Assets/black_queen.png") },
            {PieceType.King,LoadImage("Assets/black_king.png") }
        };

        private static ImageSource LoadImage(string filepath)
        {
            return new BitmapImage(new Uri(filepath,UriKind.Relative));
        }

        public static ImageSource GetImage(Player color, PieceType type)
        {
            return color switch
            {
                Player.White => whiteSources[type],
                Player.Black => blackSources[type],
                _ => null
            };
        }

        public static ImageSource GetImage(Piece piece)
        {
            if(piece == null) return null;

            return GetImage(piece.Color, piece.Type);
        }
    }
}
