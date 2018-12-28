using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

class VirtualServer : Singleton<VirtualServer>, IVirtualServer
{
    bool m_BattleStart = false;


   
    //public List<int> clientIDs = new List<int>();

    //protected override void Awake()
    //{
    //    base.Awake();
    //    Debug.Log("VirtualServerStarted!");
    //}

    //public void Init()
    //{

    //}

    //public void OnClientConnected(int clientID)
    //{
    //    Debug.Log($"client {clientID} connected ");
    //    clientIDs.Add(clientID);

    //    if (CheckAllReadyForGame())
    //    {
    //        NotifyGameStart();
    //    }
    //}

    //bool CheckAllReadyForGame()
    //{
    //    return clientIDs.Count == VirtualManager.Instance.numberOfPlayers;
    //}

    //void NotifyGameStart()
    //{
    //    var gameStartAction = new GameStartAction();
    //    gameStartAction.clientPosDict = new Dictionary<int, Vector3>();
    //    foreach (var clientid in clientIDs)
    //    {
    //        gameStartAction.clientPosDict[clientid] = new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10));
    //    }
    //    BroadCastGeneralAction(gameStartAction);
    //}

    //public void BroadCastGeneralAction(IAction action)
    //{
    //    foreach (var clientid in clientIDs)
    //    {
    //        VirtualManager.Instance.SendToClientGeneralAction(clientid, action);
    //    }
    //}

    //public void RelayLockStepActionToOthers(int lockStepTurn, int playerId, IAction action)
    //{
    //    foreach (var clientid in clientIDs)
    //    {
    //        if (clientid != playerId)
    //        {
    //            VirtualManager.Instance.SendToClientLockStepAction(clientid, lockStepTurn, playerId, action);
    //        }
    //    }
    //}

    Dictionary<int, bool> m_PlayerReadyDict= new Dictionary<int, bool>();

    public void Init()
    {
        Debug.Log("Server Started!");
    }

    void BroadcastGeneralAction(IGeneralAction action)
    {
        throw new NotImplementedException();
    }

    void IVirtualServer.BroadcastGeneralAction(IGeneralAction action)
    {
        throw new NotImplementedException();
    }

    public bool CheckAllPlayerReady()
    {
        return this.m_PlayerReadyDict.Count == VirtualManager.Instance.NumOfPlayers;
    }


    public void NotifyGameStart()
    {
        throw new NotImplementedException();
    }




    public void OnReceiveClientReady(int clientId)
    {
        this.m_PlayerReadyDict[clientId] = true;
        Debug.Log($"Server received: client{clientId} ready!");
        if(CheckAllPlayerReady())
        {
            NotifyGameStart();   
        }
    }

 

    void RelayLockStepActionToOthers(int playerId, int lockStepTurn, ILockStepAction action)
    {
        throw new NotImplementedException();
    }

    void IVirtualServer.RelayLockStepActionToOthers(int playerId, int lockStepTurn, ILockStepAction action)
    {
        throw new NotImplementedException();
    }
}
