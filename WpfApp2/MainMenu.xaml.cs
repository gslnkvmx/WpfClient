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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        ThConsoleClient.CommunicationHandler handler;
        public MainMenu()
        {
            InitializeComponent();
            // Create a new UdpClient
            var udpClient = new UdpClient();
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2004);
            handler = new ThConsoleClient.CommunicationHandler(udpClient, serverEndpoint);
            if (handler.SayHello()) Console.WriteLine("Connection opened!");
        }

        private void StartSoloGameButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SoloGamePage());
        }

        private void StartDuoGameButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DuoHostGamePage());
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
                NavigationService.Navigate(new DuoConnectedGamePage(gameId));
            }
            else
            {
                MessageBox.Show("Invalid Game ID. Please enter a number.");
            }
        }
    }
}
