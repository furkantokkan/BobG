using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Humanoid : MonoBehaviour
{
    public float visibleRadius;
    [SerializeField] public Image healthBar;
    protected bool lockRotation;

    protected virtual void Attack(Transform point, Transform parent, int damage)
    {
        var bullet = ObjectPool.Instance.GetPooledObject(0);
        bullet.transform.position = point.position;
        bullet.GetComponent<Bullet>().sender = parent.gameObject;
        bullet.GetComponent<Bullet>().bulletDamage = damage;
        bullet.transform.forward = parent.transform.GetChild(0).transform.forward;
    }

    protected void LookAtEnemy(Collider enemy)
    {
        if (enemy != null)
        {
            lockRotation = true;
            Vector3 dir = enemy.transform.position - transform.position;
            dir.y = 0f;
            transform.GetChild(0).transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(dir), Time.time * 10f);
        }
    }
    protected void LookAtVector(Vector3 target)
    {
        lockRotation = true;
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        transform.GetChild(0).transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(dir), Time.time * 10f);
    }
}
