using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
///  这里作为虚拟管理中心. 
///  作为客户端和服务器的链接通道.
///  
///  1. 初始化客户端和服务器.
///  2. 把网络通信隔离在此处,方便模拟网络的各种情况.
///  3. 客户端和服务端不直接互相调用,而是通过VirtualManager进行通信.
///  4. 此类存储的数据作为已经过通讯手段 双方均有的数据.
///  
/// </summary>

interface IVirtualManager
{
    int NumOfPlayers { get; }
    void CreateServer();
    void CreateClient(int clientId);
    void SendReadyToServer(int clientId);


}
