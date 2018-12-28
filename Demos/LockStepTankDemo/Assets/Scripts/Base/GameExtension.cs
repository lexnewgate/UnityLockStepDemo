using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

static public class GameExtension
{

  
        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }


        /*
        //Depth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            foreach(Transform child in aParent)
            {
                if(child.name == aName )
                    return child;
                var result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
        */

    static public T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T ret = go.GetComponent<T>();
        if (null == ret)
            ret = go.AddComponent<T>();
        return ret;
    }

    static public T GetComponentInChildrenOrParent<T>(this GameObject go) where T : Component
    {
        var obj = go.GetComponent<T>();
        if(obj!=null)
        {
            return obj;
        }
        obj = go.GetComponentInChildren<T>();
        if (obj == null)
        {
            obj = go.GetComponentInParent<T>();
        }
        return obj;
    }

    static public List<T> GetComponentsInChildrenOrParent<T>(this GameObject go) where T : Component
    {
        List<T> re = new List<T>();
        var cs = go.GetComponents<T>();
        if (cs != null&& cs.Length>0)
        {
            re.AddRange(cs);
        }
        var ccs = go.GetComponentsInChildren<T>();
        if (ccs != null && ccs.Length > 0)
        {
            re.AddRange(ccs);
        }
        var pcs = go.GetComponentsInParent<T>();
        if (pcs != null && pcs.Length > 0)
        {
            re.AddRange(pcs);
        }
        //Debug.LogWarning(typeof(T).Name+" "+re.Count);
        return re;
    }

    static public T GetComponentInParentOrChildren<T>(this GameObject go) where T : Component
    {
        var obj = go.GetComponent<T>();
        if (obj != null)
        {
            return obj;
        }
        obj = go.GetComponentInParent<T>();
        if (obj == null)
        {
            obj = go.GetComponentInChildren<T>();
        }
        return obj;
    }
    static public IEnumerable<T> GetEnumeratorComponentsInSelfChildren<T>(this Transform tf) where T : Component
    {
        var t = tf.GetComponent<T>();
        if (t != null)
        {
            yield return t;
        }
        //T temp = null;
        Transform child;
        for (int i = 0; i < tf.childCount; i++)
        {
            child = tf.GetChild(i);
            var tcs = child.GetComponents<T>();
            foreach(var temp in tcs)
            {
                yield return temp;
            }
            //if (temp != null)
            //{
            //    yield return temp;
            //}
            var re = GetEnumeratorComponentsInChildren<T>(child);
            foreach (var r in re)
            {
                yield return r;
            }
        }
    }

    static public IEnumerable<T> GetEnumeratorComponentsInChildren<T>(this Transform tf) where T : Component
    {
        T temp = null;
        Transform child;
        for (int i = 0; i < tf.childCount; i++)
        {
            child = tf.GetChild(i);
            temp = child.GetComponent<T>();
            if (temp != null)
            {
                yield return temp;
            }
            var re = GetEnumeratorComponentsInChildren<T>(child);
            foreach (var r in re)
            {
                yield return r;
            }
        }
    }

    static public void ForeachAllChildren<T>(this Transform tf,Action<T> action) where T : Component
    {
        T temp = null;
        Transform child = null;
        for (int i = 0; i < tf.childCount; i++)
        {
            child = tf.GetChild(i);
            temp = tf.GetComponent<T>();
            if (temp != null)
            {
                action(temp);
            }
            ForeachAllChildren<T>(child,action);
        }
    }

    public static DateTime ConvertFromTimestamp(long timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
    }

    public static long ConvertToTimestamp(DateTime time)
    {
        return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }

    public static bool IsLayerInLayermask(int layermask,int layer)
    {
        return layermask == (layermask | (1 << layer));
    }

    public static void ResetMaterialInEditor(this GameObject root)
    {
#if UNITY_EDITOR  &&!UNITY_STANDALONE_WIN
        var mr = root.GetComponent<MeshRenderer>();
        var smr = root.GetComponent<SkinnedMeshRenderer>();
        if (mr != null)
        {
            var sdn = mr.material.shader.name;
            mr.sharedMaterial.shader = Shader.Find(sdn);
        }
        if (smr != null)
        {
            var sdname = smr.sharedMaterial.shader.name;
            
            smr.sharedMaterial.shader = Shader.Find(sdname);
        }
        foreach (Transform child in root.transform)
        {
            child.gameObject.ResetMaterialInEditor();
        }
#endif
    }

    public static void ChangeLayersRecursively(this Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
        {
            child.ChangeLayersRecursively(layer);
        }
    }



#if UNITY_IPHONE
            /* Interface to native implementation */
           [DllImport ("__Internal")]
           private static extern void _copyTextToClipboard(string text);
#endif



    /*
    public static void ResetParticleRendererMaterilaShader(this GameObject t)
    {
        var ms = t.GetComponent<ParticleRenderer>();
        if (ms!=null)
        {
            var sds = ms.sharedMaterial.shader.name;
            ms.sharedMaterial.shader = Shader.Find(sds);
        }
        foreach (Transform child in t.transform)
        {
            child.gameObject.ResetParticleRendererMaterilaShader();
        }
    }
    */
}
