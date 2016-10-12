using System.Collections.Generic;
using UnityEngine;

/***
 * WayNodeManager.cs
 *
 * @author XiangJinBao
 */
namespace Xsjm.WayNode
{
    /// <summary>
    /// 路径节点管理器
    /// </summary>
    [ExecuteInEditMode]
    public class WayNodeManager : MonoBehaviour
    {
        // 路径点前缀
        public const string WayNodePrefix = "Node";

        // 是否显示点路径
        public bool IsShowWayNode = true;
        // 节点绘制半径大小
        public float GizmosNodeSize = 0.5f;
        // 路径节点绘制颜色
        public Color32 WayNodeColor = Color.white;
        // 路径节点选中颜色
        public Color32 SelectedWayNodeColor = Color.red;

        // 所有WayNodes
        public static List<WayNode> AllWayNodes = new List<WayNode>();

        public static WayNodeManager Instance 
        {
            get; private set;
        }

        /// <summary>
        /// 添加WayNode
        /// </summary>
        /// <param name="node"></param>
        public static void AddWaypoint(WayNode node)
        {
            if (WayNodeManager.AllWayNodes == null) {
                WayNodeManager.AllWayNodes = new List<WayNode>();
            }
            if (!WayNodeManager.AllWayNodes.Contains(node)) {
                WayNodeManager.AllWayNodes.Add(node);
            }
        }

        // 删除WayNode
        public static void DelWaypoint(WayNode n)
        {
            if (WayNodeManager.AllWayNodes != null) {
                WayNodeManager.AllWayNodes.Remove(n);
            }
        }

        void OnEnable()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}
