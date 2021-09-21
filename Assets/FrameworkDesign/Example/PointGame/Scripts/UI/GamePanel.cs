using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrameworkDesign.Example
{
    public class GamePanel : AbstractPointGameController
    {
        private ICountDownSystem mCountDownSystem;
        private IGameModel mGameModel;

        private void Awake()
        {
            mCountDownSystem = this.GetSystem<ICountDownSystem>();
            mGameModel = this.GetModel<IGameModel>();
            
            mGameModel.Gold.RegisterOnValueChanged(OnGoldValueChanged);
            mGameModel.Life.RegisterOnValueChanged(OnLifeValueChanged);
            mGameModel.Score.RegisterOnValueChanged(OnScoreValueChanged);

            // 第一次需要调用一下
            OnGoldValueChanged(mGameModel.Gold.Value);
            OnLifeValueChanged(mGameModel.Life.Value);
            OnScoreValueChanged(mGameModel.Score.Value);
        }
        
        private void OnLifeValueChanged(int life)
        {
            transform.Find("LifeText").GetComponent<Text>().text = "生命：" + life;
        }

        private void OnGoldValueChanged(int gold)
        {
            transform.Find("GoldText").GetComponent<Text>().text = "金币：" + gold;
        }

        private void OnScoreValueChanged(int score)
        {
            transform.Find("ScoreText").GetComponent<Text>().text = "分数:" + score;
        }

        private void Update()
        {
            // 每 20 帧 更新一次
            if (Time.frameCount % 20 == 0)
            {
                transform.Find("CountDownText").GetComponent<Text>().text =
                    mCountDownSystem.CurrentRemainSeconds + "s";

                mCountDownSystem.Update();
            }

        }
        
        private void OnDestroy()
        {
            mGameModel.Gold.UnRegisterOnValueChanged(OnGoldValueChanged);
            mGameModel.Life.UnRegisterOnValueChanged(OnLifeValueChanged);
            mGameModel.Score.UnRegisterOnValueChanged(OnScoreValueChanged);
            mGameModel = null;
            mCountDownSystem = null;
        }

    }
}