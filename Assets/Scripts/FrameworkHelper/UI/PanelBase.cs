using UnityEngine;
using DG.Tweening;
using System;

public abstract class PanelBase : MonoBehaviour {

    public CanvasGroup CanvasGroup { get; set; }    

    public bool BlocksRaycasts
    {
        get
        {
            return CanvasGroup.blocksRaycasts;
        }
        set
        {
            if(CanvasGroup.blocksRaycasts != value)
            {
                CanvasGroup.blocksRaycasts = value;
            }
        }
    }

    public bool Interactable
    {
        get
        {
            return CanvasGroup.interactable;
        }
        set
        {
            if (CanvasGroup.interactable != value)
            {
                CanvasGroup.interactable = value;
            }
        }
    }

    public float Alpha
    {
        get
        {
            return CanvasGroup.alpha;
        }
        set
        {
            if (CanvasGroup.alpha != value)
            {
                CanvasGroup.alpha = value;
            }
        }
    }

    public abstract void Refresh();

    public virtual void Close(
        float sp = 1, 
        Action callback = null)
    {
        BlocksRaycasts = false;
        Interactable = false;
        CanvasGroup.DOFade(0, sp)
            .OnComplete(() =>
            {
                if( callback  != null )
                {
                    callback();
                }
            })
            .Play();
    }

    public virtual void Open(float sp = 1)
    {
        BlocksRaycasts = true;
        Interactable = true;
        CanvasGroup
            .DOFade(1, sp)
            .Play();
        Refresh();
    }
    
    /// <summary>
    /// Call by GUIManager to init all panel
    /// </summary>
    public virtual void Init()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        CanvasGroup.alpha = 0;
        BlocksRaycasts = false;
        Interactable = false;
    }
    
}
