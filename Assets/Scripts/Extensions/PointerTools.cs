using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extensions
{
    public static class PointerTools
    {
        public static bool IsPointerOverUI() => IsPointerOverUI(Input.mousePosition);

        public static bool IsPointerOverUI(Vector2 screenPosition)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        public static bool IsPointerOverUI(int touchID)
        {
            if (Input.touchCount <= touchID)
                return false;

            return IsPointerOverUI(Input.GetTouch(touchID).position);
        }
    }
}