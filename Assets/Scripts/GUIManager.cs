using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FrameworkdHelper;
using UnityEngine.Playables;

public class GUIManager : Singleton<GUIManager>
{
    [Header("Panels")]
    public PanelBase[] Panels; // setup right oder enum Panel to get right panel

    [Header("References")]
    //public Text _goldText;
    public LoadingPanel LoadingCtrl;

    [SerializeField] 
    private PlayableDirector _introPlayableDirector;

    [SerializeField]
    private PlayableDirector _deathPlayableDirector;

    public static WaitForSecondsRealtime WaitASecondUnScale;
    public static WaitForSeconds WaitXSecond;
    public static WaitForSeconds WaitNextLevelInSecond;
    public bool IsAllAnimCompleted
    {
        get
        {
            return LoadingCtrl.IsCompleted;
        }
    }

    public enum Panel {
        MainMenu
    }

    public PanelBase CurrentPanel { get; private set; }

    private static Player Player
    {
        get
        {
            return GameController.Instance.Player;
        }
    }

    public void Start()
    {
        WaitASecondUnScale = new WaitForSecondsRealtime(1);
        WaitXSecond = new WaitForSeconds(2);
        WaitNextLevelInSecond = new WaitForSeconds(8);
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].Init();
        }
        LoadingCtrl.ShowProgressLoading(1, 2, () =>
        {
            OpenPanel(Panel.MainMenu);
        });
        _introPlayableDirector.gameObject.SetActive(true);
        _deathPlayableDirector.gameObject.SetActive(false);
        _deathPlayableDirector.Pause();
    }
    
    public void OpenPanel(PanelBase panel)
    {
        if (CurrentPanel != null)
            ClosePanel(CurrentPanel);
        CurrentPanel = panel;
        GameController.Instance.PauseGame();           
        EscapeHandler.Instance.EscapeHandling += ClosePanel;
        panel.Open();        
    }

    public void ClosePanel(PanelBase panel)
    {
        CurrentPanel = null;
        GameController.Instance.ResumeGame();
        EscapeHandler.Instance.EscapeHandling -= ClosePanel;
        panel.Close(0.5f);
    }

    public void ClosePanel()
    {
        if(CurrentPanel == null)
        {
            return;
        }
        CurrentPanel.Close(0.5f);
        CurrentPanel = null;
        EscapeHandler.Instance.EscapeHandling -= ClosePanel;
        GameController.Instance.ResumeGame();
    }

    public void OpenPanel(Panel panel)
    {        
        OpenPanel(Panels[panel.GetHashCode()]);
    }
    
    public void PlayGame()
    {
        ClosePanel();
        ResetAndHideIntro();
        GameController.Instance.PlayGame();
    }

    public void ResetAndHideIntro()
    {
        _introPlayableDirector.Pause();
        _introPlayableDirector.time = -1.5f;
        _introPlayableDirector.gameObject.SetActive(false);
    }

    public void ShowIntro()
    {
        _introPlayableDirector.gameObject.SetActive(true);
        _introPlayableDirector.Resume();
    }

    public void GameOver()
    {
        ShowDeathCutScene();
        StartCoroutine(StartBlurScreen());
    }

    private void ShowDeathCutScene()
    {
        _deathPlayableDirector.time = -0.5f;
        _deathPlayableDirector.transform.position = Player.Position;
        _deathPlayableDirector.gameObject.SetActive(true);
        _deathPlayableDirector.Resume();
    }

    private void ShowMainMenu()
    {
        GameController.Instance.SpawnManager.DespawnAllActiveObjectPools();
        GameController.Instance.ResetLevelPosition();
        _deathPlayableDirector.gameObject.SetActive(false);
        _deathPlayableDirector.Pause();
        LoadingCtrl.ShowProgressLoading(1, 2, () =>
        {
            OpenPanel(Panel.MainMenu);
        });
        ShowIntro();
    }

    private IEnumerator StartBlurScreen()
    {
        yield return WaitXSecond;
        LoadingCtrl.BlurScreen();
        yield return WaitXSecond;
        ShowMainMenu();
    }


#if UNITY_EDITOR
    [ContextMenu("PauseIntro")]
    public void PauseIntroMenuEditor()
    {
        _introPlayableDirector.Pause();
    }

    [ContextMenu("ResetIntro")]
    public void ResetIntroMenuEditor()
    {
        _introPlayableDirector.time = -1.5f;
    }

    [ContextMenu("ResumeIntro")]
    public void ResumeIntroMenuEditor()
    {
        _introPlayableDirector.Resume();
    }
#endif

}
