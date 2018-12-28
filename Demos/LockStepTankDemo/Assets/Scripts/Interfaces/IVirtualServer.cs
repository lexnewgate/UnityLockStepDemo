using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IVirtualServer
{
    void Init();
    void RelayLockStepActionToOthers(int playerId,int lockStepTurn,ILockStepAction action);
    void BroadcastGeneralAction(IGeneralAction action);
    void OnReceiveClientReady(int clientId);
    bool CheckAllPlayerReady();
    void NotifyGameStart();
}
