using System;
using UnityEngine;

[Serializable]
public class WaveData
{
    public SerializedDictionary<string, EnemyTemplate> enemyTemplates;
    public Wave[] waves;
}

[Serializable]
public class EnemyTemplate
{
    public string name;
    public float spawnInterval;
}

[Serializable]
public class Wave
{
    public int[] waveRange;
    public int nextWaveTime;
    public WaveEnemy[] enemies;
}

[Serializable]
public class WaveEnemy
{
    public string template; // enemyTemplates를 참조하기 위한 키
    public int count;
}
