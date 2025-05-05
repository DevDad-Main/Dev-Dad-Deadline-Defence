using System.Collections;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave Settings")]
    public int startingEnemies = 5;
    public float timeBetweenWaves = 20f;
    public float difficultyRamp = 1.2f;

    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    public event Action<int> OnWaveStarted;
    public event Action OnWaveEnded;
    public event Action<float> OnBuildPhaseStarted;
    public bool IsBetweenWaves => isBuildingPhase;

    private int currentWave = 0;
    private bool isBuildingPhase = true;
    private int enemiesLeft;

    private void Awake() => Instance = this;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        isBuildingPhase = true;
        OnBuildPhaseStarted?.Invoke(timeBetweenWaves);

        yield return new WaitForSeconds(timeBetweenWaves);

        isBuildingPhase = false;
        currentWave++;
        OnWaveStarted?.Invoke(currentWave);

        int enemiesToSpawn = Mathf.RoundToInt(startingEnemies * Mathf.Pow(difficultyRamp, currentWave - 1));
        enemiesLeft = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.2f); // Slight delay between spawns
        }
    }

    private void SpawnEnemy()
    {
        Transform spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);

        // Enemy enemyScript = enemy.GetComponent<Enemy>();
        // if (enemyScript != null)
        //     enemyScript.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        enemiesLeft--;

        if (enemiesLeft <= 0)
        {
            OnWaveEnded?.Invoke();
            StartCoroutine(StartNextWave());
        }
    }

    public bool IsBuildPhase() => isBuildingPhase;
}
