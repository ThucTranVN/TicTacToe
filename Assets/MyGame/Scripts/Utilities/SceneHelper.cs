using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelper : MonoBehaviour
{
    private class SceneInfo
    {
        public Queue<string> scenes;
        public Action onBegin;
        public Action onFinish;
        public float delayFinish;

        public SceneInfo()
        {
            scenes = new Queue<string>();
        }
    }
    
    private static SceneHelper m_api;
    private static SceneHelper api
    {
        get
        {
            if (m_api == null)
            {
                GameObject go = new GameObject();
                DontDestroyOnLoad(go);
                go.name = "LoadSceneHelper";
                m_api = go.AddComponent<SceneHelper>();
                m_api.Init();
            }

            return m_api;
        }
    }

    private Queue<SceneInfo> m_queueLoad; //queue scene need load
    private Queue<string> m_queueUnload; //queue scene need unload
    private bool m_load;
    private bool m_unload;

    //cache last state
    private List<string> m_lastState;
    private List<string> m_listActivedSceneInQueue;
    private List<string> m_cachedLatestState;

    //const
    private const float CONST_TRANSITION_DURATION = 0.8f;
    private const float CONST_TRANSITION_SCALE = 0.9f;
    private const Ease CONST_TRANSITION_EASE = Ease.OutQuart;

    public static string[] GetCurStateScenes()
    {
        List<string> scenes = new List<string>();
        for (int i = SceneManager.sceneCount - 1; i > -1; i--)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s == SceneManager.GetSceneByBuildIndex(0))
                continue;
            scenes.Add(s.name);
            //Debug.Log(s.name);
        }
        return scenes.ToArray();
    }

    //load scene additive, other loaded scene will be unload
    public static void LoadScene(bool isBackState = false, params string[] scenes)
    {
        LoadSceneAdditive(forceUnloadAll: true, callback: null, isAssetBundle: false, isBackState: isBackState, scenes: scenes);
    }

    //load scene additive with callback, other loaded scene will be unload
    public static void LoadScene(Action callback, bool isBackState = false, params string[] scenes)
    {
        LoadSceneAdditive(forceUnloadAll: true, callback: callback, isAssetBundle: false, isBackState: isBackState, scenes: scenes);
    }

    //load scene additive with callback, other loaded scene will not be unload
    public static void LoadSceneAdditive(Action callback, bool isBackState = false, bool isAssetBundle = false, params string[] scenes)
    {
        LoadSceneAdditive(forceUnloadAll: false, callback: callback, isAssetBundle: isAssetBundle, isBackState: isBackState, scenes: scenes) ;
    }

    public static void LoadSceneAdditive(bool forceUnloadAll, Action callback, bool isAssetBundle, bool isBackState = false, params string[] scenes)
    {
        if(scenes.All(x => !IsSceneExist(x,isAssetBundle)))
            return;
        
        if(forceUnloadAll)
        {
            DoLoadSceneAdditive(
                onBegin: null,
                onFinish: () => {
                    callback?.Invoke();
                    UnloadAllScenes(isBackState: isBackState, ignoreScenes: scenes);
                },
                delayFinish: 0f,
                isAssetBundle: isAssetBundle,
                isBackState: isBackState,
                scenes: scenes
            );
        }
        else
        {
            DoLoadSceneAdditive(
                onBegin: null,
                onFinish: callback,
                delayFinish: 0f,
                isAssetBundle: isAssetBundle,
                isBackState: isBackState,
                scenes: scenes
            );
        }
    }

    private static void DoLoadSceneAdditive(Action onBegin, Action onFinish, float delayFinish, bool isAssetBundle, bool isBackState = false, params string[] scenes)
    {
        SceneInfo info = new SceneInfo
        {
            onBegin = onBegin,
            onFinish = onFinish,
            delayFinish = delayFinish
        };
        foreach (string s in scenes)
        {
            if(!IsSceneExist(s,isAssetBundle))
                continue;

            if (CheckLoaded(s))
            {
                Debug.Log("Scene loaded:" + s); 
                SceneManager.UnloadSceneAsync(s);
            }

            info.scenes.Enqueue(s);
        }
        //
        api.m_queueLoad.Enqueue(info);
        //
        if (!api.m_load && api.m_queueLoad.Count > 0) //start coroutine if not start
        {
            api.StartCoroutine(api.Load(isBackState));
        }
    }

    //unload scene
    public static void UnloadScene(Action callback = null, bool isBackState = false, params string[] scene)
    {
        DoUnloadSceneAdditive(callback, false, isBackState, scene);
    }

    //unload scene
    private static void DoUnloadSceneAdditive(Action callback, bool cache, bool isBackState, params string[] scene)
    {
        foreach (string s in scene)
        {
            if (CheckLoaded(s))
            {
                api.m_queueUnload.Enqueue(s);
                if (cache)
                {
                    api.m_lastState.Add(s);
                }
            }
            else
            {
                Debug.Log("Scene not loaded:" + s);
            }
        }

        if (!api.m_unload && m_api.m_queueUnload.Count > 0) //start coroutine if not start
        {
            api.StartCoroutine(api.Unload(isBackState, callback));
        }
        else
        {
            callback?.Invoke();
        }
    }

    public static void UnloadAllScenes(Action callback = null, bool isBackState = false, params string[] ignoreScenes)
    {
        List<string> scenes = new List<string>();
        for (int i = SceneManager.sceneCount - 1; i > -1; i--)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (ignoreScenes.Contains(s.name) || s == SceneManager.GetSceneByBuildIndex(0))
            {
                continue;
            }
            scenes.Add(s.name);
        }
        api.m_lastState.Clear();
        DoUnloadSceneAdditive(callback, true, isBackState, scenes.ToArray());
    }

    public static void BackState(bool useCached = false)
    {
        // Will only use cached backstate when indicated and has value else will just use original backstate
        string[] lastState = useCached && api.m_cachedLatestState.Count > 0 ? api.m_cachedLatestState.ToArray() : api.m_lastState.ToArray();

        LoadScene(isBackState: true, scenes: lastState);

        api.m_lastState.Clear();

        // Don't clear unless indicated to prevent sub pages from clearing cache when they use backstate
        if (useCached)
            api.m_cachedLatestState.Clear();
    }

    public static string[] GetListBackStateScenes (bool useCached = false, bool forceClearCache = true)
    {
        // Will only use cached backstate when indicated and has value else will just use original backstate
        string[] lastState = useCached && api.m_cachedLatestState.Count > 0 ? api.m_cachedLatestState.ToArray() : api.m_lastState.ToArray();

        if (forceClearCache)
            api.m_lastState.Clear();

        // Don't clear unless indicated to prevent sub pages from clearing cache when they use backstate
        if (useCached)
            api.m_cachedLatestState.Clear();

        return lastState;
    }

    // Current workaround for backstate when main page has multiple subpages
    public static void CacheLatestBackState()
    {
        // If already has a value it means that user is coming from a sub page scene so just ignore
        if (api.m_cachedLatestState.Count <= 0)
        {
            foreach (string scene in api.m_lastState)
            {
                api.m_cachedLatestState.Add(scene);
            }
        }
    }

    /// <summary>
    /// Check if any scene loaded except LoadFirst scene
    /// </summary>
    public static bool AnySceneLoaded()
    {
        return SceneManager.sceneCount > 1;
    }

    public static bool CheckLoaded(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }

    /// <summary>
    /// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
    /// </summary>
    public static bool IsSceneExist(string name, bool isAssetBundle)
    {
        if(isAssetBundle)
            return true;

        if (string.IsNullOrEmpty(name))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(name, sceneName, true) == 0)
                return true;
        }

        return false;
    }

    private void Init()
    {
        m_queueLoad = new Queue<SceneInfo>();
        m_queueUnload = new Queue<string>();
        m_load = false;
        m_unload = false;
        m_lastState = new List<string>();
        m_listActivedSceneInQueue = new List<string>();
        m_cachedLatestState = new List<string>();
    }

    //coroutine load scene
    private IEnumerator Load(bool isBackState)
    {
        //ToDo: ShowLoadingWithNoBackground
        float sceneWidth = Screen.width;

        m_load = true;
        while (m_load)
        {
            SceneInfo scene = m_queueLoad.Dequeue();
            scene.onBegin?.Invoke();

            while (scene.scenes.Count > 0)
            {
                string sceneName = scene.scenes.Dequeue();

                Scene sceneCheck = SceneManager.GetSceneByName(sceneName);
                if (!m_listActivedSceneInQueue.Contains(sceneName))
                    m_listActivedSceneInQueue.Add(sceneName);

                if (sceneCheck.isLoaded) yield return SceneManager.UnloadSceneAsync(sceneName);
                AsyncOperation sync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                yield return sync;

                Canvas sceneCanvas = GameObject.FindObjectsOfType<Canvas>().Where(x => x.gameObject.scene.name == sceneName).FirstOrDefault();
                if(sceneCanvas)
                {
                    GameObject container = sceneCanvas.transform.Find("Container")?.gameObject;
                    if(container)
                    {
                        DOTween.Complete(container.transform);
                        container.transform.DOScale(Vector3.one * CONST_TRANSITION_SCALE, CONST_TRANSITION_DURATION).From();
                        
                        if(isBackState)
                        {
                            container.transform.DOLocalMoveX(container.transform.localPosition.x - sceneWidth, CONST_TRANSITION_DURATION)
                                .SetEase(CONST_TRANSITION_EASE).From();
                        }
                        else
                        {
                            container.transform.DOLocalMoveX(container.transform.localPosition.x + sceneWidth, CONST_TRANSITION_DURATION)
                                .SetEase(CONST_TRANSITION_EASE).From();
                        }

                        CanvasGroup cvgContainer = container.GetComponent<CanvasGroup>();
                        if(cvgContainer == null) cvgContainer = container.AddComponent<CanvasGroup>();

                        DOTween.Complete(cvgContainer);
                        cvgContainer.DOFade(0f, CONST_TRANSITION_DURATION).SetEase(CONST_TRANSITION_EASE).From()
                            .OnStart(() => cvgContainer.blocksRaycasts = false)
                            .OnComplete(() => cvgContainer.blocksRaycasts = true);
                    }
                }
            }
        
            if (scene.delayFinish > 0)
            {
                yield return new WaitForSeconds(scene.delayFinish);
            }
            scene.onFinish?.Invoke();

            //
            if (m_queueLoad.Count < 1) //stop if queue empty
            {
                m_load = false;
            }
        }

        //ToDo: HideLoadingWithNoBackground
    }

    //coroutine unload
    private IEnumerator Unload(bool isBackState, Action callback)
    {
        float sceneWidth = Screen.width;

        m_unload = true;
        while (m_unload)
        {
            Scene scene = SceneManager.GetSceneByName(m_queueUnload.Dequeue());
            if (m_listActivedSceneInQueue.Contains(scene.name))
                m_listActivedSceneInQueue.Remove(scene.name);
            if (scene != null && scene.isLoaded) 
            {
                Canvas sceneCanvas = GameObject.FindObjectsOfType<Canvas>().Where(x => x.gameObject.scene.name == scene.name).FirstOrDefault();
                GameObject container = sceneCanvas?.transform.Find("Container")?.gameObject ?? null;
                //
                if(container)
                {
                    DOTween.Complete(container.transform);
                    container.transform.DOScale(Vector3.one * CONST_TRANSITION_SCALE, CONST_TRANSITION_DURATION);

                    if(isBackState)
                    {
                        container.transform.DOLocalMoveX(container.transform.localPosition.x + sceneWidth, CONST_TRANSITION_DURATION)
                            .SetEase(CONST_TRANSITION_EASE);
                    }
                    else
                    {
                        container.transform.DOLocalMoveX(container.transform.localPosition.x - sceneWidth, CONST_TRANSITION_DURATION)
                            .SetEase(CONST_TRANSITION_EASE);
                    }

                    CanvasGroup cvgContainer = container.GetComponent<CanvasGroup>();
                    if(cvgContainer == null) cvgContainer = container.AddComponent<CanvasGroup>();

                    DOTween.Complete(cvgContainer);
                    cvgContainer.DOFade(0f, CONST_TRANSITION_DURATION).SetEase(CONST_TRANSITION_EASE)
                        .OnStart(() => cvgContainer.blocksRaycasts = false)
                        .OnComplete(() => {
                            if (!m_listActivedSceneInQueue.Contains(scene.name))
                                SceneManager.UnloadSceneAsync(scene);
                        });
                }
                else
                {
                    if (!m_listActivedSceneInQueue.Contains(scene.name))
                        SceneManager.UnloadSceneAsync(scene);

                }

            }

            if (m_queueUnload.Count < 1) //stop if queue empty
            {
                m_unload = false;
            }
        }

        yield return new WaitForEndOfFrame();
        callback?.Invoke();
    }

    public static void SetActiveScene(string sceneName)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}
