using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Firis
{
    public class BoxTrigger2D : MonoBehaviour
    {
        public Action<Collider2D> OnEnterCallback;
        public Action<Collider2D> OnStayCallback;
        public Action<Collider2D> OnExitCallback;

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnEnterCallback?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnStayCallback?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnExitCallback?.Invoke(other);
        }
    }
}
