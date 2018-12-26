using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp : MonoBehaviour
{
    public GameObject player;
    SimpleSocket socket;

    void Start()
    {
        socket = new SimpleSocket();
        socket.Init();
        SimpleSocket.OnReceive += onReceive;
        KeyCodeAction.OnInput = onInput;
    }



    private float AccumilatedTime = 0f;

    private float FrameLength = 0.05f; //50 miliseconds

    //called once per unity frame
    public void Update()
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

    void onInput(KeyCode keyCode)
    {
        //if(keyCode==KeyCode.W)
        //{
        //    player.transform.Translate(Vector3.forward);
        //}
        //else if(key)
        if(player)

        switch (keyCode)
        {
            case KeyCode.W:
                player.transform.Translate(Vector3.forward);
                break;
            case KeyCode.S:
                player.transform.Translate(-Vector3.forward);
                break;
            case KeyCode.A:
                player.transform.Translate(-Vector3.right);
                break;
            case KeyCode.D:
                player.transform.Translate(Vector3.right);
                break;
        }

    }

    byte[] GetKeyCodeActionBytes(KeyCode keyCode)
    {
        return ActionWrapper.ToByte<KeyCodeAction>(new KeyCodeAction { keycode = keyCode});
    }


    Queue<ActionWrapper> actionWrapperList = new Queue<ActionWrapper>();

    void onReceive(byte[] cmd)
    {
        var actionWrapper = BinarySerialization.DeserializeObject<ActionWrapper>(cmd);
        actionWrapperList.Enqueue(actionWrapper);
    }
}



