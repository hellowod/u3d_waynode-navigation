using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

/***
 * WayNodeInspector.cs
 *
 * @author XiangJinBao
 */
namespace Xsjm.WayNode
{
    /// <summary>
    /// 节点界面布局
    /// </summary>
    [CustomEditor(typeof(WayNode))]
    [CanEditMultipleObjects]
    public class WayNodeInspector : Editor
    {
        private SerializedObject wayNode;
        private SerializedProperty linkedNodes;
        private WayNode src;

        public void OnEnable()
        {
            wayNode = new SerializedObject(target);
            linkedNodes = wayNode.FindProperty("LinkedWayNodes");
            src = (WayNode)target;
        }

        public override void OnInspectorGUI()
        {
            wayNode.UpdateIfDirtyOrScript();

            GUILayout.Label("Node Linked List: ", EditorStyles.boldLabel);
            GUI.color = Color.white;
            if (targets.Length == 1) {
                // 显示关联节点
                for (int i = 0; i < linkedNodes.arraySize; i++) {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(linkedNodes.GetArrayElementAtIndex(i).objectReferenceValue as WayNode, typeof(WayNode), true);
                    if (GUILayout.Button("-", GUILayout.Width(20))) {
                        DeleteThisNode(i);
                    }
                    GUILayout.EndHorizontal();
                }

                // 添加新节点
                if (GUILayout.Button("Add New Node")) {
                    GameObject newObj = Instantiate(src.gameObject) as GameObject;
                    if (newObj != null) {
                        if (src.transform.parent != null) {
                            newObj.transform.parent = src.transform.parent;
                        }
                        for (int i = 0; ; ++i) {
                            if (GameObject.Find(WayNodeManager.WayNodePrefix + i.ToString()) == null) {
                                newObj.name = WayNodeManager.WayNodePrefix + i.ToString();
                                Undo.RegisterCreatedObjectUndo(newObj, newObj.name);
                                break;
                            }
                        }
                        WayNode newWaypoint = newObj.GetComponent<WayNode>();
                        newWaypoint.LinkedWayNodes.Clear();
                        AddNewNode(newWaypoint);
                        Selection.activeTransform = newWaypoint.transform;
                    }
                }
            }
            // 链接多个是操作
            if (targets.Length > 1) {
                if (GUILayout.Button("Link Selected Node")) {
                    foreach (Object obj in targets) {
                        WayNode currentWayNode = (WayNode)obj;
                        for (int i = 0; i < targets.Length; i++) {
                            if (targets[i].name != obj.name) {
                                WayNode currentNode = (WayNode)targets[i];
                                List<WayNode> node = currentNode.LinkedWayNodes;
                                if (node.IndexOf(currentWayNode) < 0) {
                                    currentNode.LinkedWayNodes.Add(currentWayNode);
                                }
                            }
                        }
                    }
                }
            }
            // 插入节点操作
            if (targets.Length == 2) {
                if (GUILayout.Button("Insert In Between Selected Node")) {
                    WayNode waynode1 = (WayNode)targets[0];
                    WayNode waynode2 = (WayNode)targets[1];

                    GameObject newObj = Instantiate(waynode1.gameObject) as GameObject;
                    WayNode currentWayNode = newObj.GetComponent<WayNode>();

                    currentWayNode.LinkedWayNodes.Clear();
                    currentWayNode.LinkedWayNodes.Add(waynode1);
                    currentWayNode.LinkedWayNodes.Add(waynode2);

                    waynode1.LinkedWayNodes.Add(currentWayNode);
                    waynode1.LinkedWayNodes.Remove(waynode2);

                    waynode2.LinkedWayNodes.Add(currentWayNode);
                    waynode2.LinkedWayNodes.Remove(waynode1);

                    currentWayNode.transform.position = (waynode1.transform.position + waynode2.transform.position) / 2;

                    for (int i = 0; ; i++) {
                        if (GameObject.Find(WayNodeManager.WayNodePrefix + i.ToString()) == null) {
                            currentWayNode.transform.name = WayNodeManager.WayNodePrefix + i.ToString();
                            break;
                        }
                    }
                    Selection.activeTransform = currentWayNode.transform;
                }
            }

            // 帮助提示，不支持中文奇怪
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("notes:");
            sb.AppendLine("");
            sb.AppendLine("1. delete can use delete key.");
            sb.AppendLine("2. link node has a problem.");

            EditorGUILayout.HelpBox(sb.ToString(), MessageType.Info, true);

            wayNode.ApplyModifiedProperties();
        }

        void OnDestroy()
        {
            if (targets.Length <= 0) {
                return;
            }
            foreach (Object t in targets) {
                WayNode node = (WayNode)t;
                if (node == null) {
                    for (int i = 0; i < node.LinkedWayNodes.Count; i++) {
                        if (node.LinkedWayNodes[i] != null) {
                            node.LinkedWayNodes[i].LinkedWayNodes.Remove(node);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node"></param>
        void AddNewNode(WayNode node)
        {
            linkedNodes.arraySize++;
            linkedNodes.GetArrayElementAtIndex(linkedNodes.arraySize - 1).objectReferenceValue = node;

            SerializedObject wayNode = new SerializedObject(node);
            SerializedProperty nodeProperty = wayNode.FindProperty("LinkedWayNodes");
            nodeProperty.arraySize++;
            nodeProperty.GetArrayElementAtIndex(nodeProperty.arraySize - 1).objectReferenceValue = src;
            wayNode.ApplyModifiedProperties();
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="index"></param>
        void DeleteThisNode(int index)
        {
            WayNode wayNodeToDelete = linkedNodes.GetArrayElementAtIndex(index).objectReferenceValue as WayNode;
            bool isNodeFound = false;

            if (wayNodeToDelete != null) {
                SerializedObject nodeObject = new SerializedObject(wayNodeToDelete);
                SerializedProperty nodeProperty = nodeObject.FindProperty("LinkedWayNodes");
                for (int i = 0; i < nodeProperty.arraySize; i++) {
                    WayNode currentWaypoint = nodeProperty.GetArrayElementAtIndex(i).objectReferenceValue as WayNode;
                    if (!isNodeFound) {
                        if (currentWaypoint == wayNode.targetObject as WayNode) {
                            isNodeFound = true;
                        } else {
                            continue;
                        }
                    }
                    if (isNodeFound) {
                        if (i + 1 < nodeProperty.arraySize) {
                            WayNode nextWayNode = nodeProperty.GetArrayElementAtIndex(i + 1).objectReferenceValue as WayNode;
                            nodeProperty.GetArrayElementAtIndex(i).objectReferenceValue = nextWayNode;
                        }
                    }
                }
                nodeProperty.arraySize--;
                nodeObject.ApplyModifiedProperties();
            }

            for (int i = index; i < linkedNodes.arraySize - 1; i++) {
                WayNode nextWayNode = linkedNodes.GetArrayElementAtIndex(i + 1).objectReferenceValue as WayNode;
                linkedNodes.GetArrayElementAtIndex(i).objectReferenceValue = nextWayNode;
            }
            linkedNodes.arraySize--;
            wayNode.ApplyModifiedProperties();
        }
    }
}
