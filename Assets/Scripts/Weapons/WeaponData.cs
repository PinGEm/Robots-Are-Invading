using UnityEngine;

public enum FireMode
{
    Semi,
    Auto,
    Burst
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "FPS/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("General")]
    public string weaponName;
    public FireMode fireMode;

    [Header("Stats")]
    public int damage = 10;
    public float fireRate = 5f;
    public float range = 100f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int maxReserveAmmo = 120;
    public float reloadTime = 2f;

    [Header("Burst")]
    public int burstCount = 3;

    [Header("Spread")]
    public float spreadAngle = 0f;
    public int pellets = 1;

    [Header("Audio Effects")]
    public AudioClip[] gunSFX;
}