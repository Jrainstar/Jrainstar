using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Firis
{
    public class Tip : UI
    {
        private Action agree;
        private Action cacel;

        private Text info;
        private Button agreeButton;
        private Button cacelButton;

        public override GameObject Load()
        {
            return Resources.Load<GameObject>("Tip");
        }

        public override void OnEnter()
        {
            info = gameObject.transform.Find("Info").GetComponent<Text>();
            agreeButton = gameObject.transform.Find("Agree").GetComponent<Button>();
            cacelButton = gameObject.transform.Find("Cacel").GetComponent<Button>();

            agreeButton.onClick.AddListener(() =>
            {
                UIComponent.Instance.Pop();
                agree?.Invoke();
            });

            cacelButton.onClick.AddListener(() =>
            {
                UIComponent.Instance.Pop();
                cacel?.Invoke();
            });
        }

        public void Set(string info, Action agree, Action cacel)
        {
            this.info.text = info;
            this.agree = agree;
            this.cacel = cacel;
        }
    }
}
