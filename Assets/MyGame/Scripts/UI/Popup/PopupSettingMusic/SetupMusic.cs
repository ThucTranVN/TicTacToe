using System;
using UnityEngine;
using UnityEngine.UI;

public class SetupMusic : MonoBehaviour
{
    [Header("Attribute")]
    [SerializeField] private AudioClip currentMusicSound;
    public Action<SetupMusic> OnSelected;

    [Header("Button")]
    [SerializeField] private Button replayMusic;
    [Header("Toggle")]
    [SerializeField] private Toggle playMusic;
    [SerializeField] private Image BgImg;
    [SerializeField] private Image checkMarkImg;

    private void Start()
    {
        playMusic.onValueChanged.AddListener(OnClickValueChangePlayMusic);
        replayMusic.onClick.AddListener(OnClickButtonReplayMusic);
    }

    public void OnClickButtonReplayMusic()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.AttachBGMSource.Stop();
            AudioManager.Instance.PlayBGM(currentMusicSound.name);
        }
    }

    public void SetCurrentMusic(AudioClip nameMusic)
    {
        currentMusicSound = nameMusic;
    }
    public AudioClip GetCurrentMusic()
    {
        return currentMusicSound;
    }

    public void OnClickValueChangePlayMusic(bool IsOn)
    {
        if (IsOn)
        {
            SetColorAlpha(true);
            PlayMusic();
            ShowReplayMusic();
            OnSelected?.Invoke(this);
        }
        else
        {
            SetColorAlpha(false);
            PauseMusic();
            HideReplayMusic();
        }
    }
    public void ShowReplayMusic()
    {
        replayMusic.interactable = true;
    }
    public void HideReplayMusic()
    {
        replayMusic.interactable = false;
    }
    public void PlayMusic()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlayBGM(currentMusicSound.name);
        }
    }
    public void PauseMusic()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.AttachBGMSource.Pause();
        }
    }
    public bool GetStateToggleMusic()
    {
        return playMusic.isOn;
    }
    public void SetStateToggleMusic(bool isState)
    {
        playMusic.isOn = isState;
    }
    public Toggle GetToggleMusic()
    {
        return playMusic;
    }
    private void SetColorAlpha(bool IsAlha)
    {
        if (IsAlha)
        {
            BgImg.color = new Color(1, 1, 1, 0f);
            checkMarkImg.color = new Color(1, 1, 1, 1f);
        }
        else
        {
            BgImg.color = new Color(1, 1, 1, 1f);
            checkMarkImg.color = new Color(1, 1, 1, 0f);
        }
    }
}
