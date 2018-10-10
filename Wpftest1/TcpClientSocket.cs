using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//添加用于Socket的类
using System.Net;
using System.Net.Sockets;

namespace Wpftest1
{
    class TcpClientSocket
    {
        int port;
        string host;
        public Socket socketOne { get; set; }
        public TcpClientSocket(int inport, string inhost)
        {
            port = inport;
            host = inhost;



        }
        public void Connect()
        {

            //创建Socket
            socketOne = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //  创建Socket
            //创建终结点EndPoint
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);   //把ip和端口转化为IPEndPoint的实例
            Console.WriteLine("Connecting...");

            socketOne.Connect(ipe); //连接到服务器
        }



    }
    
}
