using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerSocket
{
	class Program
	{
		static void Main(string[] args)
		{
            Serv serv = new Serv();
            serv.Start("127.0.0.1", 2333);

            while(true)
            {
                string str = Console.ReadLine();
                if(str.Equals("quit"))
                {
                    return;
                }
            }
		}
	}
}