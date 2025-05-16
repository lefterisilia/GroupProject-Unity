using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Slider sensitivitySlider;
    public Text sensitivityText;

    public static float sensitivity = 1.0f;

    void Start()
    {
        // Load saved sensitivity or use default
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        sensitivity = savedSensitivity;
        sensitivitySlider.value = savedSensitivity;

        // Update text
        UpdateSensitivityText(savedSensitivity);

        // Add listener
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void OnSensitivityChanged(float value)
    {
        sensitivity = value;
        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();
        UpdateSensitivityText(value);
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = "Sensitivity: " + value.ToString("F1");
        }
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * PlayerMovement.sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * PlayerMovement.sensitivity;

    }
}
