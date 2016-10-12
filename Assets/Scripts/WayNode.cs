using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/***
 * WayNode.cs
 *
 * @author XiangJinBao
 */

namespace Xsjm.WayNode
{
    /// <summary>
    /// 单个路径节点
    /// </summary>
    [ExecuteInEditMode]
    public class WayNode : MonoBehaviour
    {
        public List<WayNode> LinkedWayNodes = new List<WayNode>();

        // 类似Astar算法里面的 F=G+H公式
        public float F { get { return G + H; } }
        // 已经行走路径
        public float G { get; private set; }
        // 当前点到终点的预算路径
        public float H { get; private set; }

        public WayNode FromWayNode 
        {
            get; private set;
        }

        public Vector3 Position 
        {
            get { return this == null ? Vector3.zero : transform.position; }
        }

        public bool ProcessWayNode(WayNode fromWayNode, WayNode endWayNode)
        {
            if (endWayNode == null) {
                return false;
            }

            FromWayNode = fromWayNode;
            H = Vector3.Distance(this.Position, endWayNode.Position);
            G = fromWayNode == null ? 0 : FromWayNode.G + H;
            return true;
        }

        void OnEnable()
        {
            WayNodeManager.AddWaypoint(this);
        }

        void OnDisable()
        {
            WayNodeManager.DelWaypoint(this);
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            WayNodeManager WayNodeMgr = WayNodeManager.Instance;
            if (WayNodeMgr.IsShowWayNode) {
                Gizmos.color = WayNodeMgr.WayNodeColor;
                Gizmos.DrawSphere(transform.position, WayNodeMgr.GizmosNodeSize);
                for (int i = 0; i < LinkedWayNodes.Count; i++) {
                    Gizmos.DrawLine(Position, LinkedWayNodes[i].Position);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            WayNodeManager WayNodeMgr = WayNodeManager.Instance;
            if (WayNodeMgr.IsShowWayNode) {
                Gizmos.color = WayNodeMgr.SelectedWayNodeColor;
                Gizmos.DrawSphere(transform.position, WayNodeMgr.GizmosNodeSize);
                for (int i = 0; i < LinkedWayNodes.Count; i++) {
                    Gizmos.DrawLine(Position, LinkedWayNodes[i].Position);
                    Gizmos.DrawLine(LinkedWayNodes[i].Position, Position);
                }
            }
        }
#endif

    }
}

