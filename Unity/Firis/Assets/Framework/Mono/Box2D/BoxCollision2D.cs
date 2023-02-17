using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Firis
{
    public class BoxCollision2D : MonoBehaviour
    {
        public Action<Collision2D> OnCollisionEnterCallback;
        public Action<Collision2D> OnCollisionStayCallback;
        public Action<Collision2D> OnCollisionExitCallback;

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollisionEnterCallback?.Invoke(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            OnCollisionStayCallback?.Invoke(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            OnCollisionExitCallback?.Invoke(other);
        }
    }
}
