using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
class ActionWrapper
{
    public int opCode;
    public byte[] objectData;

    public void handle()
    {
        if(opCode==0)
        {
            KeyCodeAction keycodeAction = BinarySerialization.DeserializeObject<KeyCodeAction>(objectData);
            keycodeAction.ProcessAction();
        }
    }

    public static byte[]ToByte<T>(T o)
    {
        if(o.GetType()==typeof(KeyCodeAction))
        {
           return BinarySerialization.SerializeObjectToByteArray(new ActionWrapper { opCode = 0, objectData = BinarySerialization.SerializeObjectToByteArray(o) });
        }
        return null;
    }

}


