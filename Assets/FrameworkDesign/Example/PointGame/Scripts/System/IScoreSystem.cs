using UnityEngine;

namespace FrameworkDesign.Example
{
    public interface IScoreSystem : ISystem
    {
        
    }

    public class ScoreSystem : AbstractSystem, IScoreSystem
    {
        protected override void OnInit()
        {
            var gameModel = this.GetModel<IGameModel>();
            
            //监听游戏通关事件
            this.RegisterEvent<GamePassEvent>(e =>
            {
                var countDownSystem = this.GetSystem<ICountDownSystem>();
                var timeScore = countDownSystem.CurrentRemainSeconds * 10;
                gameModel.Score.Value += timeScore;

                Debug.Log($"Score:{gameModel.Score.Value}");
                Debug.Log($"BestScore:{gameModel.BestScore.Value}");

                if (gameModel.Score.Value > gameModel.BestScore.Value)
                {
                    Debug.Log($"新纪录");
                    gameModel.BestScore.Value = gameModel.Score.Value;
                }
            });

            //监听kill事件
            this.RegisterEvent<OnKillEnemyEvent>(e =>
            {
                gameModel.Score.Value += 10;
                
                Debug.Log($"得分：10");
                Debug.Log($"当前分数:{gameModel.Score.Value}");
            });
            
            //监听miss事件
            this.RegisterEvent<OnMissEvent>(e =>
            {
                gameModel.Score.Value -= 5;
                
                Debug.Log($"得分：-5");
                Debug.Log($"当前分数:{gameModel.Score.Value}");
            });
        }
    }
}