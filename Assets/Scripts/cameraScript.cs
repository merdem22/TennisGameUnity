using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{

    private Vector3 originalPosition;
    [SerializeField] private float joltDuration;
    [SerializeField] float maxJoltIntensity;
   
    void Awake()
    {
        references.mainCamera = GetComponent<Camera>();
        originalPosition = transform.position;
    }

    public void Jolt(float intensity)
    {
        
        StartCoroutine(CameraJolt(intensity * maxJoltIntensity, joltDuration));
    }


    private IEnumerator CameraJolt(float intensity, float duration)
    {
        Vector3 joltOffset = -transform.forward * intensity;
        Vector3 targetPosition = originalPosition + joltOffset;

        float elapsedTime = 0f;

        // Move the camera backwards over time
        while (elapsedTime < joltDuration/3)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / joltDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move it back to its original position
        elapsedTime = 0f;
        while (elapsedTime < joltDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / joltDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition; // Ensure the camera is exactly at the original position
    }
    private IEnumerator ShakeCamera(float intenstiy, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float durationModifier = 1 - elapsed / duration;
            float x = Random.Range(-intenstiy, intenstiy) * duration;
            float y = Random.Range(-intenstiy, intenstiy) * durationModifier;
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }


}
