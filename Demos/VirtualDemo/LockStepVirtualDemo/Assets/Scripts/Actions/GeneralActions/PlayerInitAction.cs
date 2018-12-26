using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PlayerInitAction : IAction
{
    public Vector3 pos;
    public bool clientId;

    public void ProcessAction(VirtualClient client)
    {
        client.InitPlayer(pos);
    }
}
