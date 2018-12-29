using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IConfirmedActions
{
    void NextTurn();
    bool ReadyForNextTurn();
    void ConfirmAction(int lockStepTurn,int playerId);
}
