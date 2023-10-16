using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Firis
{
    public class Scene : Entity
    {

    }

    public static class APP
    {
        public static Scene Scene { get; }
        public static EventSystem EventSystem { get; }
        static APP()
        {
            Scene = EntityFactory.Creat<Scene>();
            EventSystem = EventSystem.Instance;
        }
    }
}
