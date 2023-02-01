using System.Net;
using System.Net.Sockets;

namespace chatClient
{
    public partial class Form1 : Form
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            readData = "connecting";
            msg();
            try
            {
                IPAddress ipAddress = IPAddress.Parse(tbDir.Text);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8888);
                clientSocket.Connect(ipEndPoint);
                serverStream = clientSocket.GetStream();

                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(tbUsuario.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Thread ctThread = new Thread(getMessage);
                ctThread.Start();
                readData = "Conected to Chat Server ...";
                tbDestiny.Enabled = true;
            }
            catch (Exception ex) 
            { 
                readData = ex.Message;
            } 
            
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(tbMensaje.Text + "$" + tbDestiny.Text + "@");

            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;
                //
                msg();
            }
        }

        private void msg()
        {
            string decodemsg;
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
            {
                if (readData.Contains("$"))
                {
                    decodemsg = readData.Substring(readData.IndexOf(":") + 1, readData.IndexOf("$")).Trim();
                    decodemsg = decodemsg.Substring(0, decodemsg.IndexOf("$"));
                    rtbLogMensajes.Text = rtbLogMensajes.Text + Environment.NewLine + " >> " + readData.Substring(0, readData.IndexOf(":")) + ": " + decodemsg;
                }
                else
                {
                    rtbLogMensajes.Text = rtbLogMensajes.Text + Environment.NewLine + " >> " + readData;
                }
            }
        }

        private void tbMensaje_TextChanged(object sender, EventArgs e)
        {

        }

    }
}