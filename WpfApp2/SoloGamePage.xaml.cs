using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
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
using static System.Formats.Asn1.AsnWriter;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class SoloGamePage : Page
    {
        private byte pid = 1;
        private byte gid = 0;
        string score = "0";
        ThConsoleClient.CommunicationHandler handler;
        public SoloGamePage()
        {
            InitializeComponent();
            var udpClient = new UdpClient();
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2004);
            handler = new ThConsoleClient.CommunicationHandler(udpClient, serverEndpoint);
            if (handler.SayHello()) Console.WriteLine("Connection opened!");

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

        private void SoloGameLoop()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var response = handler.ReciveResponseBytes();
                    if (response[2] == gid && response[4] == 100)
                    {
                        GameOverSolo();
                        break;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Render.RenderSoloTurn(response, MyCanvas, txtScore);
                        score = response[5].ToString();
                    });
                }
            });
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

        private void GameOverSolo()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var answer = MessageBox.Show("Время вышло! Игра окончена. Ваш счет: " + score + "\nВыйти из игры?", "Конец игры",
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
            });
        }
    }
}
