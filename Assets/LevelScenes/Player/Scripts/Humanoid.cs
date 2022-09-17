using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Humanoid : MonoBehaviour
{
    [SerializeField] protected float visibleRadius;
    [SerializeField] protected Image healthBar;
    protected bool lockRotation;

    protected virtual void Attack(Transform point, Transform parent)
    {
        var bullet = ObjectPool.Instance.GetPooledObject(0);
        bullet.transform.position = point.position;
        bullet.GetComponent<Bullet>().sender = parent.gameObject;
        bullet.transform.localRotation = point.rotation * Quaternion.Euler(90, 0, 0);
    }

    protected void LookAtEnemy(Collider enemy)
    {
        if (enemy != null)
        {
            lockRotation = true;
            transform.GetChild(0).LookAt(enemy.transform);
        }
    }
    protected void LookAtVector(Vector3 target)
    {
        lockRotation = true;
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }
    protected void Move()
    {

    }
}
