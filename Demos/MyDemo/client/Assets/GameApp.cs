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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            socket.send(GetKeyCodeActionBytes(KeyCode.W));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            socket.send(GetKeyCodeActionBytes(KeyCode.A));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            socket.send(GetKeyCodeActionBytes(KeyCode.S));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            socket.send(GetKeyCodeActionBytes(KeyCode.D));
        }

        while(actionWrapperList.Count>0)
        {
            actionWrapperList.Dequeue().handle();
        }

    }

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



