using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public GameObject powerStoneModel;

    [Range(1,10)]public int staffDamage;
    [Range(15, 1000)] public int staffDistance;
    [Range(0.1f, 5)] public float staffFireRate;

    public int currentAmmo;
    [Range(1,50)]public int MaxAmmo;

    public ParticleSystem hitEffect;
 
}
