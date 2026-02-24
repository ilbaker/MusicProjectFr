using System.Threading;
using Unity.VectorGraphics;
using UnityEngine;

public class VideoController : MonoBehaviour
{
    [SerializeField]
    public GameObject rainfall, createShape, spin, heartPulse;
    float timer = 0f;
    void Start()
    {
        createShape.SetActive(false);
        spin.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);
        if (timer > 11f && timer < 20f)
        {
            spin.SetActive(true);
        } 
        else if(timer > 22f && timer < 32.5)
        {
            heartPulse.SetActive(true);
        }
        if (timer > 32.5f)
        {
            Disable(rainfall);
            Disable(spin);
            Disable(heartPulse);
            createShape.SetActive(true);
        }
        else if (timer > 32.5f && timer < 60f)
        {

        }
    }

    void Disable(GameObject obj)
    {
        foreach (Transform child in obj.transform)
            {
                child.gameObject.SetActive(false);
            }
    }
}
