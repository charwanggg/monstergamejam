using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    
}
