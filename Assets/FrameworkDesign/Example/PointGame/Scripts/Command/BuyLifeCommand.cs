using QFramework;

namespace QFramework.Example
{
    public class BuyLifeCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IGameModel>();

            gameModel.Gold.Value--;
            gameModel.Life.Value++;
        }
    }
}