using System;
using UnityEngine;

namespace FrameworkDesign
{
    //泛型约束 IEquatable 表示T必须为可比较的类型
    public class BindableProperty<T> where T : IEquatable<T>
    {
        private T mValue;

        public T Value
        {
            get => mValue;
            set
            {
                if (!value.Equals(mValue))
                {
                    mValue = value;
                    mOnValueChanged?.Invoke(value);
                }
            }
        }

        private Action<T> mOnValueChanged = (v) => { };

        public IUnRegister RegisterOnValueChanged(Action<T> onValueChanged)
        {
            mOnValueChanged += onValueChanged;

            return new BindablePropertyUnRegister<T>()
            {
                BindableProperty = this,
                OnValueChanged = onValueChanged
            };
        }

        public void UnRegisterOnValueChanged(Action<T> onValueChanged)
        {
            mOnValueChanged -= onValueChanged;
        }
    }

    public class BindablePropertyUnRegister<T> : IUnRegister where T : IEquatable<T>
    {
        public BindableProperty<T> BindableProperty { get; set; }
        
        public Action<T> OnValueChanged { get; set; }
        
        public void UnRegister()
        {
            BindableProperty.UnRegisterOnValueChanged(OnValueChanged);
        }
    }
}