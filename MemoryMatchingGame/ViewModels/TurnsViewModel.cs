namespace MemoryMatchingGame.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        public int PlayerNumber { get; }
        private int _score;
        private bool _isPlayerTurn;
        public int Score
        {
            get => _score;
            set => SetProperty(ref  _score, value);
        }
        public bool IsPlayerTurn
        {
            get => _isPlayerTurn;
            set => SetProperty(ref _isPlayerTurn, value);
        }
        public PlayerViewModel(int playerNumber)
        {
            PlayerNumber = playerNumber;
        }
        public void Reset()
        {
            Score = 0;
            IsPlayerTurn = false;
        }
    }
    public class TurnsViewModel : ViewModelBase
    {
        public PlayerViewModel Player1 { get; } = new(1);
        public PlayerViewModel Player2 { get; } = new(2);
        private PlayerViewModel _currentPlayer;
        public PlayerViewModel CurrentPlayer
        {
            get => _currentPlayer;
            private set => SetProperty(ref _currentPlayer, value);
        }

        public TurnsViewModel()
        {
            _currentPlayer = Player1;
        }
        public void Reset()
        {
            Player1.Reset();
            Player2.Reset();
            CurrentPlayer = Player1;
            CurrentPlayer.IsPlayerTurn = true;
        }

        public void SwitchTurn()
        {
            CurrentPlayer.IsPlayerTurn = false;
            CurrentPlayer = (CurrentPlayer == Player1) ? Player2 : Player1;
            CurrentPlayer.IsPlayerTurn = true;
        }
        public string GetWinnerMessage()
        {
            if (Player1.Score > Player2.Score)
                return $"Player 1 wins! ({Player1.Score} : {Player2.Score})";
            if (Player2.Score > Player1.Score)
                return $"Player 2 wins! ({Player2.Score} : {Player1.Score})";

            return $"Draw! ({Player1.Score} : {Player2.Score})";
        }
    }
}