using System.Windows.Threading;

namespace MemoryMatchingGame.Services
{
    public class GameTimerService
    {
        private readonly DispatcherTimer _gameTimer;
        public event Action? TimeChanged;
        private int _elapsedSeconds;
        public int ElapsedSeconds 
        {
            get => _elapsedSeconds;
            private set 
            {
                _elapsedSeconds = value;
                TimeChanged?.Invoke();
            } 
        }
        public bool IsRunning => _gameTimer.IsEnabled;

        public GameTimerService()
        {
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _gameTimer.Tick += (_, _) => ElapsedSeconds++;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _gameTimer.Start();
            }
        }
        public void Stop()
        {
            if (IsRunning)
            {
                _gameTimer.Stop();
            }
        }

        public void Reset()
        {
            Stop();
            ElapsedSeconds = 0;

        }
    }
}