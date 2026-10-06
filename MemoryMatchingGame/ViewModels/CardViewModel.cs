using MemoryMatchingGame.Models;
using System.Windows.Media;

namespace MemoryMatchingGame.ViewModels
{
    public class CardViewModel : ViewModelBase
    {
        private readonly Card _card;
        public int Id => _card.Id;
        public int PairId => _card.PairId;
        public ImageSource? Image => _card.Image;
        private bool _isFlipped;
        private bool _isMatched;

        public bool IsFlipped
        {
            get => _isFlipped;
            set => SetProperty(ref _isFlipped, value);
        }
        public bool IsMatched
        {
            get => _isMatched;
            set => SetProperty(ref _isMatched, value);
        }
        public CardViewModel(Card card)
        {
            _card = card;
        }
    }
}
