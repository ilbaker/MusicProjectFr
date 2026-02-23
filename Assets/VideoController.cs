using Unity.VectorGraphics;
using UnityEngine;

public class VideoController : MonoBehaviour
{
    [SerializeField]
    public GameObject rainfall, createShape;
    float timer = 0f;
    void Start()
    {
        createShape.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);
        if(timer > 32.5f) {
            Disable(rainfall);
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
