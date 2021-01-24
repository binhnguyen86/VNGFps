using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public GameObject Target;

    public bool IsKeepDistance;

    private Vector3 _originalDistance;
    private void Awake()
    {
        if (IsKeepDistance)
        {
            _originalDistance = Target.transform.position - transform.position;
        }
        else
        {
            _originalDistance = Vector3.zero;
        }

    }
    
    private void FixedUpdate()
    {
        transform.position = Target.transform.position - _originalDistance;
    }
}
