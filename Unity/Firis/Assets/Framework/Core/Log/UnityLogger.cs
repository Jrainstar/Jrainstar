using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Firis
{
    public class UnityLogger : ILogger
    {
        public void Error(string error)
        {
            Debug.LogError(error);
        }

        public void Info(string info)
        {
            Debug.Log(info);
        }

        public void Warn(string warn)
        {
            Debug.LogWarning(warn);
        }
    }
}
