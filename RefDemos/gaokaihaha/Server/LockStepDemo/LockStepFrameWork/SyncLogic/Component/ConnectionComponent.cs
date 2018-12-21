﻿using DeJson;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ConnectionComponent : ServiceComponent
{
    public Player m_player;
    public string m_playerID;

    public SyncSession m_session;
    public bool m_isWaitPushReconnect = false;

    public bool m_isInframe = false;  //这一帧已经在计算中

    public int rtt;                 //网络时延 单位ms
    private int lastInputFrame = -1; //玩家的最后一次输入帧
    public float UpdateSpeed  = 1;  //世界更新速度

    public List<PlayerCommandBase> m_commandList = new List<PlayerCommandBase>();
    public PlayerCommandBase m_defaultInput = null;   //默认输入

    public List<EntityBase> m_waitSyncEntity = new List<EntityBase>(); //等待同步的实体
    public List<int> m_waitDestroyEntity = new List<int>();            //等待同步删除的实体

    public int LastInputFrame
    {
        get => lastInputFrame;
        set
        {
            if(value > lastInputFrame)
            {
                lastInputFrame = value;
            }
        }
    }

    public PlayerCommandBase GetCommand(int frame)
    {
        //没有收到玩家输入复制玩家的最后一次输入
        if (m_commandList.Count == 0)
        {
            PlayerCommandBase pb = GetForecast(frame);
            return pb;
        }
        else
        {
            for (int i = 0; i < m_commandList.Count; i++)
            {
                if (m_commandList[i] != null 
                    && m_commandList[i].frame == frame)
                {
                    return m_commandList[i];
                }
            }

            PlayerCommandBase pb = GetForecast(frame);
            return pb;
        }
    }

    public PlayerCommandBase GetForecast(int frame)
    {
        //Debug.Log("预测操作 id:" + Entity.ID + " frame " + frame);
        PlayerCommandBase cmd = GetLastInput();

        cmd.frame = frame;
        cmd.id = Entity.ID;
        return cmd;
    }

    public PlayerCommandBase GetLastInput()
    {
        if(m_commandList.Count != 0)
        {
            try
            {
                return m_commandList[m_commandList.Count - 1].DeepCopy();

            }catch(Exception e)
            {
                Debug.LogError("m_commandList.Count - 1 " + (m_commandList.Count - 1) + " m_commandList[m_commandList.Count - 1] ->" + m_commandList[m_commandList.Count - 1] + "<-" + e.ToString());
            }

        }

        return GetDefaultInput();
    }

    public PlayerCommandBase GetDefaultInput()
    {
        //TODO defaultInput 没了
        if (m_defaultInput == null)
        {
            Debug.LogError("m_defaultInput is null");

            m_defaultInput = new CommandComponent();
        }

        return m_defaultInput;
    }

    public bool AddCommand(PlayerCommandBase cmd)
    {
        //if (m_commandList.Count > 0
        //    && m_commandList[m_commandList.Count - 1].frame == cmd.frame)
        //{
        //    cmd.frame++;
        //}

        while (GetExitCommand(cmd.frame))
        {
            cmd.frame++;
        }

        LastInputFrame = cmd.frame;
        m_commandList.Add(cmd);

        //if (m_commandList.Count > 0
        //    && m_commandList[m_commandList.Count - 1].frame == cmd.frame)
        //{
        //    m_commandList[m_commandList.Count - 1] = cmd;
        //    return true;
        //}

        //LastInputFrame = cmd.frame;
        //m_commandList.Add(cmd);

        return true;
    }

    public bool GetExitCommand(int frame)
    {
        for (int i = m_commandList.Count - 1 ; i >= 0; i--)
        {
            if(m_commandList[i].frame == frame)
            {
                return true;
            }
        }

        return false;
    }
}
