using UnityEngine;

// 写命名空间是个好习惯
namespace FrameworkDesign.Example
{
    public class Enemy : AbstractPointGameController
    {
        /// <summary>
        /// 点击自己则销毁
        /// </summary>
        private void OnMouseDown()
        {
            gameObject.SetActive(false);

            this.SendCommand<KillEnemyCommand>();
        }
    }
}