using System;
using System.Reflection;
using UnityEngine;

namespace FrameworkDesign
{
    public class Singleton<T> where T : class
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    //通过反射获取构造
                    var type = typeof(T);
                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    //获取无参非Public的构造
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                    if (ctor == null)
                    {
                        throw new Exception("Non-Public Constructor() not found in " + type.Name);
                    }

                    mInstance = ctor.Invoke(null) as T;
                }

                return mInstance;
            }
        }
    }
}