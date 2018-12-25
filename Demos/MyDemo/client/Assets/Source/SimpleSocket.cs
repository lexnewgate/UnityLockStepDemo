using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SimpleSocket {

    private Socket socketClient;

    public static Action<byte[]> OnReceive;

    // Use this for initialization
    public void Init () {
        Console.WriteLine("Hello World!");
        //创建实例
        socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint point = new IPEndPoint(ip, 2333);
        //进行连接
        socketClient.Connect(point);

        //不停的接收服务器端发送的消息
        //Thread thread = new Thread(Recive);
        //thread.IsBackground = true;
        //thread.Start(socketClient);
        socketClient.BeginReceive(buffer, 0, buffersize, SocketFlags.None, ReceiveCb,null);
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socketClient.EndReceive(ar);
            OnReceive?.Invoke(buffer);
            socketClient.BeginReceive(buffer, 0, buffersize, SocketFlags.None, ReceiveCb,null);

        }
        catch(Exception ex)
        {
            Debug.Log($"{ex.ToString()}");
            socketClient.Close();
        }
    }


    const int buffersize = 1024 * 1024 * 2;
    byte[] buffer = new byte[buffersize];
   

    public void send(byte[] record)
    {
        //var buffter = Encoding.UTF8.GetBytes(record);
        var temp = socketClient.Send(record);
        //Thread.Sleep(1000);
    }
}