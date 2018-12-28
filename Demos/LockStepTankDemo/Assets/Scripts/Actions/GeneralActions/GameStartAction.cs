

//using System.Collections.Generic;
//using UnityEngine;

//class GameStartAction : IAction
//{
//   public Dictionary<int, Vector3> clientPosDict;


//    public void ProcessAction(VirtualClient client)
//    {
//        client.battleStart = true;

//        foreach(var id_pos in clientPosDict)
//        {
//            if(client.ID==id_pos.Key)
//            {
//                client.InitPlayer(id_pos.Value);
//            }
//            else
//            {
//                client.InitOtherPlayer(id_pos.Key,id_pos.Value);
//            }
//        }



//    }
//}
