using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.Windows.Threading;
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
        private System.Timers.Timer aTimer = null;
        private delegate void TimerDispatcherDelegate();
        
        
        bool socketConnected;
        byte canBS0, canBS1;
        byte[] bs = { 0xFE, 0xFD, 0x0, 0x8, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0a }; //FE - FD - 0 - 8 - 0 - 0 - 0 - 1 - 0 - 1 - 2 - 3 - 4 - 5 - 6 - 7 - F2 - E2 - 28 - 32
        TcpClientSocket client;// = new TcpClientSocket(port, host);   //  创建TcpClientSocket
                                                                    //TcpClient client = new TcpClient();
        Thread tR;

        public MainWindow()
        {
            InitializeComponent();
            int port;
            int.TryParse(textBox_port.Text, out port);
            client = new TcpClientSocket(port, textBox_host.Text);
            socketConnected = false;
            //定时查询-定时器
            aTimer = new System.Timers.Timer(100);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 100;//100ms

        }

                private void OnTimedEvent(object sender, EventArgs e)
         {
             this.Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(updateUI));
            
        }
 
         private void updateUI()
         {
            if (socketConnected) sendSocket();//执行查询
        }

        private void sendSocket()
        {
                     
                try
                {
                    //向服务器发送信息

                    if (checkBox1.IsChecked == true) canBS0 |= 1;
                    else canBS0 &= 0xfe;
                    if (checkBox2.IsChecked == true) canBS0 |= 2;
                    else canBS0 &= 0xfd;
                    if (checkBox3.IsChecked == true) canBS0 |= 4;
                    else canBS0 &= 0xfb;
                    if (checkBox4.IsChecked == true) canBS0 |= 8;
                    else canBS0 &= 0xf7;
                    if (checkBox5.IsChecked == true) canBS0 |= 16;
                    else canBS0 &= 0xef;
                    if (checkBox6.IsChecked == true) canBS0 |= 32;
                    else canBS0 &= 0xdf;
                    if (checkBox7.IsChecked == true) canBS0 |= 64;
                    else canBS0 &= 0xbf;
                    if (checkBox8.IsChecked == true) canBS0 |= 128;
                    else canBS0 &= 0x7f;
                    if (checkBox9.IsChecked == true) canBS1 |= 1;
                    else canBS1 &= 0xfe;
                    if (checkBox10.IsChecked == true) canBS1 |= 2;
                    else canBS1 &= 0xfd;
                    if (checkBox11.IsChecked == true) canBS1 |= 4;
                    else canBS1 &= 0xfb;
                    if (checkBox12.IsChecked == true) canBS1 |= 8;
                    else canBS1 &= 0xf7;
                    if (checkBox13.IsChecked == true) canBS1 |= 16;
                    else canBS1 &= 0xef;
                    if (checkBox14.IsChecked == true) canBS1 |= 32;
                    else canBS1 &= 0xdf;
                    if (checkBox15.IsChecked == true) canBS1 |= 64;
                    else canBS1 &= 0xbf;
                    if (checkBox16.IsChecked == true) canBS1 |= 128;
                    else canBS1 &= 0x7f;

                    bs[8] = canBS0;
                    bs[9] = canBS1;
                    int des = comboBox_destination.SelectedIndex;
                    bs[6] = (byte)(des / 256 % 256);
                    bs[7] = (byte)(des % 256);
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
        private void buttonct_Click(object sender, RoutedEventArgs e)
        {

            if(socketConnected)
            {
                tR.Abort(client);
                client.socketOne.Shutdown(SocketShutdown.Both);
                client.socketOne.Disconnect(true);
                buttonct.Content = "Connect";
                socketConnected = false;
                this.labelConnected1.Content = "#FFC73B3B";
            }
            else
                try
                {

                    //连接到服务器

                    Console.WriteLine("Connecting...");
                    client.Connect(); //连接到服务器
                                      //Thread tC=new Thread(new ThreadStart(client.Connect));
                                      // tC.Start();
                    socketConnected = true;
                    this.labelConnected1.Content = "#FF3EC30D";
                    buttonct.Content = "Disconnect";
                    aTimer.Start();
                    if (socketConnected)
                    {
                        tR = new Thread(new ParameterizedThreadStart(sockRecieve));
                        tR.Start(client);
                    }

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

       /* private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //向服务器发送信息

                bs[8] = canBS0;
                bs[9] = canBS1;
                int des= comboBox_destination.SelectedIndex;
                bs[6] = (byte)(des /256%256);
                bs[7] = (byte)(des % 256);
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
        */

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
        void sockRecieve(object TcpCS)
        {
            try
            {
                while (1 == 1)
                {
                    Console.WriteLine("Recieving message");
                    byte[] recvBytes = new byte[20];
                    int bytes;
                    TcpClientSocket tcpc = (TcpClientSocket)TcpCS;
                    bytes = tcpc.socketOne.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息
                                                                                       //recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    this.Dispatcher.Invoke(new Action(() => {


                        int src = comboBox_destination.SelectedIndex;
                        byte ox = recvBytes[0];
                        if (recvBytes[6] == (byte)(src / 256 % 256) && recvBytes[7] == (byte)(src % 256))
                        {
                            for (int i = 1; i < 19; i++)
                            {
                                ox ^= recvBytes[i];

                            }
                            if (recvBytes[19] == ox)
                            {

                                if ((recvBytes[8] & 1) == 0) this.checkBox_O1.IsChecked = false; else this.checkBox_O1.IsChecked = true;
                                if ((recvBytes[8] & 2) == 0) this.checkBox_O2.IsChecked = false; else this.checkBox_O2.IsChecked = true;
                                if ((recvBytes[8] & 4) == 0) this.checkBox_O3.IsChecked = false; else this.checkBox_O3.IsChecked = true;
                                if ((recvBytes[8] & 8) == 0) this.checkBox_O4.IsChecked = false; else this.checkBox_O4.IsChecked = true;
                                if ((recvBytes[8] & 16) == 0) this.checkBox_O5.IsChecked = false; else this.checkBox_O5.IsChecked = true;
                                if ((recvBytes[8] & 32) == 0) this.checkBox_O6.IsChecked = false; else this.checkBox_O6.IsChecked = true;
                                if ((recvBytes[8] & 64) == 0) this.checkBox_O7.IsChecked = false; else this.checkBox_O7.IsChecked = true;
                                if ((recvBytes[8] & 128) == 0) this.checkBox_O8.IsChecked = false; else this.checkBox_O8.IsChecked = true;
                                if ((recvBytes[9] & 1) == 0) this.checkBox_O9.IsChecked = false; else this.checkBox_O9.IsChecked = true;
                                if ((recvBytes[9] & 2) == 0) this.checkBox_O10.IsChecked = false; else this.checkBox_O10.IsChecked = true;
                                if ((recvBytes[9] & 4) == 0) this.checkBox_O11.IsChecked = false; else this.checkBox_O11.IsChecked = true;
                                if ((recvBytes[9] & 8) == 0) this.checkBox_O12.IsChecked = false; else this.checkBox_O12.IsChecked = true;
                                if ((recvBytes[9] & 16) == 0) this.checkBox_O13.IsChecked = false; else this.checkBox_O13.IsChecked = true;
                                if ((recvBytes[9] & 32) == 0) this.checkBox_O14.IsChecked = false; else this.checkBox_O14.IsChecked = true;
                                if ((recvBytes[9] & 64) == 0) this.checkBox_O15.IsChecked = false; else this.checkBox_O15.IsChecked = true;
                                if ((recvBytes[9] & 128) == 0) this.checkBox_O16.IsChecked = false; else this.checkBox_O16.IsChecked = true;
                                if ((recvBytes[10] & 1) == 0) this.checkBox_O17.IsChecked = false; else this.checkBox_O17.IsChecked = true;
                                if ((recvBytes[10] & 2) == 0) this.checkBox_O18.IsChecked = false; else this.checkBox_O18.IsChecked = true;
                                if ((recvBytes[10] & 4) == 0) this.checkBox_O19.IsChecked = false; else this.checkBox_O19.IsChecked = true;
                                if ((recvBytes[10] & 8) == 0) this.checkBox_O20.IsChecked = false; else this.checkBox_O20.IsChecked = true;
                                if ((recvBytes[10] & 16) == 0) this.checkBox_O21.IsChecked = false; else this.checkBox_O21.IsChecked = true;
                                if ((recvBytes[10] & 32) == 0) this.checkBox_O22.IsChecked = false; else this.checkBox_O22.IsChecked = true;
                                if ((recvBytes[10] & 64) == 0) this.checkBox_O23.IsChecked = false; else this.checkBox_O23.IsChecked = true;
                                if ((recvBytes[10] & 128) == 0) this.checkBox_O24.IsChecked = false; else this.checkBox_O24.IsChecked = true;
                                if ((recvBytes[11] & 1) == 0) this.checkBox_O25.IsChecked = false; else this.checkBox_O25.IsChecked = true;
                                if ((recvBytes[11] & 2) == 0) this.checkBox_O26.IsChecked = false; else this.checkBox_O26.IsChecked = true;
                                if ((recvBytes[11] & 4) == 0) this.checkBox_O27.IsChecked = false; else this.checkBox_O27.IsChecked = true;
                                if ((recvBytes[11] & 8) == 0) this.checkBox_O28.IsChecked = false; else this.checkBox_O28.IsChecked = true;
                                if ((recvBytes[11] & 16) == 0) this.checkBox_O29.IsChecked = false; else this.checkBox_O29.IsChecked = true;
                                if ((recvBytes[11] & 32) == 0) this.checkBox_O30.IsChecked = false; else this.checkBox_O30.IsChecked = true;
                                if ((recvBytes[11] & 64) == 0) this.checkBox_O31.IsChecked = false; else this.checkBox_O31.IsChecked = true;
                                if ((recvBytes[11] & 128) == 0) this.checkBox_O32.IsChecked = false; else this.checkBox_O32.IsChecked = true;
                            }

                        }

                    }));


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

    }
}
