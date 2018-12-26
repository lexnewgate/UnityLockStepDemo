using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirtualClient: MonoBehaviour
{
    public int ID = 0;
    Queue<IAction> m_generalActions=new Queue<IAction>();
    public bool battleStart = false;

    private LockStepManager m_lockStepManager;
    private void Awake()
    {
    }

    public void Init()
    {
        VirtualManager.Instance.ConnectToServer(ID);
        m_lockStepManager = new LockStepManager();
    }

    private void Update()
    {
        while(m_generalActions.Count>0)
        {
            m_generalActions.Dequeue().ProcessAction(this);
        }

        if(battleStart)
        {
            Debug.Log($"client{ID} ready to lockstep");
            m_lockStepManager.Update();
        }
    }

    public void OnReceiveGeneralAction(IAction action)
    {
        m_generalActions.Enqueue(action);
    }

    public void OnReceiveLockStepAction(IAction action)
    {

    }


    GameObject player;
    List<GameObject>otherPlayers=new List<GameObject>();
    public void InitPlayer(Vector3 pos)
    {
        player= GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.GetComponent<MeshRenderer>().material.color = Color.red;
        player.name = this.ID.ToString();
        player.transform.SetParent(this.transform);
        player.transform.position = pos;
        player.layer = this.ID+8;
    }

    public void InitOtherPlayer(int id,Vector3 pos)
    {
       var otherPlayer= GameObject.CreatePrimitive(PrimitiveType.Capsule);
        otherPlayer.name = id.ToString();
        otherPlayer.transform.SetParent(this.transform);
        otherPlayer.transform.position = pos;
        otherPlayer.layer = this.ID+8;
        otherPlayers.Add(otherPlayer);
    }







}
