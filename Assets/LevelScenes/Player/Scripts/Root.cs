using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    public Weapon currentWeapon;
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Weapon>() == null)
            {
                continue;
            }
            transform.GetChild(i).GetComponent<Weapon>().id = i;

            if (transform.GetChild(i).gameObject.activeInHierarchy)
            {
                currentWeapon = transform.GetChild(i).GetComponent<Weapon>();
            }
        }
    }
    public void ActivateObject(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == index)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                currentWeapon = transform.GetChild(i).GetComponent<Weapon>();
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
