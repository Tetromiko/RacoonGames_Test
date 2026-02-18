using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Codebase
{
    public class GameBootstrapper : MonoBehaviour
    {
        
        [SerializeField] private Launcher launcher;
        [SerializeField] private CubeSpawner cubeSpawner;
        [SerializeField] private LauncherController launcherController;
        [SerializeField] private MergeManager mergeManager;
        [SerializeField] private ScoreCounter scoreCounter;
        [SerializeField] private LoseArea loseArea;
        [SerializeField] private ResultScore resultScore;

        public Launcher Launcher => launcher;
        public CubeSpawner CubeSpawner => cubeSpawner;
        public ScoreCounter ScoreCounter => scoreCounter;
        public LoseArea LoseArea => loseArea;
        
        private ObjectPool<Cube> _cubePool;

        public void Awake()
        {
            cubeSpawner.Initialize();
            _cubePool = cubeSpawner.CubePool;
            
            launcher.Initialize(cubeSpawner, loseArea);
            mergeManager.SetPool(_cubePool);
            launcherController.Initialize(launcher);
            resultScore.Initialize(scoreCounter);

            mergeManager.OnMerge += scoreCounter.IncreaseScore;
        }
    }
}