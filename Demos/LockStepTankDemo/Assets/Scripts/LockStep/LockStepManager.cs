using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LockStepManager
{
    private float m_fAccumulatedTime = 0f;
    private float m_fNextGameTime= 0f;

    private float m_fFrameLength = 0.05f; //50 miliseconds 运行每个同步帧的间隔 20fps 这个后续是否要调动态?
    public static readonly int FirstLockStepTurnID = 0;
    public int LockStepTurnID = FirstLockStepTurnID;
    float m_fInterpolation = 0;// interpolation系数 0~1 

    public void Update()
    {
        m_fAccumulatedTime += Time.deltaTime;
        while(m_fAccumulatedTime>m_fNextGameTime)
        {
            if(LockStepTurnReady())
            {
                LockStepTurn();
                this.m_fNextGameTime += this.m_fFrameLength;
                this.LockStepTurnID++; 
            }
        }

        this.m_fInterpolation = (this.m_fAccumulatedTime + this.m_fFrameLength - m_fNextGameTime) / this.m_fFrameLength;
        UpdateRenderPosition(this.m_fInterpolation);
    }


    void UpdateRenderPosition(float interpolation)
    {
        throw new NotImplementedException();
    }

    public void LogicCalculation()
    {
        throw new NotImplementedException();
    }


    bool LockStepTurnReady()
    {
        throw new NotImplementedException();
    }

    void LockStepTurn()
    {
        throw new NotImplementedException();
    }

    void GameFrameTurn()
    {
        throw new NotImplementedException();
    }

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
