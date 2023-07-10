using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{ 
    public void ToGame()
    {
        SceneManager.LoadScene("MainGame");
    }
    public void ToLeaderBoard()
    {
        SceneManager.LoadScene("Leaderboard");
    }
}
