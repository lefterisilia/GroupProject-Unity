using UnityEngine;

public class RadioController : MonoBehaviour
{
    private AudioSource radioAudio;

    private void Start()
    {
        radioAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press E to toggle radio
        {
            if (radioAudio.isPlaying)
                radioAudio.Pause();
            else
                radioAudio.Play();
        }
    }
}
