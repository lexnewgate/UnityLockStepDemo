using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IVirtualClient
{
    int ID { get;  }
    void Init(int clientId, GameObject clientAssets);
    void OnReceiveGeneralAction(IGeneralAction action);
    void OnReceiveLockStepAction(ILockStepAction action);
    void SendLockStepAction(ILockStepAction action);
    void SendClientReady();
    void InitPlayerTrans(Dictionary<int, PlayerTransFixData>playerInitTransDatas);
}
