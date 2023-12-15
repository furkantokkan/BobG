using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;

    public GameObject sender;

    public int bulletDamage = 0;

    private float bulletLifeTime = 3f;
    void FixedUpdate()
    {
        transform.localPosition += transform.forward * (bulletSpeed * Time.deltaTime);
        bulletLifeTime -= Time.deltaTime;
        if (bulletLifeTime <= 0)
        {
            ObjectPool.Instance.SetPooledObject(gameObject, 0);
        }
    }
}
