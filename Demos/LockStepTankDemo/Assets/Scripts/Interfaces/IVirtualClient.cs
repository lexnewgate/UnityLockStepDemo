using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

interface IVirtualClient
{
    void Init(int clientId, GameObject clientAssets);
    void OnReceiveGeneralAction(IGeneralAction action);
    void OnReceiveLockStepAction(ILockStepAction action);
    void SendLockStepAction(ILockStepAction action);
    void SendClientReady();
}
