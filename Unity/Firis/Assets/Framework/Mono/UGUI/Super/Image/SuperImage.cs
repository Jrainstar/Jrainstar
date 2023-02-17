using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Firis
{
    public class SuperImage : Image, IPointerEnterHandler, IPointerExitHandler
    {
        private bool isIn;
        private bool hasOver;

        public float overTime;

        public Action<SuperImage> onEnter;
        public Action<SuperImage> onOver;
        public Action<SuperImage> offOver;

        public void OnPointerEnter(PointerEventData eventData)
        {
            durationTime = 0;
            hasOver = false;
            isIn = true;
            onEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isIn = false;
            hasOver = true;
            offOver?.Invoke(this);
        }

        private float durationTime;
        private void Update()
        {
            if (!isIn) return;
            if (hasOver) return;
            durationTime += Time.deltaTime;
            if (durationTime > overTime)
            {
                hasOver = true;
                onOver?.Invoke(this);
            }
        }
    }
}
