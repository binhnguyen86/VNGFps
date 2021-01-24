using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeHandler : MonoBehaviour {

    public delegate void OnEscapeHandling();
    public OnEscapeHandling EscapeHandling;

    public static EscapeHandler Instance;

    private void OnEnable()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(EscapeHandling != null)
                EscapeHandling();
            else
            {
                //GameSettings.Instance.PauseGame();
                //MessageManager.instance.ShowConfirmBox("Quit game", "Are you sure to quit?", "Yes");
                //MessageManager.instance.yesBtn.onClick.AddListener(() => Application.Quit());
                //MessageManager.instance.CloseBtn.onClick.AddListener(() => GameSettings.Instance.ResumeGame());
                Debug.Log("pause game");
            }
        }
    }
}
