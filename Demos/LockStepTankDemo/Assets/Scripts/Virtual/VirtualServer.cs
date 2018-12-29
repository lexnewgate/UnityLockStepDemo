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

    PlayerTransFixData[] m_PlayerDatas=new PlayerTransFixData[4] {
        new PlayerTransFixData{
            Position = new FixVector3((Fix64)40,(Fix64)(-42),(Fix64)55),
            Rotation =new FixVector3((Fix64)0,(Fix64)180,(Fix64)0)},
        new PlayerTransFixData{
            Position = new FixVector3((Fix64)56,(Fix64)(-42),(Fix64)20),
            Rotation =new FixVector3((Fix64)0,(Fix64)0,(Fix64)0)},
        new PlayerTransFixData{
            Position = new FixVector3((Fix64)19,(Fix64)(-42),(Fix64)55),
            Rotation =new FixVector3((Fix64)0,(Fix64)180,(Fix64)0)},
        new PlayerTransFixData{
            Position = new FixVector3((Fix64)6,(Fix64)(-42),(Fix64)4),
            Rotation =new FixVector3((Fix64)0,(Fix64)0,(Fix64)0)},
    };

   
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

    public void BroadcastGeneralAction(IGeneralAction action)
    {
        VirtualManager.Instance.SendGeneralActionToClients(action);
    }

    public bool CheckAllPlayerReady()
    {
        return this.m_PlayerReadyDict.Count == VirtualManager.Instance.NumOfPlayers;
    }

    Tuple<int,int>GetTwoDistinctIndex()
    {
        Random.InitState((int)(Time.time * 1000));
        int first = Random.Range(0, 4);
        int temp = Random.Range(0, 4);
        int second= (temp == first) ? (temp + 1)%4 : temp;
        return new Tuple<int, int>(first, second);
    }

    public void NotifyGameStart()
    {
        var first_second = GetTwoDistinctIndex();

        var playerInitTransDatas = new Dictionary<int, PlayerTransFixData>
        {
            { 0, this.m_PlayerDatas[first_second.Item1]},
            {1, this.m_PlayerDatas[first_second.Item2] }
        };

        BroadcastGeneralAction(new GameStartAction {
            PlayerInitTransDatas = playerInitTransDatas 
        });
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

    public void RelayLockStepActionToOthers(int playerId, int lockStepTurn, ILockStepAction action)
    {
        throw new NotImplementedException();
    }
}


