using FrameworkDesign;

namespace FrameworkDesign.Example
{
    public class KillEnemyCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IGameModel>();
            gameModel.KillCount.Value++;

            if (UnityEngine.Random.Range(0, 10) < 3)
            {
                gameModel.Gold.Value += UnityEngine.Random.Range(1, 3);
            }

            this.SendEvent<OnKillEnemyEvent>();

            // 十个全部消灭再显示通关界面
            if (gameModel.KillCount.Value == 10)
            {
                // 触发游戏通关事件
                this.SendEvent<GamePassEvent>();
            }
        }
    }
}