using Game.CoreGame;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
    class GameSceneEntryPoint : MonoBehaviour
    {
        [SerializeField, IsntNull] EnemySpawner enemySpawner;
        [SerializeField, IsntNull] TowerBuilder towerBuilder;

        private void Start()
        {
            EnemyesOnBoardCollection enemyesOnBoardCollection = new EnemyesOnBoardCollection();
            enemySpawner.Init(enemyesOnBoardCollection);
            towerBuilder.Init(enemyesOnBoardCollection);
            enemySpawner.StartWave();
        }
    }
}
