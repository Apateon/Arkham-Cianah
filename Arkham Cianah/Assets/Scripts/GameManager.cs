using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int enemyNumber;
    Spawner spawner;

    GameInputs gameInputs = null;

    public GameObject pauseMenu, touchControls;

    bool cursorStatus;

    void Start()
    {
        //check which device the player is playing on
        cursorStatus = Application.platform != RuntimePlatform.Android;

        if(cursorStatus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            touchControls.SetActive(true);
        }

        gameInputs = new GameInputs();
        gameInputs.Gameplay.Enable();
        gameInputs.Gameplay.Pause.performed += PauseKey;

        spawner = GetComponent<Spawner>();
        enemyNumber = 15;

        StartSpawner();
    }

    private void OnDestroy()
    {
        gameInputs.Gameplay.Pause.performed -= PauseKey;
    }

    void StartSpawner()
    {
        //spawn player
        spawner.SpawnThing(true);

        //spawn 15 enemies
        for(int i = 0; i<enemyNumber; i++)
        {
            spawner.SpawnThing(false);
        }
    }

    public void PauseKey(InputAction.CallbackContext cxt)
    {
        PauseGame();
    }

    public void PauseGame()
    {
        GameState current = GameStateManager.Instance.CurrentGameState;
        GameState newState = current == GameState.Paused ? GameState.Gameplay : GameState.Paused;

        GameStateManager.Instance.SetState(newState);
        if (newState == GameState.Paused)
        {
            pauseMenu.SetActive(true);
            if(cursorStatus)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                touchControls.SetActive(false);
            }
        }
        else
        {
            pauseMenu.SetActive(false);
            if (cursorStatus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                touchControls.SetActive(true);
            }
        }
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("HomeScreen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}