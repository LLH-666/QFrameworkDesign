using UnityEngine;

namespace FrameworkDesign.Example
{
    public class Game : AbstractPointGameController
    {
        void Start()
        {
            this.RegisterEvent<GameStartEvent>(OnGameStart);
            
            this.RegisterEvent<OnCountDownEndEvent>(e => { transform.Find("Enemies").gameObject.SetActive(false); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<GamePassEvent>(e => { transform.Find("Enemies").gameObject.SetActive(false); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

        }

        private void OnGameStart(GameStartEvent e)
        {
            var enemyRoot = transform.Find("Enemies");
            
            enemyRoot.gameObject.SetActive(true);

            foreach (Transform childTrans in enemyRoot)
            {
                childTrans.gameObject.SetActive(true);
            }

        }

        private void OnDestroy()
        {
            this.UnRegisterEvent<GameStartEvent>(OnGameStart);
        }
    }
}