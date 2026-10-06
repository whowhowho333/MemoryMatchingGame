using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryMatchingGame.Models
{
    public class DifficultyOption
    {
        public string Title { get; }
        public int Rows { get; }
        public int Columns { get; }

        public DifficultyOption(string title, int columns, int rows)
        {
            Title = title;
            Columns = columns;
            Rows = rows;
        }
    }
}
