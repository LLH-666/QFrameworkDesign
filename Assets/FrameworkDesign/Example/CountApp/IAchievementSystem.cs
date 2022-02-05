using QFramework;
using UnityEngine;

namespace CounterApp
{
    public interface IAchievementSystem : ISystem
    {
        
    }

    public class AchievementSystem : AbstractSystem, IAchievementSystem
    {
        protected override void OnInit()
        {
            var counterModel = this.GetModel<ICounterModel>();
            var previous = counterModel.Count.Value;
            counterModel.Count.Register(newCount =>
            {
                if (newCount >= 10 && previous < 10)
                {
                    Debug.Log($"解锁10成就");
                }
                else if (newCount >= 20 && previous < 20)
                {
                    Debug.Log($"解锁20成就");
                }

                previous = newCount;
            });
        }
    }
}