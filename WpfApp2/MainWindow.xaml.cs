using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ThConsoleClient; // import the threading name space to use the dispatcher time
namespace PAC_Man_Game_WPF_MOO_ICT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte gid = 0;
        private byte pid = 1;
        private const int CellSize = 20;

        int score = 0; // score keeping integer
        ThConsoleClient.CommunicationHandler handler;
        public MainWindow()
        {
            // Create a new UdpClient
            var udpClient = new UdpClient();
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2004);
            handler = new ThConsoleClient.CommunicationHandler(udpClient, serverEndpoint);
            if (handler.SayHello()) Console.WriteLine("Connection opened!");

            InitializeComponent();

        }

        private void StartSoloGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide menu
            StartSoloGameButton.Visibility = Visibility.Hidden;
            StartDuoGameButton.Visibility = Visibility.Hidden;
            GameIdTextBox.Visibility = Visibility.Hidden;
            ConnectButton.Visibility = Visibility.Hidden;

            handler.SendRequest("TH 0 0 start solo");
            var response = handler.ReciveResponseBytes();

            if (response[2] == 1)
            {
                gid = response[3];
                Render.RenderInitSoloGame(response, MyCanvas);
            }

            MyCanvas.Focus();

            // Start game
            SoloGameLoop();
        }

        private void StartDuoGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide menu
            StartSoloGameButton.Visibility = Visibility.Hidden;
            StartDuoGameButton.Visibility = Visibility.Hidden;
            GameIdTextBox.Visibility = Visibility.Hidden;
            ConnectButton.Visibility = Visibility.Hidden;

            handler.SendRequest("TH 0 0 start duo");
            var response = handler.ReciveResponseBytes();

            if (response[2] == 1)
            {
                gid = response[3];
                Render.RenderInitDuoGame(response, MyCanvas);
            }

            MyCanvas.Focus();

            // Start game
            DuoGameLoop();
        }

        private void ConnectToGame(byte gameId)
        {
            pid = 2;

            handler.SendRequest("TH " + gameId + " 2 connect");

            var response = handler.ReciveResponseBytes();

            if (response[2] == 1)
            {
                gid = response[3];
                Render.RenderInitDuoGame(response, MyCanvas);
            }

            handler.SendRequest("TH " + gameId + " 2 begin");

            MyCanvas.Focus();
            DuoGameLoop();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, что поле не пустое
            if (string.IsNullOrEmpty(GameIdTextBox.Text))
            {
                MessageBox.Show("Please enter a valid Game ID.");
                return;
            }

            // Попытка преобразовать введенное значение в byte (ID игры)
            if (byte.TryParse(GameIdTextBox.Text, out byte gameId))
            {
                // Скрыть меню
                StartSoloGameButton.Visibility = Visibility.Hidden;
                StartDuoGameButton.Visibility = Visibility.Hidden;
                GameIdTextBox.Visibility = Visibility.Hidden;
                ConnectButton.Visibility = Visibility.Hidden;

                // Вызов метода ConnectToGame с переданным ID игры
                ConnectToGame(gameId);
            }
            else
            {
                MessageBox.Show("Invalid Game ID. Please enter a number.");
            }
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            string gidS = gid.ToString();
            // this is the key down event
            if (e.Key == Key.Left)
            {
                handler.SendRequest("TH " + gidS + " " + pid + " go l");   
            }
            if (e.Key == Key.Right)
            {
                handler.SendRequest("TH " + gidS + " " + pid + " go r");
            }
            if (e.Key == Key.Up)
            {
                handler.SendRequest("TH " + gidS + " " + pid + " go u");
            }
            if (e.Key == Key.Down)
            {
                handler.SendRequest("TH " + gidS + " " + pid + " go d");
            }
        }

        private void SoloGameLoop()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var response = handler.ReciveResponseBytes();
                    if (response[2] == gid && response[4] == 100)
                    {
                        GameOver();
                        break;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Render.RenderSoloTurn(response, MyCanvas, txtScore);
                    });
                }
            });
        }

        private void DuoGameLoop()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var response = handler.ReciveResponseBytes();
                    if (response[2] == gid && response[4] == 100)
                    {
                        GameOver();
                        break;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Render.RenderDuoTurn(response, MyCanvas, txtScore);
                    });
                }
            });
        }
        private void GameOver()
        {
            MessageBox.Show("Время вышло! Игра окончена."); // Сообщение о завершении игры
            // when the player clicks ok on the message box
            // restart the application
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

    }
}