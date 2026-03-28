using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("全屏开场 UI")]
    public GameObject introPanel;

    [Header("右上角游戏内 UI")]
    public TextMeshProUGUI instructionText;
    public GameObject continueButton;

    [Header("玩家引用")]
    public PlayerMovement player;

    [Header("追逐战系统 (Chaser)")]
    public GameObject chaserObject;
    public GameObject distanceUIObject;

    private int clickCount = 0;
    private int currentPhase = -1; 

    void Start()
    {
        StopPlayer();
        
        if (introPanel != null) introPanel.SetActive(true);
        
        if (instructionText != null)
        {
            instructionText.text = "";
            instructionText.color = Color.white; 
        }
        if (continueButton != null) continueButton.SetActive(false);

        if (chaserObject != null) chaserObject.SetActive(false);
        if (distanceUIObject != null) distanceUIObject.SetActive(false);
        
        currentPhase = -1;
    }

    void Update()
    {
        if (currentPhase == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickCount++;
                if (clickCount >= 3)
                {
                    currentPhase = 1;
                    if (continueButton != null) continueButton.SetActive(true); 
                }
            }
        }
    }

    public void OnIntroStartClicked()
    {
        if (introPanel != null) introPanel.SetActive(false);
        currentPhase = 0;
        if (instructionText != null) instructionText.text = "Left click to shoot and illuminate the environment";
    }

    public void OnContinueClicked()
    {
        if (continueButton != null) continueButton.SetActive(false);

        if (currentPhase == 1) { currentPhase = 2; instructionText.text = "Use A / D keys to move left and right"; ResumePlayer(); }
        else if (currentPhase == 3) { currentPhase = 4; instructionText.text = ""; ResumePlayer(); }
        else if (currentPhase == 5) { currentPhase = 6; instructionText.text = ""; ResumePlayer(); }
        else if (currentPhase == 7) { currentPhase = 8; instructionText.text = ""; ResumePlayer(); }
        
        // 新增：看完怪物警告后，点击 Continue 正式开始逃亡！
        else if (currentPhase == 9) 
        { 
            currentPhase = 10; 
            instructionText.text = "RUN!"; 
            ResumePlayer(); 
            StartChaserEvent(); // 激活怪物和 UI
        }
    }

    public void ReachObstaclePassed() { if (currentPhase == 2) { StopPlayer(); instructionText.text = "Pick up GREEN collectibles to refill ammo"; currentPhase = 3; continueButton.SetActive(true); } }
    public void ReachGreenPassed() { if (currentPhase == 4) { StopPlayer(); instructionText.text = "Pick up the RED power-up and SHOOT the obstacle!"; currentPhase = 5; continueButton.SetActive(true); } }
    
    // 更新：改成了 BLUE power-up
    public void ReachFlashPrompt() { if (currentPhase == 6) { StopPlayer(); instructionText.text = "It's too dark! Pick up the BLUE power-up to light up the map!"; currentPhase = 7; continueButton.SetActive(true); } }
    
    // 新增：遇到怪物前停下弹提示
    public void ReachChaserPrompt() 
    { 
        if (currentPhase == 8) 
        { 
            StopPlayer(); 
            instructionText.text = "WARNING: A Chaser is behind you!\nUse 'W' to accelerate and 'S' to decelerate.\nDodge obstacles and survive!"; 
            instructionText.color = Color.red; 
            currentPhase = 9; 
            continueButton.SetActive(true); 
        } 
    }

    // 更新：终点阶段变成了 10
    public void ReachFinalPosition() { if (currentPhase == 10) { StopPlayer(); instructionText.text = "Tutorial Completed!"; instructionText.color = Color.white; FindObjectOfType<GameManager>()?.CompleteLevel(); } }

    private void StartChaserEvent()
    {
        if (chaserObject != null && player != null)
        {
            Vector3 spawnPos = player.transform.position;
            spawnPos.z -= 20f; // 在玩家身后 20 米处生成怪物
            chaserObject.transform.position = spawnPos;
            chaserObject.SetActive(true);
        }
        
        if (distanceUIObject != null)
        {
            distanceUIObject.SetActive(true);
        }
    }

    private void StopPlayer() { if (player != null) { player.enabled = false; Rigidbody rb = player.GetComponent<Rigidbody>(); if (rb != null) rb.linearVelocity = Vector3.zero; } }
    private void ResumePlayer() { if (player != null) player.enabled = true; }
}