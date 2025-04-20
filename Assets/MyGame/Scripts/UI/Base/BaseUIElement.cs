using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUIElement : MonoBehaviour
{
    public RectTransform rtWrapper;

    protected UIListenerElement listener;
    protected CanvasGroup canvasGroup;
    protected UIType uiType = UIType.Unknown;
    protected bool isHide;

    private bool isInited;

    public bool IsHide { get => isHide; }
    public CanvasGroup CanvasGroup { get => canvasGroup; }
    public bool IsInited { get => isInited; }
    public UIType UIType { get => uiType; }

    public virtual void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickedBackButton();
        }
#elif UNITY_IOS
        //if (HomeManager.Instance.IsIOSAppOnMac)
        //{
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        OnClickedBackButton();
        //    }
        //}
#endif
    }

    public virtual void Init()
    {
        this.isInited = true;
        if (!this.gameObject.GetComponent<CanvasGroup>())
        {
            this.gameObject.AddComponent<CanvasGroup>();
        }
        this.canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        this.listener = this.gameObject.GetComponent<UIListenerElement>();

        if (this.listener != null)
        {
            this.listener.Init();
        }
        this.gameObject.SetActive(true);


        // remove for design
        if (this.rtWrapper != null)
        {
            SetStrechParent(this.rtWrapper);
            var img = this.rtWrapper.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = null;
                img.enabled = false;
            }
        }

        Hide();
    }

    public virtual void Show(object data)
    {
        this.gameObject.SetActive(true);
        this.isHide = false;
        SetActiveGroupCanvas(true);
        if (this.listener != null)
        {
            this.listener.Show();
        }
    }

    public virtual void Hide()
    {
        this.isHide = true;
        SetActiveGroupCanvas(false);
        if (this.listener != null)
        {
            this.listener.Hide();
        }

        if (UIManager.HasInstance)
        {
            UIManager.Instance.HideElementUI(this);
        }
    }

    public virtual void Remove()
    {
        if (this.listener != null)
        {
            this.listener.Remove();
        }
    }

    public virtual void Clear()
    {
        if (this.listener != null)
        {
            this.listener.Clear();
        }
    }

    private void SetActiveGroupCanvas(bool isAct)
    {
        if (CanvasGroup != null)
        {
            CanvasGroup.blocksRaycasts = isAct;
            CanvasGroup.alpha = isAct ? 1 : 0;
        }
    }

    public virtual void OnClickedBackButton()
    {

    }

    protected void SetStrechParent(RectTransform v)
    {
        v.SetAnchor(AnchorPresets.StretchAll);
        v.SetPivot(PivotPresets.MiddleCenter);
        v.offsetMax = Vector2.zero;
        v.offsetMin = Vector2.zero;
    }

}
