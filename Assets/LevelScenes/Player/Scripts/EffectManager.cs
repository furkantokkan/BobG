using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public ParticleSystem Lava, Death, Skull, Heal, Upgrade;
    public ParticleSystem[] Conffettis, Muzzles;
    public GameObject Circle;
    public void FireEffect()
    {
        foreach (ParticleSystem muzzle in Muzzles)
        {
            if(muzzle.transform.parent.gameObject.activeSelf)
            {
                muzzle.Play();
            }
        }
    }
    public void DanceEffect()
    {
        foreach (ParticleSystem conffetti in Conffettis)
        {
            conffetti.Play();
        }
    }
    public void DeathEffect()
    {
        Skull.Play();
    }
}
