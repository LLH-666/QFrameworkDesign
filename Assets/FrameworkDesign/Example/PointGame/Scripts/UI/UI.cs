using UnityEngine;

namespace FrameworkDesign.Example
{
    public class UI : AbstractPointGameController
    {
        void Start()
        {
            this.RegisterEvent<GamePassEvent>(OnGamePass);
            
            this.RegisterEvent<OnCountDownEndEvent>(e =>
            {
                transform.Find("Canvas/GamePanel").gameObject.SetActive(false);
                transform.Find("Canvas/GameOverPanel").gameObject.SetActive(true);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

        }

        private void OnGamePass(GamePassEvent e)
        {
            transform.Find("Canvas/GamePanel").gameObject.SetActive(false);
            transform.Find("Canvas/GamePassPanel").gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            this.UnRegisterEvent<GamePassEvent>(OnGamePass);
        }
    }
}