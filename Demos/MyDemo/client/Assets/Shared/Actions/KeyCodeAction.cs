using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class KeyCodeAction : IAction
{
   public KeyCode keycode;

   
    public static Action<KeyCode> OnInput;

    public void ProcessAction()
    {
        Debug.Log($"keycode:{keycode}");
        OnInput?.Invoke(keycode);
    }
}
