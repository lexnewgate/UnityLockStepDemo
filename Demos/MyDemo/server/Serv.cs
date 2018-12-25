using System;
using System.Net;
using System.Net.Sockets;

public class Serv
{
	//监听嵌套字
	public Socket listenfd;
	//客户端链接
	public Conn[] conns;
	//最大链接数
	public int maxConn = 50;
	
	//获取链接池索引，返回负数表示获取失败
	public int NewIndex()
	{
		if (conns == null)
			return -1;
		for (int i = 0; i < conns.Length; i++)
		{
			if (conns[i] == null)
			{
				conns[i] = new Conn();
				return i;
			}
			else if (conns[i].isUse == false)
			{
				return i;
			}
		}
		return -1;
	}
	
	//开启服务器
	public void Start(string host, int port)
	{
		//链接池
		conns = new Conn[maxConn];
		for (int i = 0; i < maxConn; i++)
		{
			conns[i] = new Conn();
		}
		//Socket
		listenfd = new Socket(AddressFamily.InterNetwork,
		                      SocketType.Stream, ProtocolType.Tcp);
		//Bind
		IPAddress ipAdr = IPAddress.Parse(host);
		IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
		listenfd.Bind(ipEp);
		//Listen
		listenfd.Listen(maxConn);
		//Accept
		listenfd.BeginAccept(AcceptCb, null);
		Console.WriteLine("[服务器]启动成功");
	}


	//Accept回调
	private void AcceptCb(IAsyncResult ar)
	{
		try
		{
			Socket socket = listenfd.EndAccept(ar);
			int index = NewIndex();
			
			if(index < 0)
			{
				socket.Close();
				Console.Write("[警告]链接已满");
			}
			else
			{
				Conn conn = conns[index];
				conn.Init(socket);
				string adr = conn.GetAdress();
				Console.WriteLine("客户端连接 [" + adr　+"] conn池ID：" + index);
				conn.socket.BeginReceive(conn.readBuff, 
				                         conn.buffCount, conn.BuffRemain(),
				                         SocketFlags.None, ReceiveCb, conn);
			}
			listenfd.BeginAccept(AcceptCb,null);
		}
		catch(Exception e)
		{
			Console.WriteLine("AcceptCb失败:" + e.Message);
		}
	}

	private void ReceiveCb(IAsyncResult ar)
	{
		Conn conn = (Conn)ar.AsyncState;
		try
		{
			int count = conn.socket.EndReceive(ar);
			//关闭信号
			if(count <= 0)
			{
				Console.WriteLine("收到 [" + conn.GetAdress()　+"] 断开链接");
				conn.Close();
				return;
			}
			//数据处理
			//string str = System.Text.Encoding.UTF8.GetString(conn.readBuff, 0, count);
			//Console.WriteLine("收到 [" + conn.GetAdress()　+"] 数据：" + str);
			//str = conn.GetAdress() + ":" + str;
			//byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
			////广播
			for(int i=0;i < conns.Length; i++)
			{
				if(conns[i] == null)
					continue;
				if(!conns[i].isUse)
					continue;
				Console.WriteLine("将消息转播给 " + conns[i].GetAdress());
				conns[i].socket.Send(conn.readBuff,count,SocketFlags.None);
			}
			//继续接收	
			conn.socket.BeginReceive(conn.readBuff, 
			                         conn.buffCount, conn.BuffRemain(), 
			                         SocketFlags.None, ReceiveCb, conn);
		}
		catch(Exception e)
		{
			Console.WriteLine("收到 [" + conn.GetAdress()　+"] 断开链接");
			conn.Close();
		}
	}
}