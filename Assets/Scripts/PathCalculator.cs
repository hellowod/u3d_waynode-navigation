using UnityEngine;
using System.Collections.Generic;
using System;

/***
 * PathCalculator.cs
 *
 * @author XiangJinBao
 */
namespace Xsjm.WayNode
{
    /// <summary>
    /// WayNode简单寻路算法 (模拟简易Atar算法)
    /// </summary>
    public class PathCalculator
    {
        private List<WayNode> openList = new List<WayNode>();
        private List<WayNode> closeList = new List<WayNode>();
        private List<WayNode> finalPath = new List<WayNode>();

        public WayNode StartWayNode 
        {
            get; private set;
        }

        public WayNode EndWayNode 
        {
            get; private set;
        }

        public WayNode CurrentWayNode 
        {
            get; private set;
        }

        /// <summary>
        /// 获取最近Node节点
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private static WayNode GetNearestWayNode(Vector3 targetPosition)
        {
            WayNode nearestWaypoint = null;
            float nearestDistance = float.PositiveInfinity;

            for (int i = 0; i < WayNodeManager.AllWayNodes.Count; i++) {
                if (WayNodeManager.AllWayNodes[i] != null) {
                    float distance = Vector3.Distance(WayNodeManager.AllWayNodes[i].Position, targetPosition);
                    if (distance < nearestDistance) {
                        nearestWaypoint = WayNodeManager.AllWayNodes[i];
                        nearestDistance = distance;
                    }
                }
            }
            return nearestWaypoint;
        }

        /// <summary>
        /// 获取路径点（算法核心）
        /// </summary>
        /// <param name="fromWaypoint"></param>
        /// <param name="targetWaypoint"></param>
        /// <returns></returns>
        public List<WayNode> GetPath(WayNode fromWaypoint, WayNode targetWaypoint)
        {
            if (WayNodeManager.AllWayNodes == null || WayNodeManager.AllWayNodes.Count < 1) {
                throw new Exception("Not Get Node Path.");
            }

            openList = new List<WayNode>();
            closeList = new List<WayNode>();
            finalPath = new List<WayNode>();

            StartWayNode = fromWaypoint;
            EndWayNode = targetWaypoint;
            CurrentWayNode = StartWayNode;

            if (StartWayNode == EndWayNode) {
                finalPath.Add(EndWayNode);
                return finalPath;
            } else {
                StartWayNode.ProcessWayNode(null, EndWayNode);
            }

            // 遍历寻找路径
            while (true) {
                openList.Remove(CurrentWayNode);
                closeList.Add(CurrentWayNode);

                bool hasLinkedNode = false;
                for (int i = 0; i < CurrentWayNode.LinkedWayNodes.Count; ++i) {
                    WayNode wayNode = CurrentWayNode.LinkedWayNodes[i];
                    if (!closeList.Contains(wayNode)) {
                        wayNode.ProcessWayNode(CurrentWayNode, EndWayNode);
                        openList.Add(wayNode);
                        hasLinkedNode = true;
                    }
                }

                if (openList.Count < 1) {
                    throw new Exception("Not Calculate Node Path.");
                }

                if (hasLinkedNode) {
                    for (int i = 0; i < openList.Count; ++i) {
                        if (openList[i] == EndWayNode) {
                            EndWayNode.ProcessWayNode(CurrentWayNode, EndWayNode);
                            CurrentWayNode = EndWayNode;
                            // 反向加入最终列表
                            WayNode wayNode = CurrentWayNode;
                            while (wayNode.FromWayNode != null) {
                                finalPath.Insert(0, wayNode);
                                wayNode = wayNode.FromWayNode;
                            }
                            return finalPath;
                        }
                    }
                }

                CurrentWayNode = openList[0];
                for (int i = 1; i < openList.Count; ++i) {
                    if (CurrentWayNode.F > openList[i].F) {
                        CurrentWayNode = openList[i];
                    }
                }
            }
        }

        /// <summary>
        /// 计算路径点列表
        /// </summary>
        /// <param name="curentPosition"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public List<WayNode> GetPath(Vector3 curentPosition, Vector3 targetPosition)
        {
            return GetPath(GetNearestWayNode(curentPosition), GetNearestWayNode(targetPosition));
        }

        /// <summary>
        /// 获取DoTween路径列表
        /// </summary>
        /// <param name="curentPosition"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public List<Vector3> GetDoTweenPath(Vector3 curentPosition, Vector3 targetPosition)
        {
            List<Vector3> doTweenWayNodes = new List<Vector3>();
            List<WayNode> wayNodes = GetPath(curentPosition, targetPosition);
            if (wayNodes != null) {
                foreach (WayNode node in wayNodes) {
                    doTweenWayNodes.Add(node.Position);
                }
            }
            if (doTweenWayNodes.Count < 1) {
                return null;
            }
            return doTweenWayNodes;
        }
    }
}

