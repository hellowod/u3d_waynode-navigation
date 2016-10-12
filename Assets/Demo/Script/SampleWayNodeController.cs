using UnityEngine;
using Xsjm.WayNode;

/***
 * SampleAgentController.cs
 *
 * @Desc   简单的插件测试类
 * @author XiangJinBao
 */
[RequireComponent(typeof(WayNodeAgent))]
public class SampleWayNodeController : MonoBehaviour
{
    public WayNodeAgent PlayerAgent;
    public Animation PlayerAnimation;

    private void Start()
    {
        if (PlayerAgent == null) {
            PlayerAgent = GetComponent<WayNodeAgent>();
        }
        if (PlayerAgent != null) {
            PlayerAgent.OnAgentStartMove += OnStartMove;
            PlayerAgent.OnAgentCompleteMove += OnCompleteMove;
        }
        if (PlayerAnimation == null) {
            PlayerAnimation = GetComponentInChildren<Animation>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerAgent != null) {
            PlayerAgent.Move(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private void OnStartMove()
    {
        Debug.Log("开始寻路");
        if (PlayerAnimation != null) {
            PlayerAnimation.CrossFade("Run");
        }
    }

    private void OnCompleteMove()
    {
        Debug.Log("寻路完成");
        if (PlayerAnimation != null) {
            PlayerAnimation.CrossFade("Attack");
        }
    }
}

