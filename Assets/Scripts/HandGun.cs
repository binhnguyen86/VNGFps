using UnityEngine;

public class HandGun : MonoBehaviour
{
    [SerializeField]
    private int _bullets;

    [SerializeField]
    private float _reloadTime;

    [SerializeField]
    private float _fireRate;
    
    private float _nextFireTime;
    private float _timeToFinishReload;
    private int _currentBullets;

    public void SetupWeapon()
    {
        _nextFireTime = 0;
        _timeToFinishReload = 0;
        _currentBullets = _bullets;
    }

    public Bullet Shoot(SpawnManager SpawnManager, int bullets, Vector3 muzzle, Vector3 target)
    {
        if (_timeToFinishReload > Time.time || _nextFireTime > Time.time)
        {
            return null;
        }

        _nextFireTime = Time.time + _fireRate;
        _currentBullets -= bullets;

        Bullet bullet = SpawnManager.GetPoolObject("Bullet") as Bullet;
        bullet.Fire(muzzle, target);
        if (_currentBullets <= 0)
        {
            Reload();
        }
        return bullet;
    }

    private void Reload()
    {
        _currentBullets = _bullets;
        _timeToFinishReload = Time.time + _reloadTime;
    }

}
