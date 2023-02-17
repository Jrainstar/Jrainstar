using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Firis
{
    public static class ForeachHelper
    {
        public static void ForEach<T>(this List<T> list, Action<T> callback)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var t = list[i];
                callback(t);
            }
        }
    }

}
