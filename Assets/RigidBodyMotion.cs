using UnityEngine;

public class RigidBodyMotion : MonoBehaviour
{
    public GameObject sphere;
    public Transform parent;
    GameObject[] spheres;

    Rigidbody[] rb;
    int num = 400;
    float colorTimer = 0f, bassSmooth = 0f;
    void Start()
    {
        spheres = new GameObject[num];
        rb = new Rigidbody [num];

        for(int i = 0; i < num; i++)
        {
            spheres[i] = Instantiate(sphere);
            spheres[i].transform.SetParent(parent);
            spheres[i].transform.position = new Vector3(Random.Range(-160f,160f), Random.Range(-85f, 85f), 137f);
            spheres[i].transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f);

            rb[i] = spheres[i].AddComponent<Rigidbody>();

            rb[i].mass = 5f;
            rb[i].linearDamping = 1f;
            rb[i].AddForce(Quaternion.Euler(0, 0, 45) * Vector3.up);
        }
    }

    // Update is called once per frame
    void Update()
    {
        colorTimer += Time.deltaTime;
        float bass = 0f;

        for (int i = 0; i < 10; i++)
        {
            bass += AudioSpectrum.samples[i];
        }

        bass *= 80f;
        bassSmooth = Mathf.Lerp(bassSmooth, bass, Time.deltaTime * 10f);
        bass = Mathf.Clamp(bass, 0, 80f);

        for(int i = 0; i < num; i++)
        {
            Vector3 dir = Random.insideUnitCircle.normalized;
            rb[i].AddForce(new Vector3(dir.x, dir.y, 0f) * bass, ForceMode.Impulse);
        }
    }
}
