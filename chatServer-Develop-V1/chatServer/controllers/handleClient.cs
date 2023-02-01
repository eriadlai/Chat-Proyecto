using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace chatServer.controllers
{
    internal class handleClient
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;

       

        public void startClientCaleman(TcpClient inClientSocket, string clientNo, Hashtable cList)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clientNo;
            this.clientsList = cList;

            Thread ctThread = new Thread(doChatCaleman);
            ctThread.Start();
        }
        private void doChatCaleman()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
            string dataFromClient = null;
            string msg = null;
            string clDestinatario = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);

                    clDestinatario = dataFromClient.Substring(dataFromClient.IndexOf("$") + 1, dataFromClient.IndexOf("@")).Trim();
                    msg = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    

                    Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    //botLogic(dataFromClient.ToLower(), clNo);
                    mainController.unicastCaleman(msg, clNo, clDestinatario, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }//end while
        }
       

        
    }
}
