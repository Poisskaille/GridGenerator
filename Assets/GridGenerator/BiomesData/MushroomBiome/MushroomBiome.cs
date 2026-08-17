using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MushroomBiome", menuName = "Biomes/Mushroom")]
public class MushroomBiome : BiomeData
{
    public override IEnumerator BiomeEffect()
    {
        base.BiomeEffect();

        yield return null;
        effectFinished = true;
    }
}
