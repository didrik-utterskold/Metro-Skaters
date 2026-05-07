using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerUP : MonoBehaviour
{
    private List<GameObject> powerUps = new();

    private void Start()
    {
        powerUps.AddRange(Resources.LoadAll<GameObject>("PowerUps/"));
        SpawnPowerUp();
    }

    // A 40 % chance to spawn a power-up
    public void SpawnPowerUp()
    {
        int randomValue = Random.Range(0, 100);
        if (randomValue <= 40)
        {
            GameObject selectedRandomPowerUp = powerUps[Random.Range(0, powerUps.Count)];
            Instantiate(selectedRandomPowerUp, transform.position, Quaternion.identity, transform);
        }
    }
}
