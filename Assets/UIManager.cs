using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void Start()
    {
        SceneManager.LoadSceneAsync(1);
    }
    
}
