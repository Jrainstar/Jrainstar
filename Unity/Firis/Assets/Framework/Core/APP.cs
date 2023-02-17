using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Firis
{
    using UnityScene = UnityEngine.SceneManagement.Scene;

    public class Scene : Entity
    {
        //预留，全局操作可以放这里

        public string sceneName { get;private set; }

        public void Load(string scene, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(scene, mode);
            sceneName = scene;
        }

        //public async Task LoadAsync(string scene, Action<float> process, LoadSceneMode mode = LoadSceneMode.Single)
        //{
        //    var progress = Progress.Create(process);
        //    await SceneManager.LoadSceneAsync(scene, mode).ToUniTask(progress: progress);
        //    sceneName = scene;
        //}
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
