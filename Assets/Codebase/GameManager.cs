using System;
using UnityEngine;

namespace Codebase
{
    public enum GameState { Idle, Playing, Lost }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameBootstrapper bootstrapper;
        [SerializeField] private GameObject gameUI;
        [SerializeField] private ResultScore loseUI;

        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            State = GameState.Playing;
            gameUI.SetActive(true);
            loseUI.gameObject.SetActive(false);
            bootstrapper.Launcher.StartSpawning();
        }

        public void RestartGame()
        {
            State = GameState.Playing;
            bootstrapper.CubeSpawner.ReleaseAll();
            bootstrapper.LoseArea.Clear();
            bootstrapper.ScoreCounter.ResetScore();
            StartGame();
        }

        public void Lose()
        {
            State = GameState.Lost;
            loseUI.gameObject.SetActive(true);
            gameUI.SetActive(false);
            loseUI.GetResult();
        }
    }
}