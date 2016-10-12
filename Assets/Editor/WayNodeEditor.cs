using UnityEngine;
using UnityEditor;

/***
 * WayNodeEditor.cs
 *
 * @author XiangJinBao
 */
namespace Xsjm.WayNode
{
    /// <summary>
    /// 插件的编辑器菜单类
    /// </summary>
    public class WayNodeEditor : Editor
    {
        [MenuItem("Sanguo Tools/美术工具/Node寻路/WayNode", false, 10)]
        static void NewW2dpWaypoint(MenuCommand menuCommand)
        {
            GameObject nodeWayNodeMgr = null;
            if (GameObject.FindObjectsOfType<WayNodeManager>().Length < 1) {
                nodeWayNodeMgr = new GameObject("WayNodeManager");
                nodeWayNodeMgr.AddComponent<WayNodeManager>();

                GameObjectUtility.SetParentAndAlign(nodeWayNodeMgr, menuCommand.context as GameObject);
                Undo.RegisterCreatedObjectUndo(nodeWayNodeMgr, "Create " + nodeWayNodeMgr.name);
            } else {
                nodeWayNodeMgr = GameObject.FindObjectsOfType<WayNodeManager>()[0].gameObject;
            }
            // 创建一个新的Node
            string newName = "";
            for (int i = 0; ; i++) {
                if (GameObject.Find(WayNodeManager.WayNodePrefix + i.ToString()) == null) {
                    newName = WayNodeManager.WayNodePrefix + i.ToString();
                    break;
                }
            }

            GameObject newNodeObj = new GameObject(newName);
            newNodeObj.AddComponent<WayNode>();
            if (nodeWayNodeMgr != null) {
                newNodeObj.transform.parent = nodeWayNodeMgr.transform;
            }
            GameObjectUtility.SetParentAndAlign(newNodeObj, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(newNodeObj, "Create " + newName);
            Selection.activeObject = newNodeObj;
        }

        [MenuItem("Sanguo Tools/美术工具/Node寻路/WayNodeManager", false, 10)]
        static void NewW2dpWaypointManager(MenuCommand menuCommand)
        {
            if (GameObject.FindObjectsOfType<WayNodeManager>().Length < 1) {
                GameObject newManagerObj = new GameObject("WayNodeManager");
                newManagerObj.AddComponent<WayNodeManager>();

                GameObjectUtility.SetParentAndAlign(newManagerObj, menuCommand.context as GameObject);
                Undo.RegisterCreatedObjectUndo(newManagerObj, "Create " + newManagerObj.name);
            }
        }
    }
}

