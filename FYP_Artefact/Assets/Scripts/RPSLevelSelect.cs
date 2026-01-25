using UnityEngine;
using UnityEngine.SceneManagement;

public class RPSLevelSelect : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField] private string wildcard;
    [SerializeField] private string strategist;
    [SerializeField] private string minister;

    public void OnWildcardPressed()
    {
        SceneManager.LoadScene(wildcard);
    }
    
    public void OnStrategistPressed()
    {
        SceneManager.LoadScene(strategist);
    }
    
    public void OnMinisterPressed()
    {
        SceneManager.LoadScene(minister);
    }
}
