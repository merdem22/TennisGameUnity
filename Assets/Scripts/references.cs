using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class references
{

    //commonly accesed stuff should be here for ease of use.

    public static Camera mainCamera;
    public static float maxBallSpeed;
    public static float courtWidth;
    public static float courtLength;
    
    public static Vector3 GetCursorLocationOnGround()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Court")))
        {
            return hit.point;
        }
        return Vector3.zero; //raycast doesn't hit.
    }
}
