using System.Threading;
using Unity.VectorGraphics;
using UnityEngine;

public class VideoController : MonoBehaviour
{
    [SerializeField]
    public GameObject rainfall, createShape1, spin, heartPulse, shape2, shape3, shape4;
    float timer = 0f;
    void Start()
    {
        createShape1.SetActive(false);
        spin.SetActive(false);
        heartPulse.SetActive(false);
        shape2.SetActive(false);
        shape3.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);
        if (timer > 14.5f && timer < 25f)
        {
            spin.SetActive(true);
        } 
        else if(timer > 25f && timer < 36f)
        {
            heartPulse.SetActive(true);
            Disable(spin);
        }
        if (timer > 36f && timer < 48f)
        {
            Disable(heartPulse);
            Disable(rainfall);
            createShape1.SetActive(true);

        }
        else if (timer > 48f && timer < 58f)
        {
            createShape1.GetComponent<ShapeToShape4>().timeMod = 0.5f;
            spin.SetActive(true);
        } else if (timer > 58f && timer < 70f)
        {
            Disable(createShape1);
            shape3.SetActive(true);
        }
        else if (timer > 70f && timer < 92f)
        {
            Disable(shape3);
            shape2.SetActive(true);
            Enable(shape2);
            spin.SetActive(true);
            Enable(spin);
        }
        else if (timer > 92f && timer < 104f)
        {
            heartPulse.SetActive(true);
            Enable(heartPulse);
            Disable(spin);
            shape2.GetComponentInChildren<ShapeToShape4>().timeMod = 1;
        } else if (timer > 95f && timer < 99f)
        {
            Disable(shape2);
            
        } else if (timer > 124f)
        {
            shape2.transform.Translate(new Vector3(0, 0, 50));
            rainfall.SetActive(true);
        }
    }

    void Disable(GameObject obj)
    {
        foreach (Transform child in obj.transform)
            {
                child.gameObject.SetActive(false);
            }
    }

    void Enable(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

}
