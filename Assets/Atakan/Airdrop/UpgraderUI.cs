using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgraderUI : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            UIManager.Instance.upgradePanel.SetActive(true);
        }
    }
}
