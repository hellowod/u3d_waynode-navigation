using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/***
 * WayNodeManagerEditor.cs
 *
 * @author XiangJinBao
 */
namespace Xsjm.WayNode
{
    /// <summary>
    /// 节点管理界面布局
    /// </summary>
    [CustomEditor(typeof(WayNodeManager))]
    [CanEditMultipleObjects]
    public class WayNodeManaegrInspector : Editor
    {
        public const string WAYNODENAME = "WayNode.dat";

        private WayNodeManager src;

        public void OnEnable()
        {
            src = (WayNodeManager)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            src.IsShowWayNode = EditorGUILayout.ToggleLeft("ShowWayNode", src.IsShowWayNode);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            src.GizmosNodeSize = EditorGUILayout.FloatField("NodeDrawRadius", src.GizmosNodeSize);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            src.WayNodeColor = EditorGUILayout.ColorField("NodeColor", src.WayNodeColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            src.SelectedWayNodeColor = EditorGUILayout.ColorField("NodeSelectedColor", src.SelectedWayNodeColor);
            EditorGUILayout.EndHorizontal();

            //if (GUILayout.Button("导出节点数据")) {
            //    ImportWayMapData();
            //    AssetDatabase.Refresh();
            //}
        }

        /// <summary>
        /// 导出地形数据
        /// </summary>
        private void ImportWayMapData()
        {
            List<WayNode> allWayNodes = WayNodeManager.AllWayNodes;
            string fullPath = GetWayNodePath();
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(fullPath)) {
                File.Delete(fullPath);
            }
            bool isFlag = false;
            FileStream fs = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            foreach (WayNode node in allWayNodes) {
                StringBuilder nodeSb = new StringBuilder();
                StringBuilder linkedNodeSb = new StringBuilder();
                foreach (WayNode linkedNode in node.LinkedWayNodes) {
                    linkedNodeSb.Append(linkedNode.Position);
                }
                string nodeString = string.Format("[{0}{1}]", node.Position, linkedNodeSb.ToString());
                if (!isFlag) {
                    isFlag = true;
                    nodeSb.Append(nodeString);
                }
                nodeSb.AppendLine(nodeString);
                byte[] buff = System.Text.UTF8Encoding.UTF8.GetBytes(nodeSb.ToString());
                fs.Write(buff, 0, buff.Length);
            }
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// 获取路径点数据保存路径
        /// </summary>
        /// <returns></returns>
        private string GetWayNodePath()
        {
            string filePath = EditorApplication.currentScene;
            string fileDir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            return string.Format("{0}/{1}/{2}/{3}", Application.dataPath, fileDir.Replace("Assets/", ""), fileName, WAYNODENAME);
        }
    }

}
