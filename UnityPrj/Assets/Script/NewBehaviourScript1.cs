using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NewBehaviourScript1
{
    public static Socket m_socket;
    static Thread threadReceive;
    public static byte[] recives;

    public static void Connect()
    {
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_socket.Connect("127.0.0.1", 20001);
        //开启一个新的线程不停的接收服务器发送消息的线程
        threadReceive = new Thread(new ThreadStart(Receive));
        //设置为后台线程
        threadReceive.IsBackground = true;
        threadReceive.Start();
    }
    public static void Receive()
    {
        while (true)
        {
            if (m_socket.Connected)
            {
                byte[] recive = new byte[1024];
                int count = m_socket.Receive(recive);
                if (count > 0)
                {
                    recives = recive;
                }
                System.Console.Out.WriteLine("aaaaaaaaaaa" + count);
            }
        }
    }
}
