using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DominoInfluenceController : MonoBehaviour
{
    private List<RaycastResult> _results = new List<RaycastResult>();

    private void Update()
    {
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Interract();
    //    }

    //    if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
    //    {
    //        Interract();
    //    }
    }

    void Rotate(int step)
    {

    }


    void MoveOnX(int step)
    {

    }

    //void Interract()
    //{
    //    // raycast

    //    _results.Clear();

    //    PointerEventData eventData = new PointerEventData(EventSystem.current);
    //    eventData.position = Input.mousePosition;
    //    EventSystem.current.RaycastAll(eventData, _results);

    //    foreach (var result in _results)
    //    {
    //        if (result.gameObject.TryGetComponent(out Draggable draggable))
    //        {
                
    //            return;
    //        }
    //    }
    //}
}
