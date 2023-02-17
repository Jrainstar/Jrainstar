using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Firis
{
    #region Test
    public class Panel_A : UI
    {
        public Button toB;
        public bool ispause;

        public override GameObject Load()
        {
            return Resources.Load<GameObject>("Panel_A");
        }

        public override void OnEnter()
        {
            toB = gameObject.transform.Find("ToB").GetComponent<Button>();
            toB.onClick.AddListener(() =>
            {
                if (ispause)
                {
                    var tip = UIComponent.Instance.Push<Tip>();
                    tip.Set("此面板已经停止", null, null);
                    return;
                }
                UIComponent.Instance.Push<Panel_B>();
            });
        }

        public override void OnPause()
        {
            ispause = true;
            Debug.Log("Panel_A is Pause");
        }

        public override void OnResume()
        {
            ispause = false;
            Debug.Log("Panel_A is Resume");
        }
    }

    public class Panel_B : UI
    {
        public Button toC;
        public Button close;
        public bool ispause;
        public override void OnEnter()
        {
            toC = gameObject.transform.Find("ToC").GetComponent<Button>();
            toC.onClick.AddListener(() =>
            {
                if (ispause)
                {
                    var tip = UIComponent.Instance.Push<Tip>();
                    tip.Set("此面板已经停止", null, null);
                    return;
                }
                UIComponent.Instance.Push<Panel_C>();
            });


            close = gameObject.transform.Find("Close").GetComponent<Button>();
            close.onClick.AddListener(() =>
            {
                if (ispause)
                {
                    var tip = UIComponent.Instance.Push<Tip>();
                    tip.Set("此面板已经停止", null, null);
                    return;
                }
                UIComponent.Instance.Pop();
            });
        }

        public override void OnPause()
        {
            ispause = true;
            Debug.Log("Panel_B is Pause");
        }

        public override void OnResume()
        {
            ispause = false;
            Debug.Log("Panel_B is Resume");
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Panel_B is Exit");
        }

        public override GameObject Load()
        {
            return Resources.Load<GameObject>("Panel_B");
        }
    }

    public class Panel_C : UI
    {
        public Button close;

        public override GameObject Load()
        {
            return Resources.Load<GameObject>("Panel_C");
        }

        public override void OnEnter()
        {
            close = gameObject.transform.Find("Close").GetComponent<Button>();
            close.onClick.AddListener(() =>
            {
                UIComponent.Instance.Pop();
            });
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Panel_C is Exit");
        }
    }
    #endregion
}
