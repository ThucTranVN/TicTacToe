using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : BaseManager<PlatformManager>
{
    public GamePlatform CurrentPlatform = GamePlatform.Unknown;

    protected override void Awake()
    {
        base.Awake();
        SetPlatform();
    }

    public void SetPlatform()
    {
#if UNITY_STANDALONE_WIN

        this.CurrentPlatform = GamePlatform.Windown;

#elif UNITY_STANDALONE_OSX

        this.CurrentPlatform = GamePlatform.MacOS;

#elif UNITY_ANDROID

        this.CurrentPlatform = GamePlatform.Android;

#elif UNITY_IOS

        this.CurrentPlatform = GamePlatform.iOS;        

#endif
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.CHANGED_PLATFORM);
        }

        print($"CurrentPlatform: {CurrentPlatform}");
    }

    public bool IsMobilePlatform()
    {
        if (this.CurrentPlatform == GamePlatform.Android
            || this.CurrentPlatform == GamePlatform.iOS)
            return true;
        return false;
    }

    public bool IsPCPlatform()
    {
        if (this.CurrentPlatform == GamePlatform.MacOS
            || this.CurrentPlatform == GamePlatform.Windown)
            return true;
        return false;
    }

    public void Clear()
    {
        ListenerManager.Instance.Clear();
    }
}
