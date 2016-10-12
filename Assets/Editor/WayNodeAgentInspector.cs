using DG.Tweening;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

/***
 * WayNodeAgentInspector.cs
 *
 * @author XiangJinBao
 */
namespace Xsjm.WayNode
{
    /// <summary>
    /// 导航代理组件的界面布局
    /// </summary>
    [CustomEditor(typeof(WayNodeAgent))]
    public class WayNodeAgentInspector : Editor
    {
        private SerializedObject nodeAagent;
        private WayNodeAgent src;

        private SerializedProperty moveStyle;

        private SerializedProperty movePathType;
        private SerializedProperty movePathMode;
        private SerializedProperty moveTime;
        private SerializedProperty easeType;

        private SerializedProperty moveSpeed;
        private SerializedProperty distanceTolerance;

        public void OnEnable()
        {
            nodeAagent = new SerializedObject(target);

            moveStyle = nodeAagent.FindProperty("MoveStyle");

            movePathType = nodeAagent.FindProperty("MovePathType");
            moveTime = nodeAagent.FindProperty("MoveTime");
            movePathMode = nodeAagent.FindProperty("MovePathMode");
            easeType = nodeAagent.FindProperty("EaseType");

            moveSpeed = nodeAagent.FindProperty("MoveSpeed");
            distanceTolerance = nodeAagent.FindProperty("DistanceTolerance");

            src = (WayNodeAgent)target;
        }

        public override void OnInspectorGUI()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("notes:");
            sb.AppendLine("1. like unity NavMeshAgent function.");
            sb.AppendLine("2. Navigation agent supports two modes，DoTween and WayNodeSelf。");
            EditorGUILayout.HelpBox(sb.ToString(), MessageType.Info, true);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            moveStyle.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup("NavigationMode", (WayNodeAgent.MoveType)moveStyle.enumValueIndex));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (moveStyle.enumValueIndex == (int)WayNodeAgent.MoveType.DoTween) {
                EditorGUILayout.BeginHorizontal();
                movePathType.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup("MovePathType", (PathType)movePathType.enumValueIndex));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();
                movePathMode.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup("MovePathMode", (PathMode)movePathMode.enumValueIndex));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();
                easeType.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup("EaseType", (Ease)easeType.enumValueIndex));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();
                moveTime.floatValue = EditorGUILayout.FloatField("MoveTime", moveTime.floatValue);
                EditorGUILayout.EndHorizontal();
            }
            if (moveStyle.enumValueIndex == (int)WayNodeAgent.MoveType.WayNodeSelf) {
                EditorGUILayout.BeginHorizontal();
                moveSpeed.floatValue = EditorGUILayout.FloatField("MoveSpeed", moveSpeed.floatValue);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();
                distanceTolerance.floatValue = EditorGUILayout.FloatField("DistanceTolerance", distanceTolerance.floatValue);
                EditorGUILayout.EndHorizontal();
            }

            nodeAagent.ApplyModifiedProperties();
        }
    }
}