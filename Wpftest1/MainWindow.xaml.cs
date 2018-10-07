using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//添加用于Socket的类
using System.Net;
using System.Net.Sockets;

namespace Wpftest1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const int port = 4001;
        const string host = "192.168.0.178";
        TcpClientSocket client = new TcpClientSocket(port ,host);   //  创建TcpClientSocket
        //TcpClient client = new TcpClient();
        Boolean cd=false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonct_Click(object sender, RoutedEventArgs e)
        {

            
                try
                {
                    
                    //创建Socket并连接到服务器

                    Console.WriteLine("Connecting...");
                //client.Connect(ipe); //连接到服务器
                client.Connect();


            }
                catch (ArgumentException ae)
                {
                    Console.WriteLine("argumentNullException:{0}", ae);
                }
                catch (SocketException ae)
                {
                    Console.WriteLine("SocketException:{0}", ae);
                }
            
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //向服务器发送信息

                byte[] bs = { 0xFE, 0xFD, 0x0, 0x8, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x0, 0x0, 0x0, 0x0a, 0xFE, 0xFD, 0x0, 0x8, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x7, 0x7, 0x0, 0x0, 0x0, 0x0b }; //FE - FD - 0 - 8 - 0 - 0 - 0 - 1 - 0 - 1 - 2 - 3 - 4 - 5 - 6 - 7 - F2 - E2 - 28 - 32

                string oo = "";
                foreach (byte r in bs)
                {

                    oo = oo + "-" + r.ToString("X");

                }
                Console.WriteLine("{0}", oo);    //回显服务器的返回信息
                Console.WriteLine("Send message");
                client.socketOne.Send(bs, bs.Length, 0); //发送信息到服务器端
        }
             catch (ArgumentException ae)
            {
                Console.WriteLine("argumentNullException:{0}", ae);
            }
            catch (SocketException ae)
            {
                Console.WriteLine("SocketException:{0}", ae);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Console.WriteLine("Recieving message");
                byte[] recvBytes = new byte[1000];
                int bytes;
                bytes = client.socketOne.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息
                                                                      //recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                Console.WriteLine("client get message");    //回显服务器的返回信息
                string oo = "";
                foreach (byte r in recvBytes)
                {

                    oo = oo + "-" + r.ToString("X");

                }
                Console.WriteLine("{0}", oo);    //回显服务器的返回信息

            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("argumentNullException:{0}", ae);
            }
            catch (SocketException ae)
            {
                Console.WriteLine("SocketException:{0}", ae);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.Title = e.GetPosition(this).ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Console.WriteLine(Thread.GetDomain());
#endif
        }
    }
}
