using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : PanelBase
{

    [SerializeField] 
    private Ease _easeType;

    [SerializeField] 
    private Vector2 _outPosition;

    [SerializeField] 
    private Vector2 _inPosition;

    [SerializeField]
    private Button PlayButton;

    [SerializeField]
    private RectTransform _rectTransform;
    
    public override void Init()
    {
        base.Init();
        Alpha = 1;
        PlayButton.onClick.AddListener(PlayGame);
        _rectTransform.anchoredPosition = _outPosition;
    }

    public override void Refresh()
    {
        //Debug.Log(transform.position);
    }

    public override void Close(
        float sp = 1,
        Action callback = null)
    {
        BlocksRaycasts = false;
        Interactable = false;
        _rectTransform
            .DOAnchorPos(_outPosition, sp, true)
            .SetEase(_easeType)
            .OnComplete(() =>
            {
                if (callback != null)
                {
                    callback();
                }
            })
            .Play();
    }

    public override void Open(float sp = 1)
    {
        BlocksRaycasts = true;
        Interactable = true;
        _rectTransform
            .DOAnchorPos(_inPosition, sp, true)
            .SetEase(_easeType)
            .Play();
        Refresh();
    }

    public void PlayGame()
    {
        GUIManager.Instance.PlayGame();
    }


#if UNITY_EDITOR
    [ContextMenu("OpenPanel")]
    public void OpenPanel()
    {
        Open(3);
        
    }

    [ContextMenu("ClosePanel")]
    public void ClosePanel()
    {
        Close(3);
    }
#endif
}
