using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TranformHelper
{
    public static Transform FindChildren(this Transform tf, string name)
    {
        Transform childTF = tf.Find(name);
        if (childTF != null) return childTF;
        for (int i = 0; i < tf.childCount; i++)
        {
            childTF = FindChildren(tf.GetChild(i), name);
            if (childTF != null) return childTF;
        }
        return null;
    }
}
