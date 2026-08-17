using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Obstacle 
{
    public GameObject prefab;
    public int weight = 10;
}

[CreateAssetMenu(fileName = "BiomeData", menuName = "Scriptable Objects/BiomeData")]
public class BiomeData : ScriptableObject
{
    public virtual IEnumerator BiomeEffect()
    {
        yield return null;
        effectFinished = true;
    }

    [HideInInspector]
    public bool effectFinished = false;

    private int biomeID = 0;

    [Range(1, 10)]
    public int biomeScale = 5;

    [Header("Dice Rolls")]
    public int minimum = 0;

    [Header("Biome environnement")]
    public Material _walkableMaterial;
    public Material _nonWalkableMaterial;
    public AudioClip _audioSong;

    public List<Obstacle> _obstaclesPrefab;

    public void SetBiomeID(int id) {  biomeID = id; }
    public int GetBiomeID() { return biomeID; }
}
