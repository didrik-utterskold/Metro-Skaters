using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerUP : MonoBehaviour
{
    List<GameObject> PowerUps = new();

    GameObject selectedRandomPowerUp;

    private void Awake()
    {
        PowerUps.AddRange(Resources.LoadAll<GameObject>("PowerUps/"));
        SpawnPowerUp();
    }

    // A 40 % chance to spawn a power-up
    public void SpawnPowerUp()
    {
        int randomValue = Random.Range(0, 100);
        if (randomValue <= 40)
        {
            selectedRandomPowerUp = PowerUps[Random.Range(0, PowerUps.Count)];
            Instantiate(selectedRandomPowerUp, transform.position, Quaternion.identity);
        }
    }
}
