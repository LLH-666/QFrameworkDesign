using System;
using FrameworkDesign;
using UnityEditor;
using UnityEngine;

namespace CounterApp.Editor
{
    public class EditorCounterApp : AbstractEditorWindowCounterAppController
    {
        /// <summary>
        /// 打开窗口
        /// </summary>
        [MenuItem("EditorCounterApp/Open")]
        static void Open()
        {
            CounterApp.OnRegisterPatch += architecture =>
            {
                architecture.RegisterUtility<IStorage>(new EditorPrefsStorage());
            };
            
            var editorCounterApp = GetWindow<EditorCounterApp>();
            editorCounterApp.name = nameof(EditorCounterApp);
            editorCounterApp.position = new Rect(100, 100, 400, 600);
            editorCounterApp.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("+"))
            {
                this.SendCommand<AddCountCommand>();
            }
            
            //由于实时刷新，所以直接就渲染数据即可
            var value = this.GetModel<ICounterModel>().Count.Value.ToString();
            GUILayout.Label(value);
            
            if (GUILayout.Button("-"))
            {
                this.SendCommand<SubCountCommand>();
            }
        }
    }
}