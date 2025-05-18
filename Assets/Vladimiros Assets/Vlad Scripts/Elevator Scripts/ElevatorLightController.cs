using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorLightController : MonoBehaviour
{
    public List<Light> elevatorLights; // Drag your 4 lights here
    public bool flickerWhenUnpowered = true;
    public float flickerInterval = 0.2f;

    private bool isPowered = false;
    private Coroutine flickerRoutine;

    void Start()
    {
        // Start flickering until fixed
        flickerRoutine = StartCoroutine(FlickerLights());
    }

    public void PowerOn()
    {
        isPowered = true;

        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        foreach (Light light in elevatorLights)
        {
            if (light != null)
                light.enabled = true;
        }
    }

    IEnumerator FlickerLights()
    {
        while (!isPowered && flickerWhenUnpowered)
        {
            foreach (Light light in elevatorLights)
            {
                if (light != null)
                    light.enabled = Random.value > 0.5f;
            }

            yield return new WaitForSeconds(flickerInterval);
        }
    }
}
