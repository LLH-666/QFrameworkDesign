using System;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class GameStartPanel : MonoBehaviour,IController
    {
        private IGameModel mGameModel;
        
        void Start()
        {
            transform.Find("BtnGameStart").GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    gameObject.SetActive(false);

                    // 触发事件
                    this.SendCommand<StartGameCommand>();
                 });
            
            transform.Find("BtnBuyLife").GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    this.SendCommand<BuyLifeCommand>();
                });

            mGameModel = this.GetModel<IGameModel>();

            mGameModel.Gold.Register(OnGoldValueChanged);
            mGameModel.Life.Register(OnLifeValueChanged);
            mGameModel.BestScore.Register(OnBestScoreValueChanged);
            
            OnGoldValueChanged(mGameModel.Gold.Value);
            OnLifeValueChanged(mGameModel.Life.Value);
            OnBestScoreValueChanged(mGameModel.BestScore.Value);
        }

        private void OnBestScoreValueChanged(int obj)
        {
            transform.Find("BestScoreText").GetComponent<Text>().text = "最高分:" + obj;
        }

        private void OnLifeValueChanged(int life)
        {
            transform.Find("LifeText").GetComponent<Text>().text = "生命:" + life;
        }

        private void OnGoldValueChanged(int gold)
        {
            if (gold > 0)
            {
                transform.Find("BtnBuyLife").gameObject.SetActive(true);
            }
            else
            {
                transform.Find("BtnBuyLife").gameObject.SetActive(false);
            }

            transform.Find("GoldText").GetComponent<Text>().text = "金币:" + gold;
        }

        private void OnDestroy()
        {
            mGameModel.Gold.UnRegister(OnGoldValueChanged);
            mGameModel.Life.UnRegister(OnLifeValueChanged);

            mGameModel = null;
        }

        public IArchitecture GetArchitecture()
        {
            return PointGame.Interface;
        }
    }
}