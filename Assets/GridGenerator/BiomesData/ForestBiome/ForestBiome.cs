using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ForestBiome", menuName = "Biomes/Forest")]
public class ForestBiome : BiomeData
{
    public override IEnumerator BiomeEffect()
    {
        yield return base.BiomeEffect();
    }
}
