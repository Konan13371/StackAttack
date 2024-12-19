using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Stack_Attack
{
    internal class GraphicRendering
    {
        private const int marginTop = 25;
        private const int marginLeft = 25;
        private DataModels gameData;
        private RedBlockData redBlockData;
        private GameState gameState;
        private List<Image> blockImages;
        private Image redBlockImage;

        public GraphicRendering(DataModels gameData, RedBlockData redBlockData, GameState gameState, List<Image> blockImages, Image redBlockImage)
        {
            this.gameData = gameData;
            this.redBlockData = redBlockData;
            this.gameState = gameState;
            this.blockImages = blockImages;
            this.redBlockImage = redBlockImage;
        }

        public void Render(Graphics graphics)
        {
            // Отрисовка игрового поля и элементов.
            graphics.SmoothingMode = SmoothingMode.AntiAlias; // Сглаживание.

            // Рисуем сетку и блоки.
            for (int i = 0; i < gameData.RowsCount; i++)
            {
                for (int j = 0; j < gameData.ColumnsCount; j++)
                {
                    // Цвет ячейки можно менять в зависимости от ее состояния (например, i + j % 2 == 0).
                    Brush brush = new SolidBrush(Color.Transparent);

                    // Создаем прямоугольник для ячейки.
                    Rectangle rect = new Rectangle(marginLeft + j * gameData.CellSize, marginTop + i * gameData.CellSize, gameData.CellSize, gameData.CellSize);

                    // Рисуем прямоугольник.
                    graphics.FillRectangle(brush, rect);
                    graphics.DrawRectangle(Pens.Black, rect);

                    brush.Dispose(); // Освобождаем ресурсы.

                    // Рисуем синие квадраты, если ячейка заполнена.
                    if (gameData.GameMatrix[i, j] == 1)
                    {
                        // Выбираем случайное изображение из списка
                        int imageIndex = (i + j) % blockImages.Count;
                        Image currentImage = blockImages[imageIndex];
                        graphics.DrawImage(currentImage, rect);
                    }
                }
            }

            // Рисуем красный блок с учетом отступов.
            Rectangle redRect = new Rectangle(marginLeft + redBlockData.Column * gameData.CellSize, marginTop + redBlockData.Row * gameData.CellSize, gameData.CellSize, gameData.CellSize);
            for (int i = 0; i < redBlockData.Size; ++i)
            {
                redRect = new Rectangle(marginLeft + redBlockData.Column * gameData.CellSize, marginTop + (redBlockData.Row + i) * gameData.CellSize, gameData.CellSize, gameData.CellSize);
                graphics.DrawImage(redBlockImage, redRect);
            }

            // Сообщение об окончании игры.
            if (gameState.IsGameOver)
            {
                graphics.DrawString("Игра окончена!", new Font("Times New Romans", 50), Brushes.Aqua, new PointF(gameData.ColumnsCount * gameData.CellSize / 3, gameData.RowsCount * gameData.CellSize / 2 + marginTop));
            }

            // Отображение счета и уровня.
            graphics.DrawString($" Очки: {gameState.Score}; Уровень: {gameState.Level}", new Font("Microsoft Sans Serif", 20), Brushes.Black, new PointF(10, 630));
        }
    }
}
