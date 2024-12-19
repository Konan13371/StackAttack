using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace Stack_Attack
{
    public partial class Game : Form
    {
        // Размеры игрового поля и ячейки
        private const int rowsCount = 8;
        private const int columnsCount = 12;
        private const int cellSize = 75;

        // Создаем экземпляры классов
        private DataModels gameData;
        private RedBlockData redBlockData;
        private GameState gameState;
        private GameLogic gameLogic;
        private GraphicRendering graphicRendering;

        private Timer timer;
        private List<Image> blockImages = new List<Image>();
        private Image redBlockImage;
        private SoundPlayer backgroundMusicPlayer;

        public Game()
        {
            // Включение двойной буферизации для плавной отрисовки.
            DoubleBuffered = true;

            // Инициализация компонентов, созданных дизайнером форм.
            InitializeComponent();

            // Инициализация классов данных
            gameData = new DataModels(rowsCount, columnsCount, cellSize);
            redBlockData = new RedBlockData(6, 6, 1);
            gameState = new GameState();

            // Загрузка изображений и музыки
            LoadBlockImages();
            LoadRedBlockImage();
            LoadBackgroundMusic();
            StartBackgroundMusic();

            // Инициализация классов логики и отрисовки
            gameLogic = new GameLogic(gameData, redBlockData, gameState);
            graphicRendering = new GraphicRendering(gameData, redBlockData, gameState, blockImages, redBlockImage);

            gameLogic.OnGameChanged = () => Invalidate();

            // Добавление начальных синих блоков.
            gameLogic.AddInitialBlocks();

            // Настройка и запуск таймера.
            timer = new Timer { Interval = 200 };
            timer.Tick += timer1_Tick;
            timer.Start();
        }

        private void LoadBackgroundMusic()
        {
            // Загрузка фоновой музыки из ресурсов.
            System.IO.Stream stream = Properties.Resources.MusicStackAttack;

            if (stream != null)
            {
                backgroundMusicPlayer = new SoundPlayer(stream);
                backgroundMusicPlayer.Load();
            }
            else
            {
                // Обработка ошибки загрузки музыки (можно заменить на логирование).
                MessageBox.Show("Music resource not found!");
            }
        }

        private void StartBackgroundMusic()
        {
            if (backgroundMusicPlayer != null)
            {
                // Запуск фоновой музыки в цикле.
                backgroundMusicPlayer.PlayLooping();
            }
        }

        private void LoadBlockImages()
        {
            // Загрузка изображений для анимации синих блоков.
            blockImages.Add(Properties.Resources.Frame1);
            blockImages.Add(Properties.Resources.Frame2);
            blockImages.Add(Properties.Resources.Frame3);
            blockImages.Add(Properties.Resources.Frame4);
            blockImages.Add(Properties.Resources.Frame5);
        }

        private void LoadRedBlockImage()
        {
            // Загрузка изображения красного блока.
            redBlockImage = Properties.Resources.Frame6;
        }

        private void Game_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Обработка закрытия формы.
            StopBackgroundMusic();
            Application.Exit();
        }

        private void StopBackgroundMusic()
        {
            // Остановка фоновой музыки.
            if (backgroundMusicPlayer != null)
            {
                backgroundMusicPlayer.Stop();
                backgroundMusicPlayer.Dispose();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            // Обработка нажатия кнопки "Выход".
            if (MessageBox.Show("Вы точно хотите закончить игру?", "Информация", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                Application.Exit();
        }

        private void Game_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            // Обработка нажатия клавиш для управления красным блоком.
            if (gameState.IsGameOver) return;

            int newRow = redBlockData.Row;
            int newCol = redBlockData.Column;
            bool moved = false; // Флаг, указывающий на успешное перемещение.

            // Обработка нажатия клавиш для перемещения красного блока.
            switch (keyEventArgs.KeyCode)
            {
                case Keys.Left:
                    moved = gameLogic.TryMoveRedBlock(ref newRow, ref newCol, - 1, 0);
                    break;
                case Keys.Right:
                    moved = gameLogic.TryMoveRedBlock(ref newRow, ref newCol, 1, 0);
                    break;
                case Keys.Up:
                    if (gameLogic.CanMoveUp()) moved = gameLogic.TryMoveRedBlock(ref newRow, ref newCol, 0, -1);
                    break;
                case Keys.Down:
                    moved = gameLogic.TryMoveRedBlock(ref newRow, ref newCol, 0, 1);
                    break;
            }

            // Дополнительная проверка на столкновение после перемещения.
            if (newRow >= 0 && newRow <= gameData.RowsCount - redBlockData.Size && newCol >= 0 && newCol < gameData.ColumnsCount)
            {
                // Проверка на попытку движения.
                // Проверка на столкновения перед перемещением.
                bool collision = false; // Флаг, указывающий на обнаружение столкновения.

                // Проверка на столкновение с синими блоками.
                for (int i = newRow; i < newRow + redBlockData.Size; i++)
                {
                    if (gameData.GameMatrix[i, newCol] == 1)
                    {
                        collision = true;
                        break;
                    }
                }

                // Перемещение красного блока, если столкновения нет.
                if (!collision) // Если столкновение не обнаружено, обновляем столбец красного блока на новый столбец.
                {
                    redBlockData.Row = newRow;
                    redBlockData.Column = newCol;
                    moved = true;
                    gameLogic.CheckGameOver();
                }
            }

            // Перерисовка игрового поля, если блок переместился.
            if (moved) Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Обработка тика таймера
            gameLogic.ProcessGameTick();
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка игрового поля и элементов.
            graphicRendering.Render(e.Graphics);
        }
    }
}