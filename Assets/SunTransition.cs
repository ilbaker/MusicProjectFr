using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class SunTransition : MonoBehaviour
{
    public GameObject sun;
     public Transform parentTransform;
    public Vector3 pos;
    Material mat;
    float timer = 0f, duration = 4f;
    bool start = false;
    void Start()
    {
        sun = Instantiate(sun);
        sun.transform.SetParent(parentTransform);
        sun.transform.position = new Vector3(10.59f, 10f, 16f); 
        sun.transform.localScale = new Vector3(7.6f, 7.6f, 7.6f);
        Renderer rend = sun.GetComponent<Renderer>();
        mat = rend.material;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.yellow * 1f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 35)
        {
            timer = 0f;
            start = true;
        }
        if (start == true)
        {
            float t = timer / duration;

            float currentScale = Mathf.Lerp(1f, 120f, t);
            float currentGlow = Mathf.Lerp(1f, 6000f, t);

            sun.transform.localScale = Vector3.one * currentScale;
            mat.SetColor("_EmissionColor", Color.yellow * currentGlow);
        }
    }
}
