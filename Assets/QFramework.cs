using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    #region Architecture
    
    public interface IArchitecture
    {
        /// <summary>
        /// 注册系统
        /// </summary>
        void RegisterSystem<T>(T instance) where T : ISystem;

        /// <summary>
        /// 注册 Model
        /// </summary>
        void RegisterModel<T>(T instance) where T : IModel;

        /// <summary>
        /// 注册 Utility
        /// </summary>
        void RegisterUtility<T>(T instance) where T : IUtility;

        /// <summary>
        /// 获取系统
        /// </summary>
        T GetSystem<T>() where T : class, ISystem;
        
        /// <summary>
        /// 获取 Model
        /// </summary>
        T GetModel<T>() where T : class, IModel;

        /// <summary>
        /// 获取 Utility
        /// </summary>
        T GetUtility<T>() where T : class, IUtility;

        /// <summary>
        /// 发送命令
        /// </summary>
        void SendCommand<T>() where T : ICommand, new();

        /// <summary>
        /// 发送命令
        /// </summary>
        void SendCommand<T>(T command) where T : ICommand;

        /// <summary>
        /// 发送查询指令
        /// </summary>
        TResult SendQuery<TResult>(IQuery<TResult> query);

        /// <summary>
        /// 发送事件
        /// </summary>
        void SendEvent<T>() where T : new();
        void SendEvent<T>(T e);

        /// <summary>
        /// 注册事件
        /// </summary>
        IUnRegister RegisterEvent<T>(Action<T> onEvent);

        /// <summary>
        /// 注销事件
        /// </summary>
        void UnRegisterEvent<T>(Action<T> onEvent);
    }
    
    /// <summary>
    /// 架构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        private bool mInited = false;
        
        /// <summary>
        /// 用于初始化的 Models 的缓存
        /// </summary>
        private List<IModel> mModels = new List<IModel>();
        
        /// <summary>
        /// 用于初始化的 Systems 的缓存
        /// </summary>
        private List<ISystem> mSystems = new List<ISystem>();

        // 提供一个注册 System 的 API
        public void RegisterSystem<TSystem>(TSystem instance) where TSystem : ISystem
        {
            instance.SetArchitecture(this);
            mContainer.Register<TSystem>(instance);
            
            // 如果初始化过了
            if (mInited)
            {
                instance.Init();
            }
            else
            {
                // 添加到 System 缓存中，用于初始化
                mSystems.Add(instance);
            }
        }

        // 提供一个注册 Model 的 API
        public void RegisterModel<TModel>(TModel instance) where TModel : IModel
        {
            // 需要给 Model 赋值一下
            instance.SetArchitecture(this);
            mContainer.Register<TModel>(instance);
            
            // 如果初始化过了
            if (mInited)
            {
                instance.Init();
            }
            else
            {
                // 添加到 Model 缓存中，用于初始化
                mModels.Add(instance);
            }
        }

        // 提供一个注册 Utility 的 API
        public void RegisterUtility<TUtility>(TUtility instance) where TUtility : IUtility
        {
            mContainer.Register<TUtility>(instance);
        }

        // 获取 System 的 API
        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return mContainer.Get<TSystem>(); 
        }

        // 获取 Model 的 API
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return mContainer.Get<TModel>();
        }
        
        // 获取 Utility 的 API
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return mContainer.Get<TUtility>();
        }

        public void SendCommand<TCommand>() where TCommand : ICommand, new()
        {
            var command = new TCommand();
            command.SetArchitecture(this);
            command.Execute();
        }

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }

        private ITypeEventSystem mTypeEventSystem = new TypeEventSystem();

        public void SendEvent<TEvent>() where TEvent : new()
        {
            mTypeEventSystem.Send<TEvent>();
        }

        public void SendEvent<TEvent>(TEvent e)
        {
            mTypeEventSystem.Send<TEvent>(e);
        }

        public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            return mTypeEventSystem.Register<TEvent>(onEvent);
        }

        public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            mTypeEventSystem.UnRegister<TEvent>(onEvent);
        }

        #region 类似单例模式 但是仅在内部可访问
        /// <summary>
        /// 增加注册
        /// </summary>
        public static Action<T> OnRegisterPatch = architecture => { };
        
        private static T mArchitecture = null;

        public static IArchitecture Interface
        {
            get
            {
                if (mArchitecture == null)
                {
                    MakeSureArchitecture();
                }

                return mArchitecture;
            }
        }
        
        // 确保 Container 是有实例的
        static void MakeSureArchitecture()
        {
            if (mArchitecture == null)
            {
                mArchitecture = new T();
                mArchitecture.Init();
                
                //调用
                OnRegisterPatch?.Invoke(mArchitecture);

                // 初始化 Model
                foreach (var architectureModel in mArchitecture.mModels)
                {
                    architectureModel.Init();
                }

                // 清空 Model
                mArchitecture.mModels.Clear();
                
                // 初始化 System
                foreach (var architectureSystem in mArchitecture.mSystems)
                {
                    architectureSystem.Init();
                }

                // 清空 System
                mArchitecture.mSystems.Clear();
                
                mArchitecture.mInited = true;
            }
        }

        #endregion

        private IOCContainer mContainer = new IOCContainer();

        // 留给子类注册模块
        protected abstract void Init();
    }
    
    #endregion

    #region Controller
    
    public interface IController : IBelongToArchitecture, ICanGetSystem, ICanGetModel, ICanSendCommand,
        ICanRegisterEvent, ICanSendQuery
    {
    }
    
    #endregion

    #region System

    public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetUtility,
        ICanRegisterEvent, ICanSendEvent, ICanGetSystem
    {
        void Init();
    }

    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture mArchitecture = null;

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return mArchitecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }

        void ISystem.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }

    #endregion

    #region Model

    public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent
    {
        void Init();
    }

    public abstract class AbstractModel : IModel
    {
        private IArchitecture mArchitecture = null;

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return mArchitecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }

        /// <summary>
        /// 接口阉割
        /// </summary>
        void IModel.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }

    #endregion
    
    #region Utility

    public interface IUtility
    {
    }

    #endregion
    
    #region Command

    public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel, ICanGetUtility,
        ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        void Execute();
    }

    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture mArchitecture = null;

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return mArchitecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }

        void ICommand.Execute()
        {
            OnExecute();
        }

        protected abstract void OnExecute();
    }

    #endregion

    #region Query

    public interface IQuery<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem,
        ICanSendQuery
    {
        TResult Do();
    }

    public abstract class AbstractQuery<T> : IQuery<T>
    {
        public T Do()
        {
            return OnDo();
        }

        protected abstract T OnDo();

        private IArchitecture mArchitecture;

        public IArchitecture GetArchitecture()
        {
            return mArchitecture;
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }
    }

    #endregion

    #region Rule

    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }
    
    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }
    
    public interface ICanGetModel : IBelongToArchitecture
    {
    }

    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel
        {
            return self.GetArchitecture().GetModel<T>();
        }
    }
    
    public interface ICanGetSystem : IBelongToArchitecture
    {
        
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem
        {
            return self.GetArchitecture().GetSystem<T>();
        }
    }
    
    public interface ICanGetUtility : IBelongToArchitecture
    {
        
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility
        {
            return self.GetArchitecture().GetUtility<T>();
        }
    }
    
    public interface ICanRegisterEvent : IBelongToArchitecture
    {
    }
    
    public static class CanRegisterEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
        {
            return self.GetArchitecture().RegisterEvent<T>(onEvent);
        }

        public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
        {
            self.GetArchitecture().UnRegisterEvent<T>(onEvent);
        }
    }
    
    public interface ICanSendCommand : IBelongToArchitecture
    {
        
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new()
        {
            self.GetArchitecture().SendCommand<T>();
        }

        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand
        {
            self.GetArchitecture().SendCommand<T>(command);
        }
    }
    
    public interface ICanSendEvent : IBelongToArchitecture
    {
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : new()
        {
            self.GetArchitecture().SendEvent<T>();
        }

        public static void SendEvent<T>(this ICanSendEvent self, T e)
        {
            self.GetArchitecture().SendEvent<T>(e);
        }
    }
    
    public interface ICanSendQuery : IBelongToArchitecture
    {
        
    }
    
    public static class CanSendQueryExtension
    {
        public static TResult SendQuery<TResult>(this ICanSendQuery self,IQuery<TResult> query)
        {
            return self.GetArchitecture().SendQuery(query);
        }
    }

    #endregion

    #region TypeEventSystem

    public interface ITypeEventSystem
    {
        /// <summary>
        /// 发送事件
        /// </summary>
        void Send<T>() where T : new();

        void Send<T>(T e);

        /// <summary>
        /// 注册事件
        /// </summary>
        IUnRegister Register<T>(Action<T> onEvent);

        /// <summary>
        /// 注销事件
        /// </summary>
        void UnRegister<T>(Action<T> onEvent);
    }

    /// <summary>
    /// 用于注销的接口
    /// </summary>
    public interface IUnRegister
    {
        void UnRegister();
    }

    /// <summary>
    /// 注销接口的实现
    /// </summary>
    public struct TypeEventSystemUnRegister<T> : IUnRegister
    {
        public ITypeEventSystem TypeEventSystem { get; set; }
        public Action<T> OnEvent { get; set; }

        public void UnRegister()
        {
            TypeEventSystem.UnRegister(OnEvent);

            TypeEventSystem = null;
            OnEvent = null;
        }
    }

    /// <summary>
    /// 注销的触发器
    /// </summary>
    public class UnRegisterOnDestroyTrigger : MonoBehaviour
    {
        private HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister)
        {
            mUnRegisters.Add(unRegister);
        }

        private void OnDestroy()
        {
            foreach (var unRegister in mUnRegisters)
            {
                unRegister.UnRegister();
            }

            mUnRegisters.Clear();
        }
    }

    /// <summary>
    /// 注销触发器的使用简化
    /// </summary>
    public static class UnRegisterExtension
    {
        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();
            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }

            trigger.AddUnRegister(unRegister);
        }
    }

    public class TypeEventSystem : ITypeEventSystem
    {
        interface IRegistrations
        {
            
        }

        class Registrations<T> : IRegistrations
        {
            /// <summary>
            /// 因为委托本身就可以一对多注册
            /// </summary>
            public Action<T> OnEvent = obj => { };
        }

        private Dictionary<Type, IRegistrations> mEventRegistrations = new Dictionary<Type, IRegistrations>();

        public static readonly TypeEventSystem Global = new TypeEventSystem();

        public void Send<T>() where T : new()
        {
            var e = new T();
            Send<T>(e);
        }

        public void Send<T>(T e)
        {
            var type = typeof(T);
            IRegistrations evenRegistrations;

            if (mEventRegistrations.TryGetValue(type, out evenRegistrations))
            {
                (evenRegistrations as Registrations<T>)?.OnEvent.Invoke(e);
            }
        }

        public IUnRegister Register<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations eventRegistrations;

            if (mEventRegistrations.TryGetValue(type, out eventRegistrations))
            {
                
            }
            else
            {
                eventRegistrations = new Registrations<T>();
                mEventRegistrations.Add(type, eventRegistrations);
            }

            (eventRegistrations as Registrations<T>).OnEvent += onEvent;

            return new TypeEventSystemUnRegister<T>()
            {
                OnEvent = onEvent,
                TypeEventSystem = this
            };
        }

        public void UnRegister<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations eventRegistrations;

            if (mEventRegistrations.TryGetValue(type, out eventRegistrations))
            {
                (eventRegistrations as Registrations<T>).OnEvent -= onEvent;
            }
        }
    }

    public interface IOnEvent<T>
    {
        void OnEvent(T e);
    }

    public static class OnGlobalEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            return TypeEventSystem.Global.Register<T>(self.OnEvent);
        }

        public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
        }
    }

    #endregion

    #region IOC

    public class IOCContainer
    {
        /// <summary>
        /// 实例
        /// </summary>
        private Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public void Register<T>(T instance)
        {
            var key = typeof(T);
            if (mInstances.ContainsKey(key))
            {
                mInstances[key] = instance;
            }
            else
            {
                mInstances.Add(key, instance);
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (mInstances.TryGetValue(key, out var retObj)) 
            {
                return retObj as T;
            }

            return null;
        }
    }

    #endregion

    #region BindableProperty

    //泛型约束 IEquatable 表示T必须为可比较的类型
    public class BindableProperty<T>
    {
        private T mValue = default(T);

        public BindableProperty(T defaultValue = default)
        {
            mValue = defaultValue;
        }

        public T Value
        {
            get => mValue;
            set
            {
                if (value == null && mValue == null)
                    return;

                if (value != null && value.Equals(mValue))
                    return;
                
                mValue = value;
                mOnValueChanged?.Invoke(value);
            }
        }

        private Action<T> mOnValueChanged = (v) => { };

        public IUnRegister Register(Action<T> onValueChanged)
        {
            mOnValueChanged += onValueChanged;

            return new BindablePropertyUnRegister<T>()
            {
                BindableProperty = this,
                OnValueChanged = onValueChanged
            };
        }
        
        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(mValue);
            return Register(onValueChanged);
        }

        //a.Value==b.Value  ==>  a==b
        public static implicit operator T(BindableProperty<T> property)
        {
            return property.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            mOnValueChanged -= onValueChanged;
        }
    }

    public class BindablePropertyUnRegister<T> : IUnRegister
    {
        public BindableProperty<T> BindableProperty { get; set; }

        public Action<T> OnValueChanged { get; set; }

        public void UnRegister()
        {
            BindableProperty.UnRegister(OnValueChanged);
        }
    }

    #endregion
}
