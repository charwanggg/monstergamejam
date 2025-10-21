using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
    }
    public void startGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    
}
