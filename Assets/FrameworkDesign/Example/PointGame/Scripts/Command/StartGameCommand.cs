namespace FrameworkDesign.Example
{
    public class StartGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IGameModel>();

            gameModel.KillCount.Value = 0;
            gameModel.Score.Value = 0;
            
            this.SendEvent<GameStartEvent>();
        }
    }
}