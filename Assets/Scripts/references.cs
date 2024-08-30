using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class references
{
    public static Camera mainCamera; //is set by cameraScript.
    
    public static Vector3 GetCursorLocationOnGround()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }
        return Vector3.zero; //raycast doesn't hit.
    }
}
