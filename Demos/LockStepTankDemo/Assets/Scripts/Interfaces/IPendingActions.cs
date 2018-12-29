using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IPendingActions
{
    void NextTurn();
    void AddAction(ILockStepAction action, int actionLockStepTurn, int playerId, int currentLockStepTurn);
    bool ReadyForNextTurn();
}
