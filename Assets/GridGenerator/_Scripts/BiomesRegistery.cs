using System.Collections.Generic;
using UnityEngine;

public class BiomesRegistery : MonoBehaviour
{
    public static BiomesRegistery Instance;

    [SerializeField]
    public List<BiomeData> _biomes = new List<BiomeData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < _biomes.Count; i++)
            _biomes[i].SetBiomeID(i);
    }

    public BiomeData GetRandomBiomes() { return _biomes[Random.Range(0, _biomes.Count)]; }
    public BiomeData GetRandomBiomes(int exclude) 
    {
        BiomeData excludedBiome = _biomes[exclude];
        List<BiomeData> tempBiomesList = new List<BiomeData>(_biomes);
        tempBiomesList.Remove(excludedBiome);
        return tempBiomesList[Random.Range(0, tempBiomesList.Count)]; 
    }
    public BiomeData GetBiome(int id) { return _biomes[id]; }

}
