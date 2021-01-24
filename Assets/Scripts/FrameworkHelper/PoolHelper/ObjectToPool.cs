using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToPool : MonoBehaviour {
    
    public virtual void GetItem(ObjectToPool item){
        gameObject.SetActive(true);
    }

    public virtual void ReleaseItem(ObjectToPool item){
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Call by animator or time
    /// </summary>
    public virtual void SelfDespawn()
    {
        GameController.Instance.SpawnManager.ReleasePoolObject(this);
    }

    public virtual void StartSelfDespawnAfter(float duration)
    {
        StartCoroutine(SelfDespawnAfter(duration));
    }

    public virtual IEnumerator SelfDespawnAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        SelfDespawn();
    }
    
}
