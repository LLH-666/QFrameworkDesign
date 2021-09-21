using System;
using FrameworkDesign;
using UnityEngine;
using UnityEngine.UI;

namespace CounterApp
{
    public class CounterViewController : AbstractCounterAppController
    {
        private ICounterModel mCounterModel; 
        
        void Start()
        {
            mCounterModel = this.GetModel<ICounterModel>();

            mCounterModel.Count.RegisterOnValueChanged(OnCountChange);

            OnCountChange(mCounterModel.Count.Value);

            transform.Find("BtnAdd").GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    // 交互逻辑
                    this.SendCommand<AddCountCommand>();
                });

            transform.Find("BtnSub").GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    // 交互逻辑
                     this.SendCommand<SubCountCommand>();
                });
        }

        private void OnCountChange(int obj)
        {
            transform.Find("CountText").GetComponent<Text>().text = obj.ToString();
        }

        private void OnDestroy()
        {
            mCounterModel.Count.UnRegisterOnValueChanged(OnCountChange);

            mCounterModel = null;
        }
    }

    public interface ICounterModel : IModel
    {
        BindableProperty<int> Count { get; }
    }
    
    public class CounterModel : AbstractModel, ICounterModel
    {
        protected override void OnInit()
        {
            var storage = this.GetUtility<IStorage>();
            
            Count.Value = storage.LoadInt("COUNTER_COUNT", 0);

            Count.RegisterOnValueChanged(count =>
            {
                storage.SaveInt("COUNTER_COUNT", count);
            });
        }

        public BindableProperty<int> Count { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
    }
}