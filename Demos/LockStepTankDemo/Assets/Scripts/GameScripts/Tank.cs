using UnityEngine;
class Tank
{
    #region  LogicData
    FixVector3 m_Position;
    FixVector3 m_Rotation;   
    #endregion

    #region RenderingData 
    GameObject m_go; 
    #endregion 

    public void InitTank(PlayerTransFixData playerTransFixData,GameObject go)
    {
        this.m_Position = playerTransFixData.Position;
        this.m_Rotation = playerTransFixData.Rotation;
        this.m_go = go;
        UpdateRendering();
    }


    public void UpdateRendering()
    {
        this.m_go.transform.localPosition = new Vector3((float)this.m_Position.x,(float)this.m_Position.y,(float)this.m_Position.z);
        this.m_go.transform.localRotation = Quaternion.Euler((float)this.m_Rotation.x,(float)this.m_Rotation.y,(float) this.m_Rotation.z);
    }

    
}