using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIListenerElement : MonoBehaviour, IUIListenerElement
{
    [SerializeField, InterfaceType(typeof(IUIListenerElement))]
    private Object[] listeners;

    public Object[] Listeners { get => listeners; }

    public void Clear()
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            GetListener(i).Clear();
        }
    }

    public IUIListenerElement GetListener(int index) => listeners[index] as IUIListenerElement;

    public void Hide()
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            GetListener(i).Hide();
        }
    }

    public void Init()
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            GetListener(i).Init();
        }
    }

    public void Remove()
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            GetListener(i).Remove();
        }
    }

    public void Show()
    {
        for (int i = 0; i < listeners.Length; i++)
        {
            GetListener(i).Show();
        }
    }
}