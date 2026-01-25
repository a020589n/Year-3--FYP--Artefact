using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleOverWindow : MonoBehaviour
{
    private static BattleOverWindow _instance;
    
    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void Show(string winnerString)
    {
        gameObject.SetActive(true);
        
        transform.Find("TMP_BattleResult").GetComponent<TextMeshProUGUI>().text = winnerString;
    }

    public static void ShowBattleOverWindow(string winnerString)
    {
        _instance.Show(winnerString);
    }
    
    public void OnRetryPressed()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    
    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
