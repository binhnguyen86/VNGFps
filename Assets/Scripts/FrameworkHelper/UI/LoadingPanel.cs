using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Core
{
    public class LoadingPanel : MonoBehaviour
    {
        #region Spine function
#if SPINE
        [SerializeField]
        private SkeletonGraphic _anim = null;

        private float _currentProgress = 0;
        float _startTimeScale = 0;
#else

        [SerializeField]
        private Image _progressBarValue = null;

        [SerializeField]
        private Text _progressBarValuePercent = null;

        private float _lastValueProgressValue;
#endif
        #endregion
        [SerializeField]
        private Text _progressBarMessage = null;

        [SerializeField]
        private CanvasGroup _canvasGroup = null;

        [SerializeField]
        private CanvasGroup _containerCanvas = null;
        
        [SerializeField]
        private CanvasGroup _blurBg = null;

        [SerializeField]
        private Image _bgImage = null;

        public bool IsCompleted { get; private set; }

#if SPINE
        private IEnumerator Start()
        {

            while ( _anim.AnimationState == null )
            {
                yield return null;
            }
            _anim.AnimationState.Event += HandleEvent;
            _anim.AnimationState.Complete += HandleComplete;
            _startTimeScale = _anim.timeScale;

        }
#endif

        #region UI
        public void ShowProgressLoading(
            float value, 
            float duration, 
            System.Action callback = null)
        {
#if !SPINE
            _bgImage
                .DOFade(1, 0)
                .Play();
            if (_lastValueProgressValue < value)
            {
                _progressBarValue.DOFillAmount(value, duration)
                    .OnComplete(() =>
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                        HandleComplete();
                    })
                    .Play();
                _lastValueProgressValue = value;
                _progressBarValuePercent.text = "Loading..." + value * 100 + "%";
            }

#else
            if ( _currentProgress < value )
            {
                PlayAnim();
                _currentProgress = value;
            }
#endif

            if (_canvasGroup.alpha == 0 || _containerCanvas.alpha == 0)
            {
                IsCompleted = false;
                Show();
            }
        }
#if SPINE
        protected virtual void HandleEvent(Spine.TrackEntry entry, Spine.Event e)
        {
            if ( entry != null )
            {
                switch ( e.Data.Name )
                {
                    case "Pause":
                        StopAnim();
                        break;
                }
            }
        }

        protected virtual void HandleComplete(Spine.TrackEntry entry)
        {
            if ( entry != null )
            {
                Hide(0);
                IsCompleted = true;
            }
        }


        public void PlayAnim()
        {
            _anim.timeScale = _startTimeScale;
        }

        public void StopAnim()
        {
            if ( _currentProgress < 1 )
            {
                _anim.timeScale = 0;
            }
        }
#endif

        public void ShowMessageLoading(bool isProcessing, string textInfo)
        {
            _progressBarMessage.text = textInfo;
        }

        public void Show()
        {
            _containerCanvas.alpha = 1;
            _canvasGroup.DOFade(1, 1).Play();
            _canvasGroup.blocksRaycasts = true;
            _progressBarMessage.text = string.Empty;
        }

        public void Hide(float duration = 0.25f)
        {
            _blurBg
                .DOFade(0, duration)
                .Play();

            _canvasGroup.blocksRaycasts = false;
            _canvasGroup
                .DOFade(0, duration)
                .OnComplete(() =>
                {
                    _progressBarValue.fillAmount = 0;
                })
                .Play();
            _progressBarMessage.text = string.Empty;
        }

        public void ReloadScence()
        {
            _containerCanvas.DOFade(0, 1).OnComplete(() =>
            {
                _progressBarMessage.DOText(string.Empty, 1.5f).OnComplete(() =>
                {
                    // Todo add reload function here.
                }).Play();
            }).Play();

        }

        protected virtual void HandleComplete()
        {
            IsCompleted = true;
            _lastValueProgressValue = 0;
            Hide(0);
        }

        public void BlurScreen()
        {
            _bgImage
                .DOFade(0, 0)
                .Play();

            _blurBg
                .DOFade(1, 2)
                .Play();
        }
        #endregion
    }
}
