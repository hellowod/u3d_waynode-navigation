using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using UnityEngine;

/***
 * WayNodeAgent.cs
 *
 * @author XiangJinBao
 */
namespace Xsjm.WayNode
{
    /// <summary>
    /// 点寻路导航代理组件 （类似于Unity NavMeshAgent组件功能），使用时直接将其拖到对应的GameObject上
    /// </summary>
    public class WayNodeAgent : MonoBehaviour
    {
        public enum MoveState
        {
            Idling,
            CalculatingPath,
            Moving,
        }

        public enum MoveType
        {
            DoTween,
            WayNodeSelf,
        }

        #region 公用参数

        // 移动方式
        public MoveType MoveStyle = MoveType.WayNodeSelf;

        // 计算路径结束回调
        public event Action OnCalculatePath;
        // 开始移动回调
        public event Action OnAgentStartMove;
        // 完成移动回调
        public event Action OnAgentCompleteMove;

        // 导航路径计算（核心）
        private PathCalculator calculator = new PathCalculator();
        // 当前状态
        private MoveState currentState;
        // 缓存T变换
        private Transform trCache;

        #endregion

        #region DoTween移动方式参数

        // 移动需要时间
        public float MoveTime = 3;
        // 运动路径类型
        public PathType MovePathType = PathType.Linear;
        // 运动路径模式
        public PathMode MovePathMode = PathMode.TopDown2D;
        // 缓动模式
        public Ease EaseType = Ease.Linear;
        // 移动路径点
        private List<Vector3> currentDoTweenPath;

        #endregion

        #region WayNode移动方式参数

        // 速度控制
        public float MoveSpeed = 20f;
        // 距离误差
        public float DistanceTolerance = 0.2f;
        // 开始位置
        private Vector3 startPosition;
        // 下一个位置
        private Vector3 nextPosition;
        // 节点索引
        private int positionIndex;
        // 持续时间
        private float duration;
        // 移动时间
        private float timeElapsed;
        // 路径节点列表
        private List<WayNode> currentPath;

        #endregion

        #region 内部函数

        /// <summary>
        /// 开始移动
        /// </summary>
        private void StartMove()
        {
            if (OnAgentStartMove != null) {
                OnAgentStartMove();
            }
            currentState = MoveState.Moving;
        }

        /// <summary>
        /// 移动结束
        /// </summary>
        private void EndMove()
        {
            if (OnAgentCompleteMove != null) {
                OnAgentCompleteMove();
            }
            currentState = MoveState.Idling;
        }

        private void Awake()
        {
            trCache = transform;
        }

        /// <summary>
        /// 每一帧执行-WayNode
        /// </summary>
        private void Update()
        {
            if (MoveStyle != MoveType.WayNodeSelf) {
                return;
            }
            switch (currentState) {
                case MoveState.Moving:
                    UpdateMoving();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 移动更新-WayNode
        /// </summary>
        private void UpdateMoving()
        {
            if (Vector3.Distance(trCache.position, nextPosition) > DistanceTolerance) {
                trCache.position = Vector3.Lerp(startPosition, nextPosition, timeElapsed / duration);
                timeElapsed += Time.smoothDeltaTime;
            } else {
                if (positionIndex >= currentPath.Count) {
                    EndMove();
                } else {
                    DoNodeMove(trCache.position, currentPath[positionIndex].Position);
                    positionIndex++;
                }
            }
        }

        /// <summary>
        /// 处理移动-WayNode
        /// </summary>
        /// <param name="fromPosition"></param>
        /// <param name="toPosition"></param>
        private void DoNodeMove(Vector3 fromPosition, Vector3 toPosition)
        {
            startPosition = fromPosition;
            nextPosition = toPosition;
            timeElapsed = 0;
            duration = Vector3.Distance(startPosition, nextPosition) / MoveSpeed;
        }

        /// <summary>
        /// 处理移动-DoTween
        /// </summary>
        /// <param name="fromPosition"></param>
        /// <param name="toPosition"></param>
        private void DoTweenMove(Vector3 fromPosition, Vector3 toPosition)
        {
            currentDoTweenPath = calculator.GetDoTweenPath(fromPosition, toPosition);
            if (currentDoTweenPath != null) {
                StartMove();
                TweenerCore<Vector3, Path, PathOptions> tweenCore = transform.DOPath(
                    currentDoTweenPath.ToArray(), MoveTime, MovePathType, MovePathMode).OnComplete(() => {
                    EndMove();
                });
                tweenCore.SetEase(EaseType);
            }
        }

        #endregion

        #region 对外接口

        public void Move(WayNode fromWayNode, WayNode targetWayNode)
        {
            currentState = MoveState.Idling;

            if (OnCalculatePath != null) {
                OnCalculatePath();
            }

            currentState = MoveState.CalculatingPath;
            if (MoveStyle == MoveType.WayNodeSelf) {
                currentPath = calculator.GetPath(fromWayNode, targetWayNode);
                if (currentPath != null) {
                    positionIndex = 0;
                    DoNodeMove(trCache.position, fromWayNode.Position);
                    StartMove();
                }
            }
            if (MoveStyle == MoveType.DoTween) {
                DoTweenMove(fromWayNode.Position, targetWayNode.Position);
            }
        }

        public void Move(Vector3 fromPosition, Vector3 toPosition)
        {
            currentState = MoveState.Idling;

            if (OnCalculatePath != null) {
                OnCalculatePath();
            }

            currentState = MoveState.CalculatingPath;
            if (MoveStyle == MoveType.WayNodeSelf) {
                currentPath = calculator.GetPath(fromPosition, toPosition);
                if (currentPath != null) {
                    positionIndex = 0;
                    DoNodeMove(trCache.position, fromPosition);
                    StartMove();
                }
            }
            if (MoveStyle == MoveType.DoTween) {
                DoTweenMove(fromPosition, toPosition);
            }
        }

        #endregion

    }
}
