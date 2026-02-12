using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // 必须引用这个才能重置关卡

// ⚠️ 注意：下面的 "LaserTrap" 必须改成你的脚本文件名！
// 如果你的脚本叫 "BackwardsLaser"，就把这行改成 "public class BackwardsLaser : MonoBehaviour"
public class BackwardsLaser : MonoBehaviour 
{
    [Header("必须设置")]
    public float laserRange = 1000f;      // 激光长度
    public LayerMask hitLayers;          // 记得选 Default 和 Player
    public string playerTag = "Player";  // 玩家 Tag
    
    [Header("时间控制")]
    public float activeTime = 2.0f;      // 开启多久
    public float inactiveTime = 7.0f;    // 关闭多久
    public float startDelay = 0f;        // 初始延迟
    
    [Header("方向")]
    public bool shootBackwards = true;   // 勾选=向后射

    private LineRenderer lineRenderer;
    private bool isLaserActive = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; 
        lineRenderer.enabled = false;
        
        StartCoroutine(LaserRoutine());
    }

    void Update()
    {
        if (isLaserActive)
        {
            FireLaser();
        }
    }

    IEnumerator LaserRoutine()
    {
        if (startDelay > 0) yield return new WaitForSeconds(startDelay);

        while (true)
        {
            isLaserActive = true;
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(activeTime);

            isLaserActive = false;
            lineRenderer.enabled = false;
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    void FireLaser()
    {
        Vector3 direction = shootBackwards ? -transform.forward : transform.forward;
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, laserRange, hitLayers))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag(playerTag))
            {
                KillPlayer();
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (direction * laserRange));
        }
    }

    void KillPlayer()
    {
        // 死亡逻辑：重置当前场景，解决 No cameras rendering 问题
        Debug.Log("💀 玩家死亡，重置关卡！");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}