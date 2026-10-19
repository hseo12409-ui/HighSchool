using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        //DontDestroyOnLoad(transform.parent.gameObject);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("GameScene1");
    }

    public void GameEnd()
    {
        SceneManager.LoadScene("ThanksForPlaying");
    }
}
