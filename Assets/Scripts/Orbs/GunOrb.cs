using UnityEngine;

public class GunOrb : BaseOrb
{
    protected override void PlayerEnterOrb(PlayerContext _player)
    {
        int rand = Random.Range(0, _player.GetAllWeapons.Length);

        do
        {
            rand = Random.Range(0, _player.GetAllWeapons.Length);
        } while (_player.GetAllWeapons[rand].gameObject == _player.GetCurrentWeapon.gameObject);

        _player.GetCurrentWeapon.gameObject.SetActive(false);
        _player.GetAllWeapons[rand].SetActive(true);

        Debug.Log("Changing Weapons");

        _player.GetCurrentWeapon = _player.GetAllWeapons[rand].GetComponent<BaseWeapon>();

        Destroy(this.gameObject);
    }
}
