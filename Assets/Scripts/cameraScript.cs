using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{
    void Awake()
    {
        references.mainCamera = GetComponent<Camera>();
    }
}
