using UnityEngine;
using System.Collections.Generic;

namespace RTS
{
    public static class WorkManager
    {

        public static Rect CalculateSelectionBox(Bounds selectionBounds, Rect playingArea)
        {
            float centerX = selectionBounds.center.x;
            float centerY = selectionBounds.center.y;
            float centerZ = selectionBounds.center.z;

            float extentX = selectionBounds.extents.x;
            float extentY = selectionBounds.extents.y;
            float extentZ = selectionBounds.extents.z;

            List<Vector3> corners = new List<Vector3>();
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX + extentX, centerY + extentY, centerZ + extentZ)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX + extentX, centerY + extentY, centerZ - extentZ)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX + extentX, centerY - extentY, centerZ + extentZ)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX - extentX, centerY + extentY, centerZ + extentZ)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX + extentX, centerY - extentY, centerZ - extentZ)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX - extentX, centerY - extentY, centerZ + extentZ)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX - extentX, centerY + extentY, centerZ - extentZ)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(centerX - extentX, centerY - extentY, centerZ - extentZ)));

            
            Bounds screenBounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Count; i++)
            {
                screenBounds.Encapsulate(corners[i]);
            }

            
            float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
            float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
            float selectBoxWidth = 2 * screenBounds.extents.x;
            float selectBoxHeight = 2 * screenBounds.extents.y;

            return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);

        }

        public static GameObject GetHitObject(Vector3 origin)
        {
            Ray ray = Camera.main.ScreenPointToRay(origin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
            return null;
        }

        public static Vector3 GetHitPoint(Vector3 origin)
        {
            Ray ray = Camera.main.ScreenPointToRay(origin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) return hit.point;
            return ResourceManager.InvalidPosition;
        }
    }
}
