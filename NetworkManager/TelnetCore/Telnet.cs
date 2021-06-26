using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using NetworkManager.UI;
using System.Threading.Tasks;

namespace ToolkitUI.TelnetCore
{
    enum Verbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }
    enum Options
    {
        SGA = 3
    }

    public class Telnet
    {
        TcpClient tcpSocket;
        int TimeOutMs = 1000; //base timeout

        public Telnet(string Hostname, int Port)
        {
            try { tcpSocket = new TcpClient(Hostname, Port); }
            catch (Exception exc) { MessageBox.Show("Error: " + exc.Message); return; }
        }

        public string Login(string Username, string Password, int LoginTimeOutMs)
        {
            int oldTimeOutMs = TimeOutMs;
            TimeOutMs = LoginTimeOutMs;
            string s = Read();

            try
            {
                SendCommand(Username);
                s += Read();

                SendCommand(Password);
                s += Read();

                TimeOutMs = oldTimeOutMs;
            }
            catch (Exception exc) { MessageBox.Show("Error: " + exc.Message); }

            return s;
        }

        public void SendCommand(string cmd) => Write(cmd + "\n");

        public void Write(string cmd)
        {
            if (!tcpSocket.Connected) return;

            byte[] buf = Encoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            if (!tcpSocket.Connected) return null;

            StringBuilder sb = new StringBuilder();
            do
            {
                ParseTelnet(sb);
                Thread.Sleep(TimeOutMs);
            }
            while (tcpSocket.Available > 0);
            return sb.ToString().Replace(" [K", "");
        }

        public bool IsConnected
        {
            get { return tcpSocket.Connected; }
        }

        public void ParseTelnet(StringBuilder sb)
        {
            while (tcpSocket.Available > 0)
            {
                int input = tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = tcpSocket.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }
    }
}
