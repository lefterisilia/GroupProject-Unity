using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] GameObject FlashLightLight;
    private bool FlashlightActive = false;
    void Start()
    {
        FlashLightLight.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (FlashlightActive == false)
            {
                FlashLightLight.gameObject.SetActive(true);
                FlashlightActive = true;
            }
            else
            {
                FlashLightLight.gameObject.SetActive(false);
                FlashlightActive = false;
            }
        }
        
    }
}
