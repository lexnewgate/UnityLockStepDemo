using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LockStepManager
{
    private float AccumilatedTime = 0f;
    private float FrameLength = 0.05f; //50 miliseconds
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

    private void GameFrameTurn()
    {

    }
}
