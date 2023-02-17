using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Firis
{
    public class Box2D : MonoBehaviour
    {
        public event Action<Collider2D> onTriggerEnterCallback;
        public event Action<Collider2D> onTriggerStayCallback;
        public event Action<Collider2D> onTriggerExitCallback;
        public event Action<Collision2D> onCollisionEnterCallback;
        public event Action<Collision2D> onCollisionStayCallback;
        public event Action<Collision2D> onCollisionExitCallback;

        private void OnTriggerEnter2D(Collider2D other)
        {
            onTriggerEnterCallback?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            onTriggerStayCallback?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            onTriggerExitCallback?.Invoke(other);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            onCollisionEnterCallback?.Invoke(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            onCollisionStayCallback?.Invoke(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            onCollisionExitCallback?.Invoke(other);
        }
    }

}
