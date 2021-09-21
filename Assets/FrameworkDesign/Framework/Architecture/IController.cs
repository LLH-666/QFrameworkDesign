using FrameworkDesign.Example;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FrameworkDesign
{
    public interface IController : IBelongToArchitecture, ICanGetSystem, ICanGetModel, ICanSendCommand,
        ICanRegisterEvent
    {
    }

    public abstract class AbstractPointGameController : MonoBehaviour, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return PointGame.Interface;
        }
    }

    public abstract class AbstractEditorWindowCounterAppController : EditorWindow, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return CounterApp.CounterApp.Interface;
        }
    }

    public abstract class AbstractCounterAppController : MonoBehaviour, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return CounterApp.CounterApp.Interface;
        }
    }
}