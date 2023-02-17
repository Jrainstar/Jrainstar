using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class Scene : Entity
    {
        //预留，全局操作可以放这里
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
