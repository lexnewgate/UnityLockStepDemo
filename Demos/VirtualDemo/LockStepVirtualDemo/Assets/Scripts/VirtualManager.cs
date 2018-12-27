using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class VirtualManager : Singleton<VirtualManager>
{

    public GameObject clientPrefab;
    public int numberOfPlayers=2;
    int clientId = 0;
    public Dictionary<int, VirtualClient> virtualClientDict= new Dictionary<int, VirtualClient>();

    protected override void Awake()
    {
        base.Awake();
        VirtualServer.Instance.Init();
        GameObject.Find("Canvas/Button").GetComponent<Button>().onClick.AddListener(AddClient);

    }



    #region Network
    public void ConnectToServer(int clientId)
    {
        VirtualServer.Instance.OnClientConnected(clientId);
    }

    public void SendToServerLockStepAction(int lockStepTurn,int playerId,IAction action)
    {
        VirtualServer.Instance.RelayLockStepActionToOthers(lockStepTurn, playerId, action);
    }

    public void SendToClientGeneralAction(int clientId,IAction action)
    {
        virtualClientDict[clientId].OnReceiveGeneralAction(action);
    }

    public void SendToClientLockStepAction(int clientid,int lockStepTurn,int playerid, IAction action)
    {
        virtualClientDict[clientid].OnReceiveLockStepAction(lockStepTurn, playerid, action);
    }
    #endregion



    public void AddClient()
    {
        var client = GameObject.Instantiate<GameObject>(clientPrefab);
        var virtualClient= client.GetComponent<VirtualClient>();
        virtualClient.ID = clientId++;
        virtualClientDict[virtualClient.ID] = virtualClient;
        virtualClient.Init();
        var ClientsRoot = GameObject.Find("Clients");
        if (ClientsRoot == null)
        {
            ClientsRoot = new GameObject("Clients");
        }
        client.transform.SetParent(ClientsRoot.transform);
    }


}
