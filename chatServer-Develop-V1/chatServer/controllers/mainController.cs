using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace chatServer.controllers
{
    internal class mainController
    {
        public static Hashtable clientList = new Hashtable();
      

        public void mainCaleman()
        {
            //Se crear el socket principal con la direccion local del host del servidor
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Chat Server Started ...." + ipAddress.ToString());
            counter = 0;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();

                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientList.Remove(dataFromClient);
                clientList.Add(dataFromClient, clientSocket);

                //cast message
                Console.WriteLine(dataFromClient + " Joined chat room ");
                handleClient client = new handleClient();
                client.startClientCaleman(clientSocket, dataFromClient, clientList);

                unicast(dataFromClient + " Joined ", dataFromClient, false);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }


        public static void unicast(string msg, string uName, bool flag)
        {
            var Item = clientList.OfType<DictionaryEntry>().Where(x => (x.Key as string).Equals(uName));
            //aqu esta el rolloTcpClient unicastSocket;
            TcpClient unicastSocket;
            unicastSocket = (TcpClient)Item.FirstOrDefault().Value;
            NetworkStream unicastStream = unicastSocket.GetStream();
            Byte[] unicastBytes = null;

            if (flag == true)
            {
                //message sending
                unicastBytes = Encoding.ASCII.GetBytes("Veronnika: " + msg);
            }
            else
            {
                unicastBytes = Encoding.ASCII.GetBytes(uName + ": " +msg);
            }

            unicastStream.Write(unicastBytes, 0, unicastBytes.Length);
            unicastStream.Flush();
        }

        public static void unicastCaleman(string msg, string remitente, string destinatario, bool flag)
        {
            destinatario = destinatario.Substring(0, destinatario.IndexOf("@"));
            var Item = clientList.OfType<DictionaryEntry>().Where(x => (x.Key as string).Equals(destinatario));
            //aqu esta el rolloTcpClient unicastSocket;
            TcpClient unicastSocket;
            unicastSocket = (TcpClient)Item.FirstOrDefault().Value;
            NetworkStream unicastStream = unicastSocket.GetStream();
            Byte[] unicastBytes = null;

            if (flag == true)
            {
                //message sending
                unicastBytes = Encoding.ASCII.GetBytes("Veronnika: " + msg);
            }
            else
            {
                unicastBytes = Encoding.ASCII.GetBytes(remitente + ":" + msg + "$");
            }

            unicastStream.Write(unicastBytes, 0, unicastBytes.Length);
            unicastStream.Flush();
        }


    }
}
