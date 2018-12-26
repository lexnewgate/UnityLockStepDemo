using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PendingActions
{
    private Dictionary<int, IAction> m_CurrentActions;
    private Dictionary<int, IAction> m_NextActions;
    private Dictionary<int, IAction> m_NextNextActions;
    private Dictionary<int, IAction> m_NextNextNextActions;

    LockStepManager lsm;

    public void NextTurn()
    {
        ClearCurrentActions();
        RollActions();
    }

    public void AddAction(IAction action, int playerID, int currentLockStepTurn, int actionsLockStepTurn)
    {
        if (actionsLockStepTurn == currentLockStepTurn + 1) //下一帧
        {
            m_NextNextNextActions[playerID] = action;
        }
        else if (actionsLockStepTurn == currentLockStepTurn)//当前帧
        {
            m_NextNextActions[playerID] = action;
        }
        else if (actionsLockStepTurn == currentLockStepTurn - 1) //上一帧
        {
            m_NextActions[playerID] = action;
        }
        else
        {
            return;
        }
    }


    public bool ReadyForNextTurn()
    {
        ////第一帧不需要pending
        //if(lsm.LockStepTurnID==LockStepManager.FirstLockStepTurnID) 
        //{
        //    return true;
        //}

        if(m_NextActions.Count==lsm.numberOfPlayers)
        {
            return true;
        }

        return false;

    }



    void RollActions()
    {
        var swap = m_CurrentActions;
        m_CurrentActions = m_NextActions;
        m_NextActions = m_NextNextActions;
        m_NextNextNextActions = swap;
    }

    void ClearCurrentActions()
    {
        m_CurrentActions.Clear();
    }



}