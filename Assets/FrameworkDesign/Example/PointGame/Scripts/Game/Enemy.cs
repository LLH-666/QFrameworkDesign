using UnityEngine;

// 写命名空间是个好习惯
namespace QFramework.Example
{
    public class Enemy : MonoBehaviour,IController
    {
        /// <summary>
        /// 点击自己则销毁
        /// </summary>
        private void OnMouseDown()
        {
            gameObject.SetActive(false);

            this.SendCommand<KillEnemyCommand>();
        }

        public IArchitecture GetArchitecture()
        {
            return PointGame.Interface;
        }
    }
}