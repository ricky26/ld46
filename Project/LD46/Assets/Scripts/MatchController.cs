using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchController: MonoBehaviour
{
    public LocalState localState;
    public Squad squad;

    public void StartMatch()
    {
        SceneManager.LoadScene("ShootGun");
    }

    public void EndMatch()
    {
        SceneManager.LoadScene("Stash");
    }
}
