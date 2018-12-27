using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

class VirtualServer:Singleton<VirtualServer>
{
    public List<int> clientIDs=new List<int>();

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("VirtualServerStarted!");
    }

    public void Init()
    {
        
    }

    public void OnClientConnected(int clientID)
    {
        Debug.Log($"client {clientID} connected ");
        clientIDs.Add(clientID);

        if(clientIDs.Count==VirtualManager.Instance.numberOfPlayers)
        {
            var gameStartAction = new GameStartAction();
            gameStartAction.clientPosDict = new Dictionary<int, Vector3>();
            foreach(var clientid in clientIDs)
            {
                gameStartAction.clientPosDict[clientid] = new Vector3(Random.Range(0,10),0,Random.Range(0,10));
            }
            BroadCastGeneralAction(gameStartAction);
        }
    }

    public void BroadCastGeneralAction(IAction action)
    {
        foreach(var clientid in clientIDs)
        {
            VirtualManager.Instance.SendToClientGeneralAction(clientid, action);
        }
    }

   public void OnReceiveLockStepAction(int lockStepTurn, int playerId, IAction action)
    {
        foreach(var clientid in clientIDs)
        {
            if(clientid!=playerId)
            {
                VirtualManager.Instance.SendToClientLockStepAction(clientid,lockStepTurn, playerId, action);
            }
        }

    }


}
