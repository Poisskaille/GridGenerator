using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FrozenBiome", menuName = "Biomes/Frozen")]
public class FrozenBiome : BiomeData
{
    public override IEnumerator BiomeEffect()
    {
        //int slipIndex = 0;

        /*while (true) 
        {
            DiceManager.instance.SetFace(0, 1);
            DiceManager.instance.SetFace(1, 1);
            DiceManager.instance.SetFace(2, 2 - slipIndex);
            DiceManager.instance.SetFace(3, 2 - slipIndex);
            DiceManager.instance.SetFace(4, 3 - slipIndex);
            DiceManager.instance.SetFace(5, 3 - slipIndex);

            yield return new WaitForSeconds(0.5f);
            DiceManager.instance.ResetDice();
            yield return new WaitForSeconds(0.1f);
            DiceManager.instance.RollDice();

            yield return new WaitWhile(() => DiceManager.instance.GetResult() < 0);
            int result = DiceManager.instance.GetResult();

            if (result > 1)
            {
                slipIndex++;
                Vector2Int direction = new Vector2Int(
                    PlayerMovement.instance.GetPosition().x - PlayerMovement.instance.GetLastPosition().x, 
                    PlayerMovement.instance.GetPosition().y - PlayerMovement.instance.GetLastPosition().y);
                Debug.Log(direction);
                PlayerMovement.instance.SetPosition(PlayerMovement.instance.GetPosition() + direction);
                continue;
            }
            else
                break;
        }*/

        yield return base.BiomeEffect();
    }
}
