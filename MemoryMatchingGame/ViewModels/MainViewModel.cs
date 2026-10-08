using MemoryMatchingGame.Commands;
using MemoryMatchingGame.Models;
using MemoryMatchingGame.Services;
using System.Collections.ObjectModel;

namespace MemoryMatchingGame.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<CardViewModel> Cards { get; } = new();
        public TurnsViewModel Turns { get; } = new();
        private CardViewModel? _firstSelectedCard;
        private DifficultyOption _selectedDifficulty;
        private string _selectedCategory;
        private int _activeRows;
        private int _activeColumns;
        private int _moves;
        private int _misses;
        private bool _isGameWon;
        private bool _isBusy;
        private bool _isTwoPlayersMode;
        private string _winnerText = string.Empty;
        private readonly GameService _gameService = new();
        private readonly GameTimerService _timerService = new();
        public IReadOnlyList<DifficultyOption> Difficulties => _gameService.Difficulties;
        public IReadOnlyList<string> Categories => _gameService.Categories;

        public DifficultyOption SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (SetProperty(ref _selectedDifficulty, value))
                {
                    StartNewGame();
                }
            }
        }
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                    StartNewGame();
            }
        }
        public int ActiveRows
        {
            get => _activeRows;
            set => SetProperty(ref _activeRows, value);
        }
        public int ActiveColumns
        {
            get => _activeColumns;
            set => SetProperty(ref _activeColumns, value);
        }
        public int Moves
        {
            get => _moves;
            set
            {
                if (SetProperty(ref _moves, value))
                    OnPropertyChanged(nameof(Accuracy));
            }
        }
        public int Misses
        {
            get => _misses;
            set
            {
                if (SetProperty(ref _misses, value))
                    OnPropertyChanged(nameof(Accuracy));
            }
        }
        public string Accuracy
        {
            get
            {
                if (Moves == 0) return "100%";
                int percent = (int)(((double)(Moves - Misses) / Moves) * 100);
                return $"{percent}%";
            }
        }
        public bool IsGameWon
        {
            get => _isGameWon;
            set => SetProperty(ref _isGameWon, value);
        }
        public string ElapsedTimeString
        {
            get
            {
                int minutes = _timerService.ElapsedSeconds / 60;
                int seconds = _timerService.ElapsedSeconds % 60;
                return $"{minutes:D2}:{seconds:D2}";
            }
        }
        public bool IsTwoPlayersMode
        {
            get => _isTwoPlayersMode;
            set
            {
                if (SetProperty(ref _isTwoPlayersMode, value))
                    StartNewGame();
            }
        }
        public string WinnerText
        {
            get => _winnerText;
            set => SetProperty(ref _winnerText, value);
        }

        public RelayCommand FlipCardCommand { get; }
        public RelayCommand RestartCommand { get; }
        public RelayCommand CloseOverlayCommand { get; }
        public RelayCommand PeekCommand { get; }

        public MainViewModel()
        {
            _selectedCategory = Categories[0];
            _selectedDifficulty = Difficulties[1];

            FlipCardCommand = new RelayCommand(param =>
            {
                if (param is CardViewModel card)
                {
                    OnCardClicked(card);
                }
            });
            RestartCommand = new RelayCommand(_ => StartNewGame());
            CloseOverlayCommand = new RelayCommand(_ => IsGameWon = false);
            PeekCommand = new RelayCommand(async _ => await PeekCardsAsync());
            _timerService.TimeChanged += () => OnPropertyChanged(nameof(ElapsedTimeString));

            StartNewGame();
            _ = _gameService.PreloadAllImagesAsync();
        }
        private void StartNewGame()
        {
            Turns.Reset();
            _timerService.Reset();
            Cards.Clear();
            _firstSelectedCard = null;
            _isBusy = false;
            IsGameWon = false;
            WinnerText = string.Empty;
            Moves = 0;
            Misses = 0;

            ActiveRows = SelectedDifficulty.Rows;
            ActiveColumns = SelectedDifficulty.Columns;

            var deck = _gameService.CreateDeck(ActiveRows, ActiveColumns, SelectedCategory);

            foreach (var card in deck)
            {
                Cards.Add(new CardViewModel(card));
            }
        }
        private void EndGame()
        {
            _timerService.Stop();
            _isBusy = true;
            WinnerText = IsTwoPlayersMode ? Turns.GetWinnerMessage() : "Winner winner chicken dinner";
            IsGameWon = true;
        }

        private async void OnCardClicked(CardViewModel clickedCard)
        {
            if (_isBusy || clickedCard.IsFlipped || clickedCard.IsMatched)
            {
                return;
            }

            if (!_timerService.IsRunning)
            {
                _timerService.Start();
            }

            clickedCard.IsFlipped = true;

            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = clickedCard;
                return;
            }

            Moves++;
            _isBusy = true;

            if (_gameService.CheckMatch(_firstSelectedCard.PairId, clickedCard.PairId))
            {
                _firstSelectedCard.IsMatched = true;
                clickedCard.IsMatched = true;

                if (IsTwoPlayersMode)
                {
                    Turns.CurrentPlayer.Score++;
                }

                _firstSelectedCard = null;
                _isBusy = false;

                if (Cards.All(c => c.IsMatched))
                {
                    EndGame();
                }
            }
            else
            {
                Misses++;
                await Task.Delay(1000);
                _firstSelectedCard?.IsFlipped = false;
                clickedCard.IsFlipped = false;
                if (IsTwoPlayersMode)
                {
                    Turns.SwitchTurn();
                }
                _firstSelectedCard = null;
                _isBusy = false;
            }
        }
        private async Task PeekCardsAsync()
        {
            if (_isBusy || _isGameWon) return;

            _isBusy = true;

            var hiddenCards = new List<CardViewModel>();
            foreach(var card in Cards)
            {
                if(!card.IsFlipped && !card.IsMatched) 
                {
                    hiddenCards.Add(card);
                }
            }

            if (hiddenCards.Count == 0)
            {
                _isBusy = false;
                return;
            }

            foreach (var card in hiddenCards)
            {
                card.IsFlipped = true;
            }

            await Task.Delay(1000);

            foreach (var card in hiddenCards)
            {
                card.IsFlipped = false;
            }
            _isBusy = false;
        }
    }
}
