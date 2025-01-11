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
using ThConsoleClient;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for DuoGamePage.xaml
    /// </summary>
    public partial class DuoHostGamePage : Page
    {
        private byte pid = 1;
        private byte gid = 0;
        byte score1 = 0, score2 = 0;
        ThConsoleClient.CommunicationHandler handler;
        public DuoHostGamePage()
        {
            InitializeComponent();
            var udpClient = new UdpClient();
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2004);
            handler = new ThConsoleClient.CommunicationHandler(udpClient, serverEndpoint);
            if (handler.SayHello()) Console.WriteLine("Connection opened!");

            handler.SendRequest("TH 0 0 start duo");
            var response = handler.ReciveResponseBytes();

            if (response[2] == 1)
            {
                gid = response[3];
                Render.RenderInitDuoGame(response, DuoCanvas);
                Render.RenderGameId(gid, DuoTxtGameId);
            }

            DuoCanvas.Focus();

            // Start game
            DuoGameLoop();
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

        private void DuoGameLoop()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var response = handler.ReciveResponseBytes();
                    if (response[2] == gid && response[4] == 100)
                    {
                        DuoGameOver();
                        break;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Render.RenderDuoTurn(response, DuoCanvas, DuoTxtScore);
                        score1 = response[7];
                        score2 = response[8];
                    });
                }
            });
        }

        private void DuoGameOver()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (score1 > score2)
                {
                    var answer = MessageBox.Show(string.Format("Время вышло! Игра окончена.\nПобеда первого игрока со счетом {0}:{1}\nВыйти из игры?", score1, score2), "Конец игры",
                        MessageBoxButton.YesNo);
                    if (answer == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        NavigationService.Navigate(new MainMenu());
                    }
                }
                else if (score1 < score2)
                {
                    var answer = MessageBox.Show(string.Format("Время вышло! Игра окончена.\nПобеда второго игрока со счетом {0}:{1}\nВыйти из игры?", score1, score2), "Конец игры",
                        MessageBoxButton.YesNo);
                    if (answer == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        NavigationService.Navigate(new MainMenu());
                    }
                }
                else
                {
                    var answer = MessageBox.Show(string.Format("Время вышло! Игра окончена.\nНичья! Счет {0}:{1}\nВыйти из игры?", score1, score2), "Конец игры",
                        MessageBoxButton.YesNo);
                    if (answer == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        NavigationService.Navigate(new MainMenu());
                    }
                }
            });
        }
    }
}
