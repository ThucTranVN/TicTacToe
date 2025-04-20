using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragUiView : MonoBehaviour
{
    public bool IsStickToWall; //stick to the wall
    public bool IsDragSelfPosition = false; //update self position when drag

    private Action<Vector3> m_StartDragAction; //notify start drag, param: vector3 position
    private Action<Vector3> m_OnDragAction; //notify on drag, param: vector3 position
    private Action<Vector3> m_EndDragAction; //notify end drag, param: vector3 position
    private RectTransform m_CanvasRect; //canvas rect
    private RectTransform m_Rect; //rect transform of self

    private void Awake()
    {
        EventTrigger evt = GetComponent<EventTrigger>(); //try get EventTrigger if exist
        if (evt == null)
        {
            evt = gameObject.AddComponent<EventTrigger>(); //add Envent Trigger if not exist
        }
        //add begin drag handler
        EventTrigger.Entry beginDrag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.BeginDrag
        };
        beginDrag.callback.AddListener(OnBeginDrag);
        evt.triggers.Add(beginDrag);
        //add drag handler
        EventTrigger.Entry drag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drag
        };
        drag.callback.AddListener(OnDrag);
        evt.triggers.Add(drag);
        //add end drag handler
        EventTrigger.Entry endDrag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.EndDrag
        };
        endDrag.callback.AddListener(OnEndDrag);
        evt.triggers.Add(endDrag);
        //find reference components
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }
        m_CanvasRect = canvas.transform as RectTransform;
        m_Rect = transform as RectTransform;
    }

    private void Start()
    {
        if (IsStickToWall)
        {
            ProcessEndPosition();
        }
    }

    private void OnEndDrag(BaseEventData data)
    {
        if (m_CanvasRect == null)
        {
            return;
        }

        m_EndDragAction?.Invoke(transform.localPosition); //notify end drag

        if (IsStickToWall)
        {
            ProcessEndPosition();
        }
    }

    private void OnDrag(BaseEventData data)
    {
        if (m_CanvasRect == null)
        {
            return;
        }

        PointerEventData evt = (PointerEventData)data;
        Vector3 globalMousePos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_CanvasRect, evt.position, evt.pressEventCamera, out globalMousePos))
        //convert pos from screen to canvas
        {
            m_OnDragAction?.Invoke(globalMousePos);
            if (IsDragSelfPosition) SetDraggedPosition(globalMousePos); //set position for self
        }
    }

    private void OnBeginDrag(BaseEventData data)
    {
        if (m_CanvasRect == null)
        {
            return;
        }

        PointerEventData evt = (PointerEventData)data;
        Vector3 globalMousePos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_CanvasRect, evt.position, evt.pressEventCamera, out globalMousePos))
        //convert pos from screen to canvas
        {
            m_StartDragAction?.Invoke(globalMousePos);
            if (IsDragSelfPosition) SetDraggedPosition(globalMousePos); //set position for self
        }
    }

    public void InitDragListener(Action<Vector3> startDragEvent, Action<Vector3> onDragEvent, Action<Vector3> endDragEvent)
    {
        //register event
        m_StartDragAction = startDragEvent;
        m_OnDragAction = onDragEvent;
        m_EndDragAction = endDragEvent;
    }

    //convert position from screen to canvas
    //set position for self
    private void SetDraggedPosition(Vector3 globalMousePos)
    {
        m_Rect.position = globalMousePos;
        m_Rect.rotation = m_CanvasRect.rotation;
    }

    //calculate end position if StickToWall enable
    private void ProcessEndPosition()
    {
        Vector3 pos = transform.localPosition; //end position
        float borderW = m_CanvasRect.sizeDelta.x / 2f; //half width canvas
        float borderH = m_CanvasRect.sizeDelta.y / 2f; //half height canvas
        float sizeX = m_Rect.sizeDelta.x / 2f; //half width self
        float sizeY = m_Rect.sizeDelta.y / 2f; //half height self
        float delX = borderW - Mathf.Abs(pos.x); //distance x to border canvas
        float delY = borderH - Mathf.Abs(pos.y); //distance y to boder canvas
        float x, y; //end position after calculate
        if (delX < delY) //keep y and change x
        {
            x = pos.x < 0 ? -borderW + sizeX : borderW - sizeX;
            y = pos.y < 0 ? Mathf.Max(pos.y, -borderH + sizeY) : Mathf.Min(pos.y, borderH - sizeY);
        }
        else //keep x and change y
        {
            x = pos.x < 0 ? Mathf.Max(pos.x, -borderW + sizeX) : Mathf.Min(pos.x, borderW - sizeX);
            y = pos.y < 0 ? -borderH + sizeY : borderH - sizeY;
        }
        transform.localPosition = new Vector3(x, y, 0);
    }
}

