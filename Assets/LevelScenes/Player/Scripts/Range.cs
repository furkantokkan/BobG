using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    [SerializeField] private float visibleRadius;
    private Player _player;
    private Enemy _enemy;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        DetectEnemy();
    }

    private void DetectEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visibleRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == 8 && colliders[i].gameObject!= gameObject)
            {
                _player.EnemyCollider = colliders[i];
            }
        }
    }
}
