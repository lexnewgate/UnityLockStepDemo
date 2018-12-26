using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class JsonClass
{
    public string className = "";
    public object data;

    public void print()
    {
        Debug.Log($"className:{className}");
    }
}


public class GameApp : Singleton<GameApp>
{
    public GameObject player;
    SimpleSocket socket;

    public bool GameStart { get; set; } = false;

    public int NumberOfPlayers { get; set; }


    void Start()
    {
        socket = new SimpleSocket();
        socket.Init();
        SimpleSocket.OnReceive += onReceive;
        //KeyCodeAction.OnInput = onInput;
    }


    Queue<IAction> m_GeneralActions = new Queue<IAction>();

    void onReceive(byte[] cmd)
    {
        //var jsonclass = JsonConvert.DeserializeObject<JsonClass>( System.Text.Encoding.Default.GetString(cmd)) ;
        // //Debug.Log($"jsonClass name:{jsonclass.className}");
        // jsonclass.print();
        // var actionType = Type.GetType(jsonclass.className);
        // var action = Activator.CreateInstance(actionType) as IAction;

        //action = jsonclass.data;
        //action.ProcessAction();
        var action = BinarySerialization.DeserializeObject(cmd) as IAction;
        action.ProcessAction();

    }


    private float AccumilatedTime = 0f;
    private float FrameLength = 0.05f; //50 miliseconds


    //called once per unity frame
    public void Update()
    {
        if(GameStart==true)
        {
            Debug.Log($"Game starts:{NumberOfPlayers}");
            LockStepLogic();

        }

        while(m_GeneralActions.Count>0)
        {
            m_GeneralActions.Dequeue().ProcessAction();
        }
    }



    void LockStepLogic()
    {
        //Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength
        AccumilatedTime = AccumilatedTime + Time.deltaTime;

        //in case the FPS is too slow, we may need to update the game multiple times a frame
        while (AccumilatedTime > FrameLength)
        {
            GameFrameTurn();
            AccumilatedTime = AccumilatedTime - FrameLength;
        }

    }


    private int GameFrame = 0;



    private bool LockStepTurn()
    { 
    //{
    //    //log.Debug("LockStepTurnID: " + LockStepTurnID);
    //    //Check if we can proceed with the next turn
    //    bool nextTurn = NextTurn();
    //    if (nextTurn)
    //    {
    //        SendPendingAction();
    //        //the first and second lockstep turn will not be ready to process yet
    //        if (LockStepTurnID >= FirstLockStepTurnID + 3)
    //        {
    //            ProcessActions();
    //        }
    //    }
    //    //otherwise wait another turn to recieve all input from all players

    //    return nextTurn;
        return true;
    }


    private void GameFrameTurn()
    {
        //first frame is used to process actions
        if (GameFrame == 0)
        {
            if (LockStepTurn())
            {
                GameFrame++;
            }
        }
        else
        {
        
        }
    }


    //void Update()
    //{





    //    //if (Input.GetKeyDown(KeyCode.W))
    //    //{
    //    //    socket.send(GetKeyCodeActionBytes(KeyCode.W));
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.A))
    //    //{
    //    //    socket.send(GetKeyCodeActionBytes(KeyCode.A));
    //    //}

    //    //if (Input.GetKeyDown(KeyCode.S))
    //    //{
    //    //    socket.send(GetKeyCodeActionBytes(KeyCode.S));
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.D))
    //    //{
    //    //    socket.send(GetKeyCodeActionBytes(KeyCode.D));
    //    //}

    //    //while(actionWrapperList.Count>0)
    //    //{
    //    //    actionWrapperList.Dequeue().handle();
    //    //}

    //}

    //void onInput(KeyCode keyCode)
    //{
    //    //if(keyCode==KeyCode.W)
    //    //{
    //    //    player.transform.Translate(Vector3.forward);
    //    //}
    //    //else if(key)
    //    if(player)

    //    switch (keyCode)
    //    {
    //        case KeyCode.W:
    //            player.transform.Translate(Vector3.forward);
    //            break;
    //        case KeyCode.S:
    //            player.transform.Translate(-Vector3.forward);
    //            break;
    //        case KeyCode.A:
    //            player.transform.Translate(-Vector3.right);
    //            break;
    //        case KeyCode.D:
    //            player.transform.Translate(Vector3.right);
    //            break;
    //    }

    //}

    //byte[] GetKeyCodeActionBytes(KeyCode keyCode)
    //{
    //    return ActionWrapper.ToByte<KeyCodeAction>(new KeyCodeAction { keycode = keyCode});
    //}


}



