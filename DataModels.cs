using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stack_Attack
{
    internal class DataModels
    {
        public int RowsCount { get; }
        public int ColumnsCount { get; }
        public int CellSize { get; }
        public int[,] GameMatrix { get; set; }

        public DataModels(int rowsCount, int columnsCount, int cellSize)
        {
            RowsCount = rowsCount;
            ColumnsCount = columnsCount;
            CellSize = cellSize;
            GameMatrix = new int[rowsCount, columnsCount];
        }
    }

    // Модель данных для красного блока.
    public class RedBlockData
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Size { get; }

        public RedBlockData(int row, int column, int size)
        {
            Row = row;
            Column = column;
            Size = size;
        }
    }

    // Модель данных для состояния игры.
    public class GameState
    {
        public bool IsGameOver { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public int LevelUpThreshold { get; set; }

        public GameState(int initialLevel = 1, int initialLevelUpThreshold = 10)
        {
            IsGameOver = false;
            Score = 0;
            Level = initialLevel;
            LevelUpThreshold = initialLevelUpThreshold;
        }
    }
}
