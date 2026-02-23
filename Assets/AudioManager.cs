using UnityEngine;
using UnityEngine.SceneManagement; // Useful if you want to change music based on the scene name

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
        audioSource = GetComponent<AudioSource>();
    }
}

