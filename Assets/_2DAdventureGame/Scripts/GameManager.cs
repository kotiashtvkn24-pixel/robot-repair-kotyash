using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIHandler uiHandler;
    [SerializeField] bool showStartScreen;

    enum State { Ready, Playing, Finished }

    EnemyController[] enemies;
    State state;
    int lastFixedCount = -1;
    bool paused;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        uiHandler.ConfigureScreens(BeginLevel, ReloadScene);
        uiHandler.SetHealthValue(player.maxHealth, player.maxHealth);
        UpdateRepairCount();

        if (showStartScreen)
        {
            state = State.Ready;
            PauseLevel();
            uiHandler.DisplayStartScreen(true);
        }
        else
        {
            BeginLevel();
        }
    }

    void Update()
    {
        if (state != State.Playing)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.enterKey.wasPressedThisFrame)
            {
                if (state == State.Ready) BeginLevel();
                else ReloadScene();
            }
            return;
        }

        int repaired = UpdateRepairCount();
        if (player.health <= 0)
        {
            FinishLevel(false);
        }
        else if (enemies.Length > 0 && repaired == enemies.Length)
        {
            FinishLevel(true);
        }
    }

    int UpdateRepairCount()
    {
        int repaired = 0;
        foreach (EnemyController enemy in enemies)
        {
            if (!enemy.isBroken) repaired++;
        }

        if (repaired != lastFixedCount)
        {
            lastFixedCount = repaired;
            uiHandler.SetRepairCount(repaired, enemies.Length);
        }

        return repaired;
    }

    void BeginLevel()
    {
        state = State.Playing;
        uiHandler.DisplayStartScreen(false);
        ResumeLevel();
    }

    void FinishLevel(bool won)
    {
        state = State.Finished;
        PauseLevel();
        uiHandler.DisplayResult(won);
    }

    void PauseLevel()
    {
        paused = true;
        Time.timeScale = 0f;
        player.SetInputEnabled(false);
    }

    void ResumeLevel()
    {
        if (paused)
        {
            paused = false;
            Time.timeScale = 1f;
        }
        player.SetInputEnabled(true);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (paused) Time.timeScale = 1f;
    }
}
