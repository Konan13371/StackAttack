using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Stack_Attack
{
    // Класс, реализующий логику игры.
    internal class GameLogic
    {
        private Random random = new Random();
        public DataModels GameData { get; }
        public RedBlockData RedBlockData { get; }
        public GameState GameState { get; }
        public Action OnGameChanged { get; set; }

        public GameLogic(DataModels gameData, RedBlockData redBlockData, GameState gameState)
        {
            GameData = gameData;
            RedBlockData = redBlockData;
            GameState = gameState;
        }

        public void AddInitialBlocks()
        {
            // Добавляет начальные синие блоки на нижнюю строку поля.
            int numBlocksToAdd = random.Next(10, 12);
            List<int> usedCols = new List<int>();

            for (int i = 0; i < numBlocksToAdd; i++)
            {
                int col;
                // Цикл для выбора случайного столбца, не занятого другим блоком или красным блоком.
                do
                {
                    col = random.Next(GameData.ColumnsCount);
                } while (usedCols.Contains(col) || GameData.GameMatrix[GameData.RowsCount - 1, col] == 1 || IsRedBlockCell(GameData.RowsCount - 1, col));

                usedCols.Add(col);
                GameData.GameMatrix[GameData.RowsCount - 1, col] = 1;
            }
        }

        // Методы для работы с красным блоком
        public bool IsRedBlockCell(int row, int col)
        {
            // Проверяет, находится ли ячейка внутри красного блока.
            for (int i = RedBlockData.Row; i < RedBlockData.Row + RedBlockData.Size; ++i)
            {
                if (row == i && col == RedBlockData.Column) return true;
            }
            return false;
        }

        public bool TryMoveRedBlock(ref int newRow, ref int newCol, int deltaX, int deltaY)
        {
            // Пытается переместить красный блок, учитывая столкновения и возможность сдвига синих блоков.
            newRow = RedBlockData.Row + deltaY;
            newCol = RedBlockData.Column + deltaX;

            // Проверяем, не выходит ли красный блок за границы поля.
            if (newRow < 0 || newRow > GameData.RowsCount - RedBlockData.Size || newCol < 0 || newCol >= GameData.ColumnsCount)
                return false;

            // Проверяем на столкновение с синими блоками.
            bool collision = false;
            for (int i = newRow; i < newRow + RedBlockData.Size; i++)
            {
                if (GameData.GameMatrix[i, newCol] == 1)
                {
                    collision = true;
                    break;
                }
            }

            // Проверка наличия соседних синих блоков.
            bool hasNeighborBlock = false;

            // Проверяем блоки справа
            if (newCol < GameData.ColumnsCount - 1 && GameData.GameMatrix[newRow, newCol + 1] == 1)
                hasNeighborBlock = true;

            // Проверяем блоки слева
            if (newCol > 0 && GameData.GameMatrix[newRow, newCol - 1] == 1)
                hasNeighborBlock = true;

            // Проверяем блоки сверху
            if (newRow > 0 && GameData.GameMatrix[newRow - 1, newCol] == 1)
                hasNeighborBlock = true;

            // Если есть соседние блоки, красный блок не может перемещаться.
            if (hasNeighborBlock)
            {
                return false; // Красный блок не может быть перемещен.
            }
            // Если столкновения нет, перемещаем красный блок.
            if (!collision)
            {
                RedBlockData.Row = newRow;
                RedBlockData.Column = newCol;
                return true;
            }
            // Если столкновение есть, пытаемся сдвинуть синие блоки.
            else
            {
                if (PushBlueBlock(deltaX, deltaY))
                {
                    RedBlockData.Row = newRow;
                    RedBlockData.Column = newCol;
                    return true;
                }
                return false;
            }
        }

        private bool PushBlueBlock(int deltaX, int deltaY)
        {
            // Пытается сдвинуть синие блоки в указанном направлении.
            int pushRow = RedBlockData.Row;
            int pushCol = RedBlockData.Column;

            // Находим самую нижнюю ячейку красного блока, которая соприкасается с синим блоком.
            for (int i = 0; i < RedBlockData.Size; ++i)
            {
                if (GameData.GameMatrix[RedBlockData.Row + i, RedBlockData.Column + deltaX] == 1)
                {
                    pushRow = RedBlockData.Row + i;
                    pushCol = RedBlockData.Column + deltaX;
                    break;
                }
            }

            // Вычисляем новые координаты для сдвигаемого синего блока.
            int newPushRow = pushRow + deltaY;
            int newPushCol = pushCol + deltaX;

            // Проверяем, не выходит ли синий блок за границы поля или не встречает ли он препятствия.
            if (newPushRow < 0 || newPushRow >= GameData.RowsCount || newPushCol < 0 || newPushCol >= GameData.ColumnsCount || GameData.GameMatrix[newPushRow, newPushCol] == 1) return false;

            // Перемещаем синий блок.
            GameData.GameMatrix[newPushRow, newPushCol] = 1;
            GameData.GameMatrix[pushRow, pushCol] = 0;
            return true;
        }

        public bool CanMoveUp()
        {
            // Проверяем, есть ли синие блоки рядом для возможности перемещения вверх.
            int leftCol = RedBlockData.Column - 1;
            int rightCol = RedBlockData.Column + 1;

            // Проверяем наличие синих блоков слева или справа.
            for (int i = 0; i < RedBlockData.Size; i++)
            {
                if (leftCol >= 0 && GameData.GameMatrix[RedBlockData.Row + i, leftCol] == 1) return true;
                if (rightCol < GameData.ColumnsCount && GameData.GameMatrix[RedBlockData.Row + i, rightCol] == 1) return true;
            }
            return false;
        }

        public void ApplyGravityToRedBlock()
        {
            // Применяем гравитацию к красному блоку.
            if (RedBlockData.Row + RedBlockData.Size >= GameData.RowsCount)
            {
                return;
            }

            bool isSupported = false;
            for (int i = 0; i < RedBlockData.Size; i++)
            {
                if (RedBlockData.Row + RedBlockData.Size < GameData.RowsCount && GameData.GameMatrix[RedBlockData.Row + RedBlockData.Size, RedBlockData.Column] == 1)
                {
                    isSupported = true;
                    break;
                }
            }

            if (!isSupported)
            {
                RedBlockData.Row++;

                //проверяем что красный блок не столкнулся с нижним краем поля.
                if (RedBlockData.Row + RedBlockData.Size > GameData.RowsCount)
                {
                    RedBlockData.Row = GameData.RowsCount - RedBlockData.Size;
                }
            }
        }

        // Методы для управления состоянием игры
        public void CheckGameOver()
        {
            // Проверка на окончание игры (контакт красного блока с синими).
            for (int i = RedBlockData.Row; i < RedBlockData.Row + RedBlockData.Size; i++)
            {
                if (GameData.GameMatrix[i, RedBlockData.Column] == 1)
                {
                    GameState.IsGameOver = true;
                    break;
                }
            }
        }

        public void ProcessGameTick()
        {
            // Обработка тика таймера: падение блоков, очистка строк, повышение уровня.
            if (GameState.IsGameOver) return;

            bool gameChanged = false;

            // Генерация новых блоков
            if (random.Next(5) == 0)
            {
                int blockSide = random.Next(2);
                int col = blockSide == 0 ? random.Next(GameData.ColumnsCount / 2) : random.Next(GameData.ColumnsCount / 2, GameData.ColumnsCount);
                if (GameData.GameMatrix[0, col] == 0)
                {
                    GameData.GameMatrix[0, col] = 1;
                    gameChanged = true;
                }
            }

            // Движение блоков вниз
            for (int i = GameData.RowsCount - 2; i >= 0; i--)
            {
                for (int j = 0; j < GameData.ColumnsCount; j++)
                {
                    if (GameData.GameMatrix[i, j] == 1 && GameData.GameMatrix[i + 1, j] == 0)
                    {
                        GameData.GameMatrix[i + 1, j] = 1;
                        GameData.GameMatrix[i, j] = 0;
                        gameChanged = true;
                    }
                }
            }

            CheckGameOver();

            // Проверка на заполнение ряда и повышение уровня
            if (GameData.GameMatrix.Cast<int>().Skip((GameData.RowsCount - 1) * GameData.ColumnsCount).Take(GameData.ColumnsCount).All(x => x == 1))
            {
                GameState.Score += 10;
                if (GameState.Score >= GameState.LevelUpThreshold)
                {
                    GameState.Level++;
                    GameState.LevelUpThreshold = GameState.Level * 10;
                    // TODO: Добавить управление скоростью таймера
                    // timer.Interval = Math.Max(timer.Interval - 20, 20);
                }
                // Очистка ряда
                for (int j = 0; j < GameData.ColumnsCount; j++)
                {
                    GameData.GameMatrix[GameData.RowsCount - 1, j] = 0;
                }
                gameChanged = true;
            }

            ApplyGravityToRedBlock();

            if (gameChanged && OnGameChanged != null)
                OnGameChanged();
        }
    }
}