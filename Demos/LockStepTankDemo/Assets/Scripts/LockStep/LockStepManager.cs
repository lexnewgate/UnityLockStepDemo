using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LockStepManager
{
    //public VirtualClient virtualClient;
    //public LockStepManager(VirtualClient virtualClient)
    //{
    //    this.virtualClient = virtualClient;
    //    this.numberOfPlayers = VirtualManager.Instance.numberOfPlayers;
    //    this.pendingActions = new PendingActions(this);
    //}

    //public int numberOfPlayers;

    //private PendingActions pendingActions;
    ////private ConfirmedActions confirmedActions;

    //private Queue<IAction> actionsToSend=new Queue<IAction>();



    //private float AccumilatedTime = 0f;
    //private float FrameLength = 0.05f; //50 miliseconds
    //private int GameFramesPerLocksetpTurn = 4;
    //private int GameFrame = 0;
    //private int GameFramesPerSecond = 20;
    //public static readonly int FirstLockStepTurnID = 0;
    //public int LockStepTurnID = FirstLockStepTurnID;


    //public void Update()
    //{
    //    if(this.virtualClient.ID==0)
    //    {
    //        if(Input.GetKeyDown(KeyCode.W))
    //        {
    //            AddAction(new ForwardAction { clientID=this.virtualClient.ID});
    //        }
    //    }


    //    //Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength
    //    AccumilatedTime = AccumilatedTime + Time.deltaTime;


    //    //in case the FPS is too slow, we may need to update the game multiple times a frame
    //    while (AccumilatedTime > FrameLength)
    //    {
    //        GameFrameTurn();
    //        AccumilatedTime = AccumilatedTime - FrameLength;
    //    }
    //}

    //private void GameFrameTurn()
    //{
    //    //first frame is used to process actions
    //    if (GameFrame == 0)
    //    {
    //        if (LockStepTurn())
    //        {
    //            GameFrame++;
    //        }
    //    }
    //    else
    //    {
    //        //update game
    //        //TODO: Add custom physics
    //        //SceneManager.Manager.TwoDPhysics.Update (GameFramesPerSecond);

    //        //List<IHasGameFrame> finished = new List<IHasGameFrame>();
    //        //foreach (IHasGameFrame obj in SceneManager.Instance.GameFrameObjects)
    //        //{
    //        //    obj.GameFrameTurn(GameFramesPerSecond);
    //        //    if (obj.Finished)
    //        //    {
    //        //        finished.Add(obj);
    //        //    }
    //        //}

    //        //foreach (IHasGameFrame obj in finished)
    //        //{
    //        //    SceneManager.Instance.GameFrameObjects.Remove(obj);
    //        //}

    //        GameFrame++;
    //        if (GameFrame == GameFramesPerLocksetpTurn)
    //        {
    //            GameFrame = 0;
    //        }
    //    }
    //}

    //private bool LockStepTurn()
    //{
    //    //Check if we can proceed with the next turn
    //    bool nextTurn = NextTurn();
    //    if (nextTurn)
    //    {
    //        SendPendingAction();
    //        //the first and second lockstep turn will not be ready to process yet
    //        if (LockStepTurnID >= FirstLockStepTurnID + 3)
    //        {
    //            ProcessActions();
    //        }
    //    }
    //    //otherwise wait another turn to recieve all input from all players

    //    return nextTurn;
    //}

    //private bool NextTurn()
    //{
    //    if (pendingActions.ReadyForNextTurn())
    //    {
    //        LockStepTurnID++;
    //        pendingActions.NextTurn();
    //        return true;
    //    }

    //    return false;
    //}

    //public void AddAction(IAction action)
    //{
    //    actionsToSend.Enqueue(action);
    //}

    //private void SendPendingAction()
    //{
    //    IAction action = null;
    //    if (actionsToSend.Count > 0)
    //    {
    //        action = actionsToSend.Dequeue();
    //    }

    //    if (action == null)
    //    {
    //        action = new NoAction();
    //    }
    //    pendingActions.AddAction(action, this.virtualClient.ID, LockStepTurnID, LockStepTurnID);
    //    SendActionToOtherPlayers(LockStepTurnID,this.virtualClient.ID,action);

    //}

    //public void ReceiveAction(int lockStepTurn,int clientId,IAction action)
    //{
    //    pendingActions.AddAction(action, clientId, LockStepTurnID, lockStepTurn);
    //}

    //public void SendActionToOtherPlayers(int lockStepId,int playerid,IAction action)
    //{
    //    this.virtualClient.SendLockStepAction(lockStepId, playerid, action);
    //}

    //private void ProcessActions()
    //{
    //    foreach (IAction action in pendingActions.CurrentActions)
    //    {
    //        action.ProcessAction(this.virtualClient);
    //    }

    //}

}
