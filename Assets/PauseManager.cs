using UnityEngine;

public class PauseManager : MonoBehaviour
{
    bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            ActionHub.Instance.DisableGameplayInputs();
            Time.timeScale = 0f;
        }   
        else
        {
            ActionHub.Instance.EnableGameplayInputs();
            Time.timeScale = 1f;
        }
    }
}
