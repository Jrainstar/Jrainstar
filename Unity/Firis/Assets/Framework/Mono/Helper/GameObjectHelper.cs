//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Firis
//{
//    public static class GameObjectHelper
//    {
//        /// <summary>
//        /// 找到Binding引用的物体
//        /// </summary>
//        /// <param name="self"></param>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public static GameObject Get(this GameObject self, string name)
//        {
//            return self.GetComponent<Bind>().Get(name);
//        }

//        /// <summary>
//        /// 找到Binding引用物体的某Mono组件
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="self"></param>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public static T GetMono<T>(this GameObject self, string name) where T : UnityEngine.Component
//        {
//            return self.Get(name).GetComponent<T>();
//        }

//        public static UnityEngine.Component GetMono(this GameObject self, string name, Type monoType)
//        {
//            return self.Get(name).GetComponent(monoType);
//        }
//    }
//}