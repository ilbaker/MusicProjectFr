using UnityEngine;

public class GlowForShape : MonoBehaviour
{
    public Transform parentTransform;
    public GameObject glowSphere;
    GameObject[] arr;
    Material[] gmat;
    float colorTimer = 0f, bassSmooth = 0f;
    void Start()
    {
        arr = new GameObject[3];
        gmat = new Material[3];
        for(int i = 0; i < 3; i++)
        {
            arr[i] = Instantiate(glowSphere);
            arr[i].transform.SetParent(parentTransform);
            Renderer rend = arr[i].GetComponent<Renderer>();
            Material mat = rend.material;
            mat.EnableKeyword("_EMISSION");
            gmat[i] =  mat;
        }

        arr[0].transform.position = new Vector3(-16f, -22.6f, 26.6f);
        arr[1].transform.position = new Vector3(-30f, 24.6f, 26.4f);
        arr[2].transform.position = new Vector3(28.9f, 8.64f, 11.62f);
    }

    void Update()
    {
        colorTimer += Time.deltaTime;
        float newColor;
        float bass = 0f;

        for (int i = 0; i < 10; i++)
        {
            bass += AudioSpectrum.samples[i];
        }

        bass *= 80f;
        bassSmooth = Mathf.Lerp(bassSmooth, bass, Time.deltaTime * 10f);
        bass = Mathf.Clamp(bass, 0, 80f);

        for(int i = 0; i < 3; i++)
        {
            newColor = Mathf.Clamp01(colorTimer / (60 * 2f));

            Color c = Color.HSVToRGB(newColor, 1f, 1f);
            gmat[i].SetColor("_EmissionColor", c * bass);
        }
    } 
}
