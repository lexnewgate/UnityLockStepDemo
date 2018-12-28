using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class VirtualManager : Singleton<VirtualManager>, IVirtualManager
{
    public GameObject m_ClientsRoot;
    public GameObject m_ClientAssets;


    private int m_clientId = 0;

    protected override void Awake()
    {
        base.Awake();
        InitVirtualEnv();
    }

    void InitVirtualEnv()
    {
        CreateServer();
        CreateClient(this.m_clientId++);
        CreateClient(this.m_clientId++);
    }

    //public GameObject clientPrefab;
    //public int numberOfPlayers=2;
    //int clientId = 0;
    //public Dictionary<int, VirtualClient> virtualClientDict= new Dictionary<int, VirtualClient>();

    //protected override void Awake()
    //{
    //    base.Awake();
    //    VirtualServer.Instance.Init();
    //    GameObject.Find("Canvas/Button").GetComponent<Button>().onClick.AddListener(AddClient);

    //}



    //#region Network
    //public void ConnectToServer(int clientId)
    //{
    //    VirtualServer.Instance.OnClientConnected(clientId);
    //}

    //public void SendToServerLockStepAction(int lockStepTurn,int playerId,IAction action)
    //{
    //    VirtualServer.Instance.RelayLockStepActionToOthers(lockStepTurn, playerId, action);
    //}

    //public void SendToClientGeneralAction(int clientId,IAction action)
    //{
    //    virtualClientDict[clientId].OnReceiveGeneralAction(action);
    //}

    //public void SendToClientLockStepAction(int clientid,int lockStepTurn,int playerid, IAction action)
    //{
    //    virtualClientDict[clientid].OnReceiveLockStepAction(lockStepTurn, playerid, action);
    //}
    //#endregion



    //public void AddClient()
    //{
    //    var client = GameObject.Instantiate<GameObject>(clientPrefab);
    //    var virtualClient= client.GetComponent<VirtualClient>();
    //    virtualClient.ID = clientId++;
    //    virtualClientDict[virtualClient.ID] = virtualClient;
    //    virtualClient.Init();
    //    var ClientsRoot = GameObject.Find("Clients");
    //    if (ClientsRoot == null)
    //    {
    //        ClientsRoot = new GameObject("Clients");
    //    }
    //    client.transform.SetParent(ClientsRoot.transform);
    //}
    public int NumOfPlayers { get; private set; } = 2;

    public void CreateClient(int clientId)
    {
        var clientGo = new GameObject($"{clientId}");
        clientGo.transform.SetParent(this.m_ClientsRoot.transform);
        var virtualClient=clientGo.AddComponent<VirtualClient>();
        var clientAssetsGo = GameObject.Instantiate<GameObject>(this.m_ClientAssets);

        //帧同步就像单机,load资源等操作是无差的.由虚拟导致的差异化这里处理,保持Client干净
         var camera=clientAssetsGo.transform.FindDeepChild("Main Camera").GetComponent<Camera>();
        if(clientId==0) //屏幕左边
        {
            camera.rect = new Rect(0, 0, 0.5f, 1);
        }
        else if(clientId==1)//屏幕右边
        {

            camera.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
        else
        {
            Debug.LogError("暂只支持两个虚拟客户端");
        }

        virtualClient.Init(clientId, clientAssetsGo);
    }

    public void CreateServer()
    {
        VirtualServer.Instance.Init();
    }

    public void SendReadyToServer(int clientId)
    {
        VirtualServer.Instance.OnReceiveClientReady(clientId);
    }
}
