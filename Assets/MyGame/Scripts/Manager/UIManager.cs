using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : BaseManager<UIManager>
{
    public GameObject cScreen, cHud, cPopup, cNotify, cOverlap, gContainer, gWrapper;
    public Canvas MyCanvas;
    public EventSystem MyEvent;


    private Dictionary<string, BaseScreen> screens = new Dictionary<string, BaseScreen>();
    private Dictionary<string, BaseHud> huds = new Dictionary<string, BaseHud>();
    private Dictionary<string, BasePopup> popups = new Dictionary<string, BasePopup>();
    private Dictionary<string, BaseNotify> notifies = new Dictionary<string, BaseNotify>();
    private Dictionary<string, BaseOverlap> overlaps = new Dictionary<string, BaseOverlap>();

    private List<string> rmScreens = new List<string>();
    private List<string> rmHuds = new List<string>();
    private List<string> rmPopups = new List<string>();
    private List<string> rmNotifies = new List<string>();
    private List<string> rmOverlaps = new List<string>();


    private const string SCREEN_RESOURCES_PATH = "Prefabs/UI/Screen/";
    private const string HUD_RESOURCES_PATH = "Prefabs/UI/Hud/";
    private const string POPUP_RESOURCES_PATH = "Prefabs/UI/Popup/";
    private const string NOTIFY_RESOURCES_PATH = "Prefabs/UI/Notify/";
    private const string OVERLAP_RESOURCES_PATH = "Prefabs/UI/Overlap/";

    private const string NAME_SCREEN_CONTAINER = "CONTAINER_SCREEN";
    private const string NAME_HUD_CONTAINER = "CONTAINER_HUD";
    private const string NAME_POPUP_CONTAINER = "CONTAINER_POPUP";
    private const string NAME_OVERLAP_CONTAINER = "CONTAINER_OVERLAP";
    private const string NAME_NOTIFY_CONTAINER = "CONTAINER_NOTIFY";
    public const string NAME_UI_CONTAINER = "UI_CONTAINER";
    public const string NAME_UI_WRAPPER = "UI_WRAPPER";



    private BaseScreen curScreen;
    private BaseHud curHud;
    private BasePopup curPopup;
    private BaseNotify curNotify;
    private BaseOverlap curOverlap;
    private CanvasScaler myCanvasScaler;


    public CanvasScaler MyCanvasScaler
    {
        get
        {
            if (this.myCanvasScaler == null)
            {
                this.myCanvasScaler = this.MyCanvas.GetComponent<CanvasScaler>();
            }
            return this.myCanvasScaler;
        }
    }

    public Dictionary<string, BaseScreen> Screens { get => screens; }
    public Dictionary<string, BaseHud> Huds { get => huds; }
    public Dictionary<string, BasePopup> Popups { get => popups; }
    public Dictionary<string, BaseNotify> Notifies { get => notifies; }
    public Dictionary<string, BaseOverlap> Overlaps { get => overlaps; }

    public BaseScreen CurScreen { get => curScreen; }
    public BaseHud CurHud { get => curHud; }
    public BasePopup CurPopup { get => curPopup; }
    public BaseNotify CurNotify { get => curNotify; }
    public BaseOverlap CurOverlap { get => curOverlap; }

    public delegate void ListenShowUIElement(UIType type, BaseUIElement target, bool isShow);

    public ListenShowUIElement OnListenShowUIElement;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        this.cScreen.name = NAME_SCREEN_CONTAINER;
        this.cHud.name = NAME_HUD_CONTAINER;
        this.cPopup.name = NAME_POPUP_CONTAINER;
        this.cOverlap.name = NAME_OVERLAP_CONTAINER;
        this.cNotify.name = NAME_NOTIFY_CONTAINER;
        this.gContainer.name = NAME_UI_CONTAINER;
        this.gWrapper.name = NAME_UI_WRAPPER;
#endif

        Release();
        SetRemoveElements();
    }

    void Update()
    {
#if UNITY_EDITOR
        //for testing
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    ShowNotify<NotifyRequestLoading>();
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    ShowToastMessage("helooo ------- ");
        //}
#endif
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ShowNotify<NotifyExit>();
        }
    }

    public void ShowScreen<T>(object data = null, bool forceShow = false) where T : BaseScreen
    {
        string nameScreen = typeof(T).Name;
        BaseScreen result = null;

        if (curScreen != null)
        {
            var curName = curScreen.GetType().Name;
            if (curName.Equals(nameScreen))
            {
                result = curScreen;
            }
            else
            {
                RemoveScreen(curName);
            }
        }

        if (result == null)
        {
            if (!screens.ContainsKey(nameScreen))
            {
                BaseScreen sceneScr = GetNewScreen<T>();
                if (sceneScr != null)
                {
                    screens.Add(nameScreen, sceneScr);
                }
            }

            if (screens.ContainsKey(nameScreen))
            {
                result = screens[nameScreen];
            }
        }

        bool isShow = false;
        if (result != null)
        {
            if (forceShow)
            {
                isShow = true;
            }
            else
            {
                if (result.IsHide)
                {
                    result.Clear();
                    isShow = true;
                }
            }
        }

        if (isShow)
        {
            curScreen = result;
            HideAll();
            result.transform.SetAsLastSibling();
            result.Show(data);

            OnListenShowUIElement?.Invoke(UIType.Screen, result, true);

        }

    }

    private BaseScreen GetNewScreen<T>() where T : BaseScreen
    {
        string nameScreen = typeof(T).Name;
        GameObject pfScreen = GetUIPrefab(UIType.Screen, nameScreen);
        if (pfScreen == null || !pfScreen.GetComponent<BaseScreen>())
        {
            throw new MissingReferenceException("Cant found " + nameScreen + " screen. !!!");
        }

        GameObject ob = Instantiate(pfScreen) as GameObject;
        ob.transform.SetParent(this.cScreen.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "SCREEN_" + nameScreen;
#endif
        BaseScreen sceneScr = ob.GetComponent<BaseScreen>();
        sceneScr.Init();
        return sceneScr;
    }

    public void HideAllScreens()
    {
        BaseScreen sceneScr = null;
        foreach (KeyValuePair<string, BaseScreen> item in screens)
        {
            sceneScr = item.Value;
            if (sceneScr == null || sceneScr.IsHide)
                continue;
            sceneScr.Hide();

            if (screens.Count <= 0)
                break;
        }
    }

    public T GetExistScreen<T>() where T : BaseScreen
    {
        string nameScreen = typeof(T).Name;
        if (screens.ContainsKey(nameScreen))
        {
            return screens[nameScreen] as T;
        }
        return null;
    }

    public void ShowHud<T>(object data = null, bool forceShow = false) where T : BaseHud
    {
        string nameHud = typeof(T).Name;
        BaseHud result = null;

        if (curHud != null)
        {
            var curName = curNotify.GetType().Name;
            if (curName.Equals(nameHud))
            {
                result = curHud;
            }
            else
            {
                RemoveHud(curName);
            }
        }

        if (result == null)
        {
            if (!huds.ContainsKey(nameHud))
            {
                BaseHud hudScr = GetNewHud<T>();
                if (hudScr != null)
                {
                    huds.Add(nameHud, hudScr);
                }
            }
            if (huds.ContainsKey(nameHud))
            {
                result = huds[nameHud];
            }
        }

        bool isShow = false;
        if (result != null)
        {
            if (forceShow)
            {
                isShow = true;
            }
            else
            {
                if (result.IsHide)
                {
                    result.Clear();
                    isShow = true;
                }
            }
        }

        if (isShow)
        {
            curHud = result;
            result.transform.SetAsLastSibling();
            result.Show(data);

            OnListenShowUIElement?.Invoke(UIType.Hud, result, true);
        }
    }

    private BaseHud GetNewHud<T>() where T : BaseHud
    {
        string nameHud = typeof(T).Name;
        GameObject pfHud = GetUIPrefab(UIType.Hud, nameHud);
        if (pfHud == null || !pfHud.GetComponent<BaseHud>())
        {
            throw new MissingReferenceException("Cant found " + nameHud + " hud. !!!");
        }

        GameObject ob = Instantiate(pfHud) as GameObject;
        ob.transform.SetParent(this.cHud.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "HUD_" + nameHud;
#endif
        BaseHud hudScr = ob.GetComponent<BaseHud>();
        hudScr.Init();
        return hudScr;
    }

    public void HideAllHuds()
    {
        BaseHud hudScr = null;
        foreach (KeyValuePair<string, BaseHud> item in huds)
        {
            hudScr = item.Value;
            if (hudScr == null || hudScr.IsHide)
                continue;
            hudScr.Hide();
            if (huds.Count <= 0)
                break;
        }
    }

    public T GetExistHud<T>() where T : BaseHud
    {
        string hudName = typeof(T).Name;
        if (huds.ContainsKey(hudName))
        {
            return huds[hudName] as T;
        }
        return null;
    }

    public void HideHud<T>(bool force = false) where T : BaseHud
    {
        string hudName = typeof(T).Name;
        if (huds.ContainsKey(hudName))
        {
            if (force || !huds[hudName].IsHide)
            {
                huds[hudName].Hide();
            }
        }
    }

    public void ShowPopup<T>(object data = null, bool forceShow = false) where T : BasePopup
    {
        string namePopup = typeof(T).Name;
        BasePopup result = null;

        if (curPopup != null)
        {
            var curName = curPopup.GetType().Name;
            if (curName.Equals(namePopup))
            {
                result = curPopup;
            }
            else
            {
                RemovePopup(curName);
            }
        }

        if (result == null)
        {
            if (!popups.ContainsKey(namePopup))
            {
                BasePopup popupScr = GetNewPopup<T>();
                if (popupScr != null)
                {
                    popups.Add(namePopup, popupScr);
                }
            }
            if (popups.ContainsKey(namePopup))
            {
                result = popups[namePopup];
            }
        }

        bool isShow = false;
        if (result != null)
        {
            if (forceShow)
            {
                isShow = true;
            }
            else
            {
                if (result.IsHide)
                {
                    result.Clear();
                    isShow = true;
                }
            }
        }

        if (isShow)
        {
            if (this.curScreen != null)
            {
                this.curScreen.OnBackgroundState(true, result);
            }
            curPopup = result;
            HideAllPopups();
            result.transform.SetAsLastSibling();
            result.Show(data);

            OnListenShowUIElement?.Invoke(UIType.Popup, result, true);
        }
    }

    private BasePopup GetNewPopup<T>() where T : BasePopup
    {
        string namePopup = typeof(T).Name;
        GameObject pfPopup = GetUIPrefab(UIType.Popup, namePopup);
        if (pfPopup == null || !pfPopup.GetComponent<BasePopup>())
        {
            throw new MissingReferenceException("Cant found " + namePopup + " popup. !!!");
        }

        GameObject ob = Instantiate(pfPopup) as GameObject;
        ob.transform.SetParent(this.cPopup.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "POPUP_" + namePopup;
#endif
        BasePopup popupScr = ob.GetComponent<BasePopup>();
        popupScr.Init();
        return popupScr;
    }

    public void HidePopup<T>(bool force = false) where T : BasePopup
    {
        string popupName = typeof(T).Name;
        if (popups.ContainsKey(popupName))
        {
            if (force || !popups[popupName].IsHide)
            {
                popups[popupName].Hide();
            }
        }
    }

    public void HideAllPopups()
    {
        BasePopup scr = null;
        foreach (KeyValuePair<string, BasePopup> item in popups)
        {
            scr = item.Value;
            if (scr == null || scr.IsHide)
                continue;
            scr.Hide();

            if (popups.Count <= 0)
                break;
        }
    }

    public T GetExistPopup<T>() where T : BasePopup
    {
        string namePopup = typeof(T).Name;
        if (popups.ContainsKey(namePopup))
        {
            return popups[namePopup] as T;
        }
        return null;
    }

    public void ShowNotify<T>(object data = null, bool forceShow = false) where T : BaseNotify
    {
        string nameNotify = typeof(T).Name;
        BaseNotify result = null;

        if (curNotify != null)
        {
            var curName = curNotify.GetType().Name;
            if (curName.Equals(nameNotify))
            {
                result = curNotify;
            }
            else
            {
                RemoveNotify(curName);
            }
        }

        if (result == null)
        {
            if (!notifies.ContainsKey(nameNotify))
            {
                BaseNotify notifyScr = GetNewNotify<T>();
                if (notifyScr != null)
                {
                    notifies.Add(nameNotify, notifyScr);
                }
            }
            if (notifies.ContainsKey(nameNotify))
            {
                result = notifies[nameNotify];
            }
        }

        bool isShow = false;
        if (result != null)
        {
            if (forceShow)
            {
                isShow = true;
            }
            else
            {
                if (result.IsHide)
                {
                    result.Clear();
                    isShow = true;
                }
            }
        }

        if (isShow)
        {
            curNotify = result;
            result.transform.SetAsLastSibling();
            result.Show(data);

            OnListenShowUIElement?.Invoke(UIType.Notify, result, true);
        }
    }

    private BaseNotify GetNewNotify<T>() where T : BaseNotify
    {
        string nameNotify = typeof(T).Name;
        GameObject pfNotify = GetUIPrefab(UIType.Notify, nameNotify);
        if (pfNotify == null || !pfNotify.GetComponent<BaseNotify>())
        {
            throw new MissingReferenceException("Cant found " + nameNotify + " notify. !!!");
        }

        GameObject ob = Instantiate(pfNotify) as GameObject;
        ob.transform.SetParent(this.cNotify.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "NOTIFY_" + nameNotify;
#endif
        BaseNotify notifyScr = ob.GetComponent<BaseNotify>();
        notifyScr.Init();
        return notifyScr;
    }

    public void HideAllNotifys()
    {
        BaseNotify notifyScr = null;
        foreach (KeyValuePair<string, BaseNotify> item in notifies)
        {
            notifyScr = item.Value;
            if (notifyScr == null || notifyScr.IsHide)
                continue;
            notifyScr.Hide();
            if (notifies.Count <= 0)
                break;
        }
    }

    public T GetExistNotify<T>() where T : BaseNotify
    {
        string notifyName = typeof(T).Name;
        if (notifies.ContainsKey(notifyName))
        {
            return notifies[notifyName] as T;
        }
        return null;
    }

    public void HideNotify<T>(bool force = false) where T : BaseNotify
    {
        string notifyName = typeof(T).Name;
        if (notifies.ContainsKey(notifyName))
        {
            if (force || !notifies[notifyName].IsHide)
            {
                notifies[notifyName].Hide();
            }
        }
    }

    public void ShowOverlap<T>(object data = null, bool force = false) where T : BaseOverlap
    {
        string overlapName = typeof(T).Name;
        BaseOverlap result = null;

        if (curOverlap != null)
        {
            var curName = curOverlap.GetType().Name;
            if (curName.Equals(overlapName))
            {
                result = curOverlap;
            }
            else
            {
                RemoveOverlap(curName);
            }
        }

        if (result == null)
        {
            if (!overlaps.ContainsKey(overlapName))
            {
                BaseOverlap scr = GetNewOverlap<T>();
                if (overlaps != null)
                {
                    overlaps.Add(overlapName, scr);
                }
            }
            if (overlaps.ContainsKey(overlapName))
            {
                result = overlaps[overlapName];
            }
        }

        if (result != null && (result.IsHide || force))
        {
            result.Clear();
            curOverlap = result;
            result.transform.SetAsLastSibling();
            result.Show(data);

            OnListenShowUIElement?.Invoke(UIType.Overlap, result, true);

        }
    }

    private BaseOverlap GetNewOverlap<T>() where T : BaseOverlap
    {
        string overlapName = typeof(T).Name;
        GameObject pfOverlap = GetUIPrefab(UIType.Overlap, overlapName);
        if (pfOverlap == null || !pfOverlap.GetComponent<BaseOverlap>())
        {
            throw new MissingReferenceException("Cant found " + overlapName + " overlap. !!!");
        }

        GameObject ob = Instantiate(pfOverlap) as GameObject;
        ob.transform.SetParent(this.cOverlap.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "OVERLAP_" + overlapName;
#endif
        BaseOverlap scr = ob.GetComponent<BaseOverlap>();
        scr.Init();
        return scr;
    }

    public void HideAllOverlaps()
    {
        BaseOverlap scr = null;
        foreach (KeyValuePair<string, BaseOverlap> item in overlaps)
        {
            scr = item.Value;
            if (scr == null || scr.IsHide)
                continue;
            scr.Hide();
        }
    }

    public T GetExistOverlap<T>() where T : BaseOverlap
    {
        string overlapName = typeof(T).Name;
        if (overlaps.ContainsKey(overlapName))
        {
            return overlaps[overlapName] as T;
        }
        return null;
    }



    public void HideOverlap<T>(bool force = false) where T : BaseOverlap
    {
        string overlapName = typeof(T).Name;
        if (overlaps.ContainsKey(overlapName))
        {
            if (force || !overlaps[overlapName].IsHide)
            {
                overlaps[overlapName].Hide();
            }
        }
    }

    public void Release()
    {
        ReleaseScreen();
        ReleaseHud();
        ReleasePopup();
        ReleaseNotify();
        ReleaseOverlap();
    }

    public void HideAll()
    {
        HideAllScreens();
        HideAllHuds();
        HideAllPopups();
        HideAllNotifys();
        HideAllOverlaps();
    }

    public void ReleaseScreen()
    {
        this.screens.Clear();
        int index = 0;
        while (this.cScreen.transform.childCount > 0)
        {
            Destroy(this.cScreen.transform.GetChild(index).gameObject);
            index++;
            if (this.cScreen.transform.childCount <= index)
                break;
        }
    }

    public void ReleaseHud()
    {
        this.huds.Clear();
        int index = 0;
        while (this.cHud.transform.childCount > 0)
        {
            Destroy(this.cHud.transform.GetChild(index).gameObject);
            index++;
            if (this.cHud.transform.childCount <= index)
                break;
        }
    }

    public void ReleasePopup()
    {
        this.popups.Clear();
        int index = 0;
        while (this.cPopup.transform.childCount > 0)
        {
            Destroy(this.cPopup.transform.GetChild(index).gameObject);
            index++;
            if (this.cPopup.transform.childCount <= index)
                break;
        }
    }

    public void ReleaseNotify()
    {
        this.notifies.Clear();
        int index = 0;
        while (this.cNotify.transform.childCount > 0)
        {
            Destroy(this.cNotify.transform.GetChild(index).gameObject);
            index++;
            if (this.cNotify.transform.childCount <= index)
                break;
        }
    }

    private void ReleaseOverlap()
    {
        this.overlaps.Clear();
        int index = 0;
        while (this.cOverlap.transform.childCount > 0)
        {
            Destroy(this.cOverlap.transform.GetChild(index).gameObject);
            index++;
            if (this.cOverlap.transform.childCount <= index)
                break;
        }
    }

    private void SetRemoveElements()
    {
        //// screen
        //rmScreens.Add(nameof(ScreenMenu));

        // popup
        //rmPopups.Add(nameof(PopupAvatar));
        //rmPopups.Add(nameof(PopupHelp));

        // notify
        //rmNotifies.Add(nameof(NotifyHomeLoading));

    }

    private void RemoveScreen(string v)
    {
        for (int i = 0; i < rmScreens.Count; i++)
        {
            if (rmScreens[i].Equals(v))
            {
                if (screens.ContainsKey(v))
                {
                    screens[v].Remove();
                    Destroy(screens[v].gameObject);
                    screens.Remove(v);

                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
                break;
            }
        }
    }

    private void RemoveHud(string v)
    {
        for (int i = 0; i < rmHuds.Count; i++)
        {
            if (rmHuds[i].Equals(v))
            {
                if (huds.ContainsKey(v))
                {
                    huds[v].Remove();
                    Destroy(huds[v].gameObject);
                    huds.Remove(v);
                }
                break;
            }
        }
    }

    private void RemovePopup(string v)
    {
        for (int i = 0; i < rmPopups.Count; i++)
        {
            if (rmPopups[i].Equals(v))
            {
                if (popups.ContainsKey(v))
                {
                    popups[v].Remove();
                    Destroy(popups[v].gameObject);
                    popups.Remove(v);
                }
                break;
            }
        }
    }

    private void RemoveNotify(string v)
    {
        for (int i = 0; i < rmNotifies.Count; i++)
        {
            if (rmNotifies[i].Equals(v))
            {
                if (notifies.ContainsKey(v))
                {
                    notifies[v].Remove();
                    Destroy(notifies[v].gameObject);
                    notifies.Remove(v);
                }
                break;
            }
        }
    }
    private void RemoveOverlap(string v)
    {
        for (int i = 0; i < rmOverlaps.Count; i++)
        {
            if (rmOverlaps[i].Equals(v))
            {
                if (overlaps.ContainsKey(v))
                {
                    overlaps[v].Remove();
                    overlaps.Remove(v);
                    Destroy(overlaps[v].gameObject);
                }
                break;
            }
        }
    }

    public void ShowToastMessage(string msg)
    {
        //ShowNotify<NotifyToast>();
        //NotifyToast scr = GetExistNotify<NotifyToast>();
        //if (scr != null)
        //{
        //    scr.ShowMesage(msg, 2f, 150f);
        //}
    }

    private GameObject GetUIPrefab(UIType t, string uiName)
    {
        GameObject result = null;

        //var curPlatform = HomeManager.Instance.ComonyPlatform;

        var defaultPath = "";
        // check resource path by platform
        if (result == null)
        {
            switch (t)
            {
                case UIType.Screen:
                    {
                        defaultPath = SCREEN_RESOURCES_PATH + uiName;
                    }
                    break;
                case UIType.Hud:
                    {
                        defaultPath = HUD_RESOURCES_PATH + uiName;
                    }
                    break;
                case UIType.Popup:
                    {
                        defaultPath = POPUP_RESOURCES_PATH + uiName;
                    }
                    break;
                case UIType.Notify:
                    {
                        defaultPath = NOTIFY_RESOURCES_PATH + uiName;
                    }
                    break;
                case UIType.Overlap:
                    {
                        defaultPath = OVERLAP_RESOURCES_PATH + uiName;
                    }
                    break;
            }
            //var platformPath = defaultPath + curPlatform;
            //result = Resources.Load(platformPath) as GameObject;
        }

        // check resource path with default path
        if (result == null)
        {
            result = Resources.Load(defaultPath) as GameObject;
        }

        return result;
    }

    public void HideElementUI(BaseUIElement v)
    {
        string elementUiName = v.GetType().Name;

        switch (v.UIType)
        {
            case UIType.Screen:
                {
                    RemoveScreen(elementUiName);
                }
                break;
            case UIType.Popup:
                {
                    RemovePopup(elementUiName);
                }
                break;
            case UIType.Notify:
                {
                    RemoveNotify(elementUiName);
                }
                break;
            case UIType.Overlap:
                {
                    RemoveOverlap(elementUiName);
                }
                break;
        }

        OnListenShowUIElement?.Invoke(v.UIType, v, false);


        if (this.curScreen != null)
        {
            if (v.UIType == UIType.Popup)
            {
                this.curScreen.OnBackgroundState(false, v);
            }
        }
    }
}
