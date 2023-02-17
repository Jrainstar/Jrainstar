//using Sirenix.OdinInspector;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Firis
//{
//    [HideMonoScript]
//    [DisallowMultipleComponent]
//    public class Bind : SerializedMonoBehaviour
//    {
//        [SerializeField]
//        [PropertySpace(20)]
//        [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "GameObject")]
//        [LabelText("Bind")]
//        private Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

//        public GameObject Get(string name)
//        {
//            if (!cache.ContainsKey(name))
//            {
//                return null;
//            }
//            return cache[name];
//        }

//#if UNITY_EDITOR
//        [ShowInInspector]
//        [PropertyOrder(-1)]
//        [HideLabel]
//        [TitleGroup("拖到这里添加GameObject")]
//        [OnValueChanged("EnterData")]
//        private GameObject Entry;

//        private void EnterData()
//        {
//            string finalName = Entry.name;
//            int index = 0;
//            while (cache.ContainsKey(finalName))
//            {
//                index++;
//                finalName = $"{Entry.name}({index})";
//            }
//            cache.Add(finalName, Entry);
//            Entry = null;
//        }
//#endif
//    }
//}