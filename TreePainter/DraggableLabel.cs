using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DraggableLabel : Label
{
    public static string s_DragDataType = "DraggableLabel";

    private bool m_GotMouseDown;
    private Vector2 m_MouseOffset;

    public DraggableLabel()
    {
        //#define USE_MOUSE_EVENTS
#if USE_MOUSE_EVENTS
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
            RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
#else
        RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
        RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
        RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
#endif
    }

    void OnMouseDownEvent(MouseDownEvent e)
    {
        if (e.target == this && e.button == 0)
        {
            m_GotMouseDown = true;
            m_MouseOffset = e.localMousePosition;
        }
    }

    void OnPointerDownEvent(PointerDownEvent e)
    {
        if (e.target == this && e.isPrimary && e.button == 0)
        {
            m_GotMouseDown = true;
            m_MouseOffset = e.localPosition;
        }
    }

    void OnMouseMoveEvent(MouseMoveEvent e)
    {
        if (m_GotMouseDown && e.pressedButtons == 1)
        {
            StartDraggingBox();
            m_GotMouseDown = false;
        }
    }

    void OnPointerMoveEvent(PointerMoveEvent e)
    {
        if (m_GotMouseDown && e.isPrimary && e.pressedButtons == 1)
        {
            StartDraggingBox();
            m_GotMouseDown = false;
        }
    }

    void OnMouseUpEvent(MouseUpEvent e)
    {
        if (m_GotMouseDown && e.button == 0)
        {
            m_GotMouseDown = false;
        }
    }

    void OnPointerUpEvent(PointerUpEvent e)
    {
        if (m_GotMouseDown && e.isPrimary && e.button == 0)
        {
            m_GotMouseDown = false;
        }
    }

    public void StartDraggingBox()
    {
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.SetGenericData(s_DragDataType, this);
        DragAndDrop.StartDrag(text);
    }

    public void StopDraggingBox(Vector2 mousePosition)
    {
        style.top = -m_MouseOffset.y + mousePosition.y;
        style.left = -m_MouseOffset.x + mousePosition.x;
    }
}
