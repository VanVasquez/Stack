using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 1f; // Duration of the shake effect
    public float shakeMagnitude = 0.2f; // Intensity of the shake effect

    private Vector3 originalPosition; // The original position of the object before shaking
    private float currentDuration; // The current duration of the shake effect
    private float currentMagnitude; // The current magnitude of the shake effect
    private bool isShaking; // Indicates if the shake effect is currently active


    public AudioSource src;
    public AudioClip sfx1;

    void Update()
    {
        if (isShaking)
        {
            if (currentDuration > 0)
            {
                // Generate a random offset based on the current magnitude
                Vector3 randomOffset = Random.insideUnitSphere * currentMagnitude;

                // Apply the offset to the object's position
                transform.localPosition = originalPosition + randomOffset;

                // Reduce the duration over time
                currentDuration -= Time.deltaTime;
            }
            else
            {
                // Reset the object's position to the original position when the shake effect is done
                transform.localPosition = originalPosition;
                isShaking = false;
            }
        }
    }
    private IEnumerator StopSoundAfterDelay(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
    }
    public void StartRandomShake(float minDuration, float maxDuration, float minMagnitude, float maxMagnitude)
    {
        if (!isShaking)
        {
            originalPosition = transform.localPosition;
             
            currentDuration = Random.Range(minDuration, maxDuration);
            currentMagnitude = Random.Range(minMagnitude, maxMagnitude);

            src.clip = sfx1;
            src.PlayOneShot(sfx1);
            StartCoroutine(StopSoundAfterDelay(src, currentDuration));

            isShaking = true;

        }
    }

}
