using UnityEngine;

public class SnowGun : MonoBehaviour
{
    public GameObject snowballPrefab;

    void Start()
    {
        InvokeRepeating("Shoot", 0f, 20f);
    }

    void Shoot()
    {
        Instantiate(snowballPrefab, transform.position, Quaternion.identity);
    }
}