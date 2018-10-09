using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        byte canBS0, canBS1;
        byte[] bs = { 0xFE, 0xFD, 0x0, 0x8, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x0, 0x0, 0x0, 0x0a }; //FE - FD - 0 - 8 - 0 - 0 - 0 - 1 - 0 - 1 - 2 - 3 - 4 - 5 - 6 - 7 - F2 - E2 - 28 - 32
        TcpClientSocket client = new TcpClientSocket(port, host);   //  创建TcpClientSocket
                                                                    //TcpClient client = new TcpClient();
        Thread tR = new Thread(new ParameterizedThreadStart(sockRecieve));

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
                client.Connect(); //连接到服务器
                                  //Thread tC=new Thread(new ThreadStart(client.Connect));
                                  // tC.Start();
                this.labelConnected1.Content = "#FF3EC30D";

                //tR.Start(client);


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

                bs[8] = canBS0;
                bs[9] = canBS1;
                byte ox = bs[0];
                for (int i = 1; i < 19; i++)
                {
                    ox ^= bs[i];

                }
                bs[19] = ox;

                string oo = "";
                foreach (byte r in bs)
                {

                    oo = oo + "-" + r.ToString("X");

                }
                Console.WriteLine("{0}", oo);    //回显服务器的返回信息
                Console.WriteLine("Send message");
                client.socketOne.Send(bs, bs.Length, SocketFlags.None);
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
        /*
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
        }*/
        static void sockRecieve(object TcpCS)
        {
            try
            {
                while (1 == 1)
                {
                    Console.WriteLine("Recieving message");
                    byte[] recvBytes = new byte[1000];
                    int bytes;
                    TcpClientSocket tcpc = (TcpClientSocket)TcpCS;
                    bytes = tcpc.socketOne.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息
                                                                                       //recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    Console.WriteLine("client get message");    //回显服务器的返回信息
                    string oo = "";
                    foreach (byte r in recvBytes)
                    {

                        oo = oo + "-" + r.ToString("X");

                    }
                    Console.WriteLine("{0}", oo);    //回显服务器的返回信息
                }
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


        private void labelConnected_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((string)this.labelConnected1.Content == "#FF3EC30D") tR.Start(client);
                // else tR.Abort();
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("argumentNullException:{0}", ae);
            }
        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox1.IsChecked == true) canBS0 |= 1;
            else canBS0 &= 0xfe;
        }

        private void checkBox2_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox2.IsChecked == true) canBS0 |= 2;
            else canBS0 &= 0xfd;
        }

        private void checkBox3_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox3.IsChecked == true) canBS0 |= 4;
            else canBS0 &= 0xfb;
        }
        private void checkBox4_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox4.IsChecked == true) canBS0 |= 8;
            else canBS0 &= 0xf7;
        }
        private void checkBox5_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox5.IsChecked == true) canBS0 |= 16;
            else canBS0 &= 0xef;
        }
        private void checkBox6_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox6.IsChecked == true) canBS0 |= 32;
            else canBS0 &= 0xdf;
        }
        private void checkBox7_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox7.IsChecked == true) canBS0 |= 64;
            else canBS0 &= 0xbf;
        }
        private void checkBox8_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox8.IsChecked == true) canBS0 |= 128;
            else canBS0 &= 0x7f;
        }
        private void checkBox9_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox9.IsChecked == true) canBS1 |= 1;
            else canBS1 &= 0xfe;
        }

        private void checkBox10_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox10.IsChecked == true) canBS1 |= 2;
            else canBS1 &= 0xfd;
        }

        private void checkBox11_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox11.IsChecked == true) canBS1 |= 4;
            else canBS1 &= 0xfb;
        }
        private void checkBox12_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox12.IsChecked == true) canBS1 |= 8;
            else canBS1 &= 0xf7;
        }
        private void checkBox13_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox13.IsChecked == true) canBS1 |= 16;
            else canBS1 &= 0xef;
        }
        private void checkBox14_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox14.IsChecked == true) canBS1 |= 32;
            else canBS1 &= 0xdf;
        }
        private void checkBox15_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox15.IsChecked == true) canBS1 |= 64;
            else canBS1 &= 0xbf;
        }

 

        private void checkBox16_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox16.IsChecked == true) canBS1 |= 128;
            else canBS1 &= 0x7f;
        }
    }
}
