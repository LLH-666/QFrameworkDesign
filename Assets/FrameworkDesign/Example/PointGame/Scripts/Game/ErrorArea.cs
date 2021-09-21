using System;
using UnityEngine;

namespace FrameworkDesign.Example
{
    public class ErrorArea : AbstractPointGameController
    {
        private void OnMouseDown()
        {
            Debug.Log($"点错了");
            
            this.SendCommand<MissCommand>();
        }
    }
}