using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : BasePopup
{
    private float bgmVolume;
    private float effectVolume;
    public Slider sliderBGM;
    public Slider sliderEffect;

    public override void Init()
    {
        base.Init();
        if (AudioManager.HasInstance)
        {
            bgmVolume = AudioManager.Instance.AttachBGMSource.volume;
            effectVolume = AudioManager.Instance.AttachSESource.volume;
            sliderBGM.value = bgmVolume;
            sliderEffect.value = effectVolume;
        }
    }

    public override void Show(object data)
    {
        base.Show(data);
        if (AudioManager.HasInstance)
        {
            bgmVolume = AudioManager.Instance.AttachBGMSource.volume;
            effectVolume = AudioManager.Instance.AttachSESource.volume;
            sliderBGM.value = bgmVolume;
            sliderEffect.value = effectVolume;
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnClickCloseButton()
    {
        this.Hide();
    }

    public void OnBGMValueChange(float v)
    {
        bgmVolume = v;
    }

    public void OnEffectValueChange(float v)
    {
        effectVolume = v;
    }

    public void OnApplySetting()
    {
        if (AudioManager.HasInstance)
        {
            if(bgmVolume != AudioManager.Instance.AttachBGMSource.volume)
            {
                AudioManager.Instance.ChangeBGMVolume(bgmVolume);
            }

            if(effectVolume != AudioManager.Instance.AttachSESource.volume)
            {
                AudioManager.Instance.ChangeSEVolume(effectVolume);
            }
        }
        this.Hide();
    }
    
}
