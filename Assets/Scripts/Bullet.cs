using System;
using UnityEngine;

public class Bullet : ObjectToPool
{
    private static float Speed
    {
        get
        {
            return GameController.Instance.BulletSpeed;
        }
    }

    private Vector3 _target;
    private bool _isFiring;

    public Action OnHit;
    public void Fire(Vector3 from, Vector3 to)
    {
        _isFiring = true;
        transform.position = from;
        _target = to;
    }

    private void FixedUpdate()
    {
        if (!_isFiring)
        {
            return;
        }
        float step = Speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _target, step);

        if (Vector3.Distance(transform.position, _target) < 0.001f)
        {
            _target *= -1.0f;
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        _isFiring = false;
        SelfDespawn();
        
    }

    public override void ReleaseItem(ObjectToPool item)
    {
        base.ReleaseItem(item);
        if (OnHit != null)
        {
            OnHit();
            OnHit = null;
        }
    }
}
