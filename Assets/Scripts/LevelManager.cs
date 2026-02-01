using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject phoneScreen;
    public static LevelManager Instance;

    public List<LevelConfig> levels;
    public int currentLevelIndex = 0;

    [Header("检测系统引用")]
    public GameObject maskCheck;
    public GameObject temperatureCheck;
    public GameObject nucleicCheck;
    public GameObject healthCodeCheck;

    public LevelPersonManager levelPersonManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Count)
        {
            Debug.LogError("关卡索引非法");
            return;
        }

        currentLevelIndex = index;
        LevelConfig config = levels[index];

        // 先全部关闭
        maskCheck.SetActive(false);
        temperatureCheck.SetActive(false);
        nucleicCheck.SetActive(false);
        healthCodeCheck.SetActive(false);
        
        phoneScreen.SetActive(false);

        // 再根据配置开启
        foreach (var item in config.enabledChecks)
        {
            switch (item)
            {
                case CheckItem.Mask:
                    maskCheck.SetActive(true);
                    break;
                case CheckItem.Temperature:
                    temperatureCheck.SetActive(true);
                    break;
                case CheckItem.Nucleic:
                    nucleicCheck.SetActive(true);
                    break;
                case CheckItem.HealthCode:
                    healthCodeCheck.SetActive(true);
                    break;
            }
        }

        Debug.Log($"加载第 {index + 1} 关");
    }

    public void NextLevel()
    {
        if (currentLevelIndex + 1 >= levels.Count)
        {
            Debug.Log("所有关卡结束");
            return;
        }
        LoadLevel(currentLevelIndex + 1);

        if (levelPersonManager != null)
        {
            levelPersonManager.ResetForNewLevel();
        }
    }
}