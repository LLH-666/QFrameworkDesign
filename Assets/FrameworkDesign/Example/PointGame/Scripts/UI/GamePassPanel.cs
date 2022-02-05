using System;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class GamePassPanel : MonoBehaviour,IController
    {
        private void Start()
        {
            this.RegisterEvent<GamePassEvent>(OnGamePass);

            Refresh();
        }

        private void Refresh()
        {
            transform.Find("RemainSecondsText").GetComponent<Text>().text =
                "剩余时间:" + this.GetSystem<ICountDownSystem>().CurrentRemainSeconds + "s";

            var gameModel = this.GetModel<IGameModel>();

            transform.Find("BestScoreText").GetComponent<Text>().text =
                "最高分数:" + gameModel.BestScore.Value;

            transform.Find("ScoreText").GetComponent<Text>().text =
                "分数:" + gameModel.Score.Value;
        }
        
        private void OnGamePass(GamePassEvent e)
        {
            Refresh();
        }
        
        private void OnDestroy()
        {
            this.UnRegisterEvent<GamePassEvent>(OnGamePass);
        }

        public IArchitecture GetArchitecture()
        {
            return PointGame.Interface;
        }
    }
}