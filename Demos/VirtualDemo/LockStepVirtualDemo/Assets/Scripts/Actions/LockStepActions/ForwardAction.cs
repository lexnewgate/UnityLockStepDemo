
using System;

public class ForwardAction : IAction
{
    public int clientID;

    public void ProcessAction(VirtualClient virtualClient)
    {
        virtualClient.ForwardPlayer(clientID);

    }

}