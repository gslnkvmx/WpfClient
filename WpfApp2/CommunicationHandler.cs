using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ThConsoleClient
{
    internal class CommunicationHandler
    {
        private IPEndPoint _serverEndpoint;
        private UdpClient _udpClient;
        public CommunicationHandler(UdpClient udpClient, IPEndPoint serverEndpoint) 
        {
            _serverEndpoint = serverEndpoint;
            _udpClient = udpClient;
        }

        public void SendRequest(string request) 
        {
            var bytes = Encoding.ASCII.GetBytes(request);
            _udpClient.Send(bytes, bytes.Length, _serverEndpoint);
        }

        public string ReciveResponseString() 
        {
            var response = _udpClient.Receive(ref _serverEndpoint);
            Console.WriteLine(Encoding.ASCII.GetString(response));
            return Encoding.ASCII.GetString(response);
        }

        public byte[] ReciveResponseBytes()
        {
            var response = _udpClient.Receive(ref _serverEndpoint);
            return response;
        }

        public bool SayHello()
        {
            SendRequest("TH 0 0 hello");
            if (ReciveResponseString() == "Threasure hunters sever is ready!") return true;
            return false;
        }

       /* public void GoCommand(ConsoleKey key, byte gid)
        {
            string gidS = gid.ToString();
            switch (key) {
                case ConsoleKey.UpArrow:
                    SendRequest("TH " + gidS + " go u");
                    break;
                case ConsoleKey.DownArrow:
                    SendRequest("TH " + gidS + " go d");
                    break;
                case ConsoleKey.LeftArrow:
                    SendRequest("TH " + gidS + " go l");
                    break;
                case ConsoleKey.RightArrow:
                    SendRequest("TH " + gidS + " go r");
                    break;

            }
        }*/

        public bool StartSoloGame()
        {
            //SendRequest("TH 0 start solo");

            var response = ReciveResponseBytes();

            if (response[2] == 1)
            {
                Console.Clear();

                var gid = response[3];

                //Render.RenderInitSoloGame(response);

                Task.Run(() =>
                {
                    while (true)
                    {
                        var response = ReciveResponseBytes();
                        //Console.SetCursorPosition(20, 20);
                        //Console.WriteLine($"X: {response[3]}, Y: {response[4]}");
                        //Render.RenderTurn(response);
                    }

                });

                    /*while (true)
                    {
                        GoCommand(Console.ReadKey().Key, gid);
                    }*/

                return true;
            }
            return false;
        }
    }
}
