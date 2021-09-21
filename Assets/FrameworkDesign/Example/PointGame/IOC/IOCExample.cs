using FrameworkDesign;
using UnityEngine;

namespace FrameworkDesign.Example
{
    public class IOCExample : MonoBehaviour
    {
        private void Start()
        {
            //创建一个IOC容器
            var container = new IOCContainer();

            //组册一个蓝牙管理器的实例
            container.Register<IBluetoothManager>(new BluetoothManager());
            
            //根据类型获取蓝牙管理器的实例
            var bluetoothManager = container.Get<IBluetoothManager>();
            
            //连接蓝牙
            bluetoothManager.Connect();
        }
    }

    public interface IBluetoothManager
    {
        void Connect();
    }
    
    public class BluetoothManager : IBluetoothManager
    {
        public void Connect()
        {
            Debug.Log($"蓝牙连接成功");
        }
    }
}