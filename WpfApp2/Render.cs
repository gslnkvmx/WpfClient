using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThConsoleClient
{
    internal class Render
    {
        const int maze_size = 16;
        private static byte[] _maze = new byte[maze_size * maze_size];
        private const int CellSize = 40;
        private static byte[] _treasures = new byte[10 * 2];

        static Image playerImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/images/hero.jpg")), // Укажите путь к изображению
                Width = CellSize,
                Height = CellSize
            };

        static Ellipse treasure = new Ellipse
        {
            Fill = Brushes.Gold,
            Width = CellSize,
            Height = CellSize
        };

        static public void RenderInitSoloGame(byte[] bytes, Canvas MyCanvas)
        {
            Array.Copy(bytes, 4, _maze, 0, maze_size * maze_size);

            byte playerX = bytes[260];
            byte playerY = bytes[261];

            for (int y = 0; y < maze_size ; y++)
            {
                for (int x = 0; x < maze_size; x++)
                {
                    Rectangle cell = new Rectangle
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Fill = _maze[y*maze_size + x] == 0 ? Brushes.Black : Brushes.SandyBrown
                    };

                    // Устанавливаем позицию клетки
                    Canvas.SetLeft(cell, x * CellSize);
                    Canvas.SetTop(cell, y * CellSize);

                    MyCanvas.Children.Add(cell); // Добавляем клетку на Canvas
                }
            }

            Array.Copy(bytes, 263, _treasures, 0, 20);


            MyCanvas.Children.Add(playerImage); // Добавляем изображение игрока на Canvas

            MyCanvas.Children.Add(treasure);

            RenderTreasure(_treasures[1], _treasures[0], MyCanvas);

            RenderPlayer(playerX, playerY, MyCanvas);

        }

        static private void RenderPlayer(int playerX, int playerY, Canvas MyCanvas)
        {
            // Устанавливаем позицию игрока
            Canvas.SetLeft(playerImage, playerX * CellSize);
            Canvas.SetTop(playerImage, playerY * CellSize);
        }

        static private void RenderTreasure(int treasureX, int treasureY, Canvas MyCanvas)
        {
            // Устанавливаем позицию игрока
            Canvas.SetLeft(treasure, treasureX * CellSize);
            Canvas.SetTop(treasure, treasureY * CellSize);
        }

        static public void RenderTurn(byte[] bytes, Canvas MyCanvas)
        
        {
            byte playerX = bytes[3];
            byte playerY = bytes[4];

            RenderTreasure(bytes[7], bytes[6], MyCanvas);

            RenderPlayer(playerX, playerY, MyCanvas);
        }
    }
}
