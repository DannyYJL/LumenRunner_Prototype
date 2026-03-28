using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public enum TriggerType 
    { 
        StopAfterObstacle, 
        StopAfterGreen, 
        StopBeforeFlash, 
        StopBeforeChaser,  // 新增：遇到怪物前停下看提示
        FinalStop 
    }
    
    public TriggerType type;
    public TutorialManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (manager != null)
            {
                if (type == TriggerType.StopAfterObstacle) manager.ReachObstaclePassed();
                else if (type == TriggerType.StopAfterGreen) manager.ReachGreenPassed();
                else if (type == TriggerType.StopBeforeFlash) manager.ReachFlashPrompt();
                else if (type == TriggerType.StopBeforeChaser) manager.ReachChaserPrompt(); // 呼叫怪物提示
                else if (type == TriggerType.FinalStop) manager.ReachFinalPosition(); 
            }
            
            gameObject.SetActive(false); 
        }
    }
}