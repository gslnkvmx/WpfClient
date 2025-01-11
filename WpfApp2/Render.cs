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
        private const int CellSize = 37;
        private static byte[] _treasures = new byte[2];

        static Image playerImage = new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,,/images/hero.jpg")), // Укажите путь к изображению
            Width = CellSize,
            Height = CellSize
        };

        static Image player2Image = new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,,/images/pink.jpg")), // Укажите путь к изображению
            Width = CellSize,
            Height = CellSize
        };

        static Image treasure = new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,,/images/coin.gif")), // Укажите путь к изображению
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
                        Fill = Brushes.Black
                    };

                    Image brick = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/images/brick.jpg")), // Укажите путь к изображению
                        Width = CellSize,
                        Height = CellSize
                    };
                    if (_maze[y * maze_size + x] != 0)
                    {
                        // Устанавливаем позицию клетки
                        Canvas.SetLeft(brick, x * CellSize);
                        Canvas.SetTop(brick, y * CellSize);
                        MyCanvas.Children.Add(brick); // Добавляем клетку на Canvas
                    }
                    else
                    {
                        // Устанавливаем позицию клетки
                        Canvas.SetLeft(cell, x * CellSize);
                        Canvas.SetTop(cell, y * CellSize);
                        MyCanvas.Children.Add(cell); // Добавляем клетку на Canvas
                    }
                }
            }

            Array.Copy(bytes, 263, _treasures, 0, 2);


            MyCanvas.Children.Add(playerImage); // Добавляем изображение игрока на Canvas

            MyCanvas.Children.Add(treasure);

            RenderTreasure(_treasures[1], _treasures[0], MyCanvas);

            RenderPlayer(playerX, playerY, MyCanvas, playerImage);

        }

        static public void RenderInitDuoGame(byte[] bytes, Canvas MyCanvas)
        {
            Array.Copy(bytes, 4, _maze, 0, maze_size * maze_size);

            byte player1X = bytes[260];
            byte player1Y = bytes[261];
            byte player2X = bytes[262];
            byte player2Y = bytes[263];

            for (int y = 0; y < maze_size; y++)
            {
                for (int x = 0; x < maze_size; x++)
                {
                    Rectangle cell = new Rectangle
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Fill = Brushes.Black
                    };

                    Image brick = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/images/brick.jpg")), // Укажите путь к изображению
                        Width = CellSize,
                        Height = CellSize
                    };
                    if (_maze[y * maze_size + x] != 0)
                    {
                        // Устанавливаем позицию клетки
                        Canvas.SetLeft(brick, x * CellSize);
                        Canvas.SetTop(brick, y * CellSize);
                        MyCanvas.Children.Add(brick); // Добавляем клетку на Canvas
                    }
                    else
                    {
                        // Устанавливаем позицию клетки
                        Canvas.SetLeft(cell, x * CellSize);
                        Canvas.SetTop(cell, y * CellSize);
                        MyCanvas.Children.Add(cell); // Добавляем клетку на Canvas
                    }
                }
            }

            Array.Copy(bytes, 266, _treasures, 0, 2);

            MyCanvas.Children.Add(playerImage); // Добавляем изображение игрока на Canvas
            MyCanvas.Children.Add(player2Image);

            MyCanvas.Children.Add(treasure);

            RenderTreasure(_treasures[1], _treasures[0], MyCanvas);

            RenderPlayer(player1X, player1Y, MyCanvas, playerImage);
            RenderPlayer(player2X, player2Y, MyCanvas, player2Image);
        }

        static private void RenderPlayer(int playerX, int playerY, Canvas MyCanvas, Image playerImage)
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
        static public void RenderGameId(byte gameId, Label gameIdLabel)

        {
            gameIdLabel.Content = "Game ID: " + gameId.ToString();
        }

        static public void RenderSoloTurn(byte[] bytes, Canvas MyCanvas, Label scoreLabel)
        
        {
            byte playerX = bytes[3];
            byte playerY = bytes[4];

            RenderTreasure(bytes[7], bytes[6], MyCanvas);

            RenderPlayer(playerX, playerY, MyCanvas, playerImage);

            scoreLabel.Content = "Score: " + bytes[5].ToString();
        }

        static public void RenderDuoTurn(byte[] bytes, Canvas MyCanvas, Label scoreLabel)

        {
            byte player1X = bytes[3];
            byte player1Y = bytes[4];
            byte player2X = bytes[5];
            byte player2Y = bytes[6];
            byte player1Score = bytes[7];
            byte player2Score = bytes[8];

            RenderTreasure(bytes[10], bytes[9], MyCanvas);

            RenderPlayer(player1X, player1Y, MyCanvas, playerImage);
            RenderPlayer(player2X, player2Y, MyCanvas, player2Image);

            scoreLabel.Content = player1Score.ToString() + " : " + player2Score.ToString();
        }
    }
}
