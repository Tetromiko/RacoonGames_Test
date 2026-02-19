using UnityEngine;

namespace Codebase
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Launcher launcher;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private AudioClipsManager audioManager;
        [SerializeField] private CubeSpawner cubeSpawner;
        [SerializeField] private GameOverTrigger gameOverTrigger;

        public void Initialize(Launcher launcher, UIManager uiManager, AudioClipsManager audioManager, CubeSpawner cubeSpawner, GameOverTrigger gameOverTrigger)
        {
            this.launcher = launcher;
            this.uiManager = uiManager;
            this.audioManager = audioManager;
            this.cubeSpawner = cubeSpawner;
            this.gameOverTrigger = gameOverTrigger;
        }

        public void StartGame()
        {
            gameOverTrigger.gameObject.SetActive(true);
            launcher.gameObject.SetActive(true);
            audioManager.StopAllSounds();
            uiManager.ShowInGameMenu();
            audioManager.PlaySound("background", volume: 0.2f, loop: true);
            launcher.PrepareNextCube();
        }

        public void Reset()
        {
            uiManager.scoreCounter.ResetScore();
            cubeSpawner.ResetAllCubes();
            StartGame();
        }

        public void GameOver()
        {
            gameOverTrigger.gameObject.SetActive(false);
            launcher.gameObject.SetActive(false);
            uiManager.ShowPostGameMenu();
            audioManager.StopAllSounds();
            audioManager.PlaySound("failure", volume: 0.1f);
        }
    }
}