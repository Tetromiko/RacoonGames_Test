using UnityEngine;

namespace Codebase
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private Launcher launcher;
        [SerializeField] private CubeSpawner cubeSpawner;
        [SerializeField] private LauncherController launcherController;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private MergeManager mergeManager;
        [SerializeField] private AudioClipsManager audioManager;
        [SerializeField] private CubeAudioManager cubeAudioManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameOverTrigger gameOverTrigger;

        public void Awake()
        {
            audioManager.Initialize();
            cubeAudioManager.Initialize(audioManager);
            cubeSpawner.Initialize(mergeManager, cubeAudioManager);
            launcher.Initialize(cubeSpawner);
            launcherController.Initialize(launcher);
            gameManager.Initialize(launcher, uiManager, audioManager, cubeSpawner, gameOverTrigger);

            mergeManager.OnMerge += uiManager.scoreCounter.IncreaseScore;
            mergeManager.OnMerge += cubeAudioManager.PlayMerge;
            gameOverTrigger.OnGameOver += gameManager.GameOver;

            gameManager.StartGame();
        }
    }
}