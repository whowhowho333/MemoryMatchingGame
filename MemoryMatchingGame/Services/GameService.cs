using MemoryMatchingGame.Models;
using System.Windows.Media.Imaging;

namespace MemoryMatchingGame.Services
{
    public class GameService
    {
        private readonly Random _random = new();
        private readonly Dictionary<string, BitmapImage> _imageCache = new();
        public IReadOnlyList<string> Categories { get; } = ["Animals", "Flowers", "Fruits"];
        public IReadOnlyList<DifficultyOption> Difficulties { get; } =
        [
            new("Beginner — 4×3 (6 pairs)", 4, 3),
            new("Classic — 4×4 (8 pairs)", 4, 4),
            new("Medium — 6×4 (12 pairs)", 6, 4),
            new("Hard — 6×6 (18 pairs)", 6, 6)
        ];

        public List<Card> CreateDeck(int rows, int columns, string category)
        {
            int totalPairs = rows * columns / 2;
            var deck = new List<Card>();
            for (int i = 1; i <= totalPairs; i++)
            {
                string path = GetCardPath(category, i);
                BitmapImage bitmap = GetOptimizedImage(path);

                deck.Add(new Card 
                { 
                    Id = i * 2 - 1, 
                    PairId = i, 
                    Image = bitmap
                });

                deck.Add(new Card 
                { 
                    Id = i * 2, 
                    PairId = i,
                    Image = bitmap
                });
            }
            Shuffle(deck);
            return deck;
        }

        public bool CheckMatch(int firstPairId, int secondPairId)
        {
            return firstPairId == secondPairId;
        }

        private void Shuffle(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int j = _random.Next(i + 1);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        private BitmapImage GetOptimizedImage(string path)
        {
            if (_imageCache.TryGetValue(path, out var image))
                return image;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelWidth = 200;
            bitmap.EndInit();

            bitmap.Freeze();
            _imageCache[path] = bitmap;
            return bitmap;
        }
        public Task PreloadAllImagesAsync()
        {
            int totalCardPairs = (Difficulties[^1].Rows * Difficulties[^1].Columns) / 2;
            return Task.Run(() =>
            {
                foreach (var category in Categories)
                {
                    for (int i = 1; i <= totalCardPairs; i++)
                    {
                        string path = GetCardPath(category, i);
                        GetOptimizedImage(path);
                    }
                }
            });
        }

        private string GetCardPath(string category, int index) 
            => $"pack://application:,,,/Images/{category}/card{index}.jpg";
    }
}
