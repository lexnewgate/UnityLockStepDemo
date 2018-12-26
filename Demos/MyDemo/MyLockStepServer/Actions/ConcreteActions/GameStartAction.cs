using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]

class GameStartAction : IAction
{
    public int TypeID { get; set; } = ActionTypes.GeneralAction;

    public int NumberOfPlayes;

    public void ProcessAction()
    {
#if _CLIENTLOGIC_
        GameApp.Instance.GameStart = true;
        GameApp.Instance.NumberOfPlayers = NumberOfPlayes;
#endif
    }
}
