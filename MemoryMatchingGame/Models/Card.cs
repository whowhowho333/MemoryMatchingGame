using System.Windows.Media;

namespace MemoryMatchingGame.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int PairId { get; set; }
        public ImageSource? Image { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
    }
}