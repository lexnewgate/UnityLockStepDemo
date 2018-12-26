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
    public int clientId = 0;
    public Dictionary<int, VirtualClient> virtualClientDict= new Dictionary<int, VirtualClient>();

    protected override void Awake()
    {
        base.Awake();
        VirtualServer.Instance.Init();

        GameObject.Find("Canvas/Button").GetComponent<Button>().onClick.AddListener(AddClient);

    }

    public void ConnectToServer(int clientId)
    {
        VirtualServer.Instance.OnClientConnected(clientId);
    }

    public void SendToServer(IAction action)
    {

    }

    public void SendToClientGeneralAction(int clientId,IAction action)
    {
        virtualClientDict[clientId].OnReceiveGeneralAction(action);
    }

    public void SendToClientLockStepAction(int clientId, IAction action)
    {

    }


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
