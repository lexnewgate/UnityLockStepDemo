using System.Collections.Generic;

public class GameStartAction :IGeneralAction 
{
    public Dictionary<int, PlayerTransFixData> PlayerInitTransDatas;

    public void Handle(IVirtualClient virtualClient)
    {
        virtualClient.InitPlayerTrans(this.PlayerInitTransDatas);
    }
}
