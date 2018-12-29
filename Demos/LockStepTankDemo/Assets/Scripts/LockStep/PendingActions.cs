using System;
using System.Collections.Generic;
using UnityEngine;

public class PendingActions
{

    //public IAction[] CurrentActions;
    //private IAction[] NextActions;
    //private IAction[] NextNextActions;
    ////incase other players advance to the next step and send their action before we advance a step
    //private IAction[] NextNextNextActions;

    //private int currentActionsCount;
    //private int nextActionsCount;
    //private int nextNextActionsCount;
    //private int nextNextNextActionsCount;

    //LockStepManager lsm;

    //public PendingActions(LockStepManager lsm)
    //{
    //    this.lsm = lsm;

    //    CurrentActions = new IAction[lsm.numberOfPlayers];
    //    NextActions = new IAction[lsm.numberOfPlayers];
    //    NextNextActions = new IAction[lsm.numberOfPlayers];
    //    NextNextNextActions = new IAction[lsm.numberOfPlayers];

    //    currentActionsCount = 0;
    //    nextActionsCount = 0;
    //    nextNextActionsCount = 0;
    //    nextNextNextActionsCount = 0;
    //}

    //public void NextTurn()
    //{
    //    //Finished processing this turns actions - clear it
    //    for (int i = 0; i < CurrentActions.Length; i++)
    //    {
    //        CurrentActions[i] = null;
    //    }
    //    IAction[] swap = CurrentActions;

    //    //last turn's actions is now this turn's actions
    //    CurrentActions = NextActions;
    //    currentActionsCount = nextActionsCount;

    //    //last turn's next next actions is now this turn's next actions
    //    NextActions = NextNextActions;
    //    nextActionsCount = nextNextActionsCount;

    //    NextNextActions = NextNextNextActions;
    //    nextNextActionsCount = nextNextNextActionsCount;

    //    //set NextNextNextActions to the empty list
    //    NextNextNextActions = swap;
    //    nextNextNextActionsCount = 0;
    //}

    //public void AddAction(IAction action, int playerID, int currentLockStepTurn, int actionsLockStepTurn)
    //{
    //    //add action for processing later
    //    if (actionsLockStepTurn == currentLockStepTurn + 1)
    //    {
    //        NextNextNextActions[playerID] = action;
    //        nextNextNextActionsCount++;
    //    }
    //    else if (actionsLockStepTurn == currentLockStepTurn)
    //    {

    //        if(playerID==this.lsm.virtualClient.ID&&NextNextActions[playerID]!=null)
    //        {
    //            Debug.Log("add self action multiple times");
    //        }

    //        //if recieved action during our current turn
    //        //add for processing 2 turns away
          
    //        NextNextActions[playerID] = action;
    //        nextNextActionsCount++;
    //    }
    //    else if (actionsLockStepTurn == currentLockStepTurn - 1)
    //    {
    //        //if recieved action for last turn
    //        //add for processing 1 turn away
        
    //        NextActions[playerID] = action;
    //        nextActionsCount++;
    //    }
    //    else
    //    {
    //        return;
    //    }
    //}

    //public bool ReadyForNextTurn()
    //{
    //    if (nextNextActionsCount == lsm.numberOfPlayers) // 收到当前帧数据 下下帧执行
    //    {
    //        return true;
    //    }

    //    //if this is the 1st turn, no actions had the chance to be recieved yet
    //    if (lsm.LockStepTurnID == LockStepManager.FirstLockStepTurnID)
    //    {
    //        return true;
    //    }
    //    //if none of the conditions have been met, return false
    //    return false;
    //}
}