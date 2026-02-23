using UnityEngine;
using UnityEngine.Animations;

public class Exposition : MonoBehaviour
{
    public GameObject raindrop, reboundObj, impactObj, cloudObj, parent;
    Transform parentTransform;
    
    GameObject[] spheres, cloud, reboundL, reboundR, impact;
    Material[] cloudMat;
    [Header("Piano trigger (onsets)")]
public float pianoMinHz = 180f;
public float pianoMaxHz = 2200f;

public float hissMinHz = 6000f;
public float hissMaxHz = 14000f;

public float onsetThreshold = 0.00006f;
public float ratioThreshold = 1.6f;
public float pianoCooldown = 0.12f;

float prevPianoE = 0f;
bool wasOn = false;
float lastPianoTime = -999f;
    float cloudSmooth = 0f;

    static int numSphere = 100;
    static int cloudBalls = 50;

    float []fallTime, reboundTime;
    float fallDuration = 2f;
    float reboundDuration = 0.5f;
    bool trackerSpace = true;
    int fftSize;
    float hzPerBin;
    float nyquist;
    bool[] wasAbove, state, trackerReboundFall;
float lastTime = -999f;
float cooldown = 0.05f;
float minHz = 400, maxHz = 2500;
public float cloudMinHz = 0;
public float cloudMaxHz = 100;
float threshold = 0.0006f;
     public int displayBins = 192;
     int []mappedBins;

    Vector3[] startPos;

    void Start()
    {
        parentTransform = parent.transform;
        fftSize = AudioSpectrum.FFTSIZE;

        // Nyquist is the maximum analyzable frequency in digital audio.
        nyquist = AudioSettings.outputSampleRate * 0.5f;
        hzPerBin = nyquist / fftSize;

        // Keep settings in a practical range for the classroom demo.
        displayBins = Mathf.Clamp(displayBins, 16, fftSize);
        mappedBins = new int[displayBins];
        BuildLogMapping();

        wasAbove = new bool[AudioSpectrum.FFTSIZE / 2];

        float r = 12f;

        spheres = new GameObject[numSphere];
        cloud = new GameObject[cloudBalls];
        startPos = new Vector3[numSphere];
        reboundL = new GameObject[numSphere];
        reboundR = new GameObject[numSphere];
        impact = new GameObject[numSphere];
        state = new bool [numSphere];
        fallTime = new float [numSphere];
        reboundTime = new float[numSphere];
        trackerReboundFall = new bool [numSphere];
        cloudMat = new Material[cloudBalls];
        
        for (int i = 0; i < cloudBalls; i++)
        {
            cloud[i] = Instantiate(cloudObj);
            cloud[i].transform.SetParent(parentTransform);
            Renderer rend = cloud[i].GetComponent<Renderer>();
            Material mat = rend.material;
            mat.EnableKeyword("_EMISSION");
            cloudMat[i] =  mat;

            float diameter = r * Random.Range(0.5f, 1f);
            cloud[i].transform.localScale = Vector3.one * diameter;

            if (i == 0)
            {
                cloud[i].transform.position = new Vector3(-25f, r, r);
            }
            else
            {
                float prevDiameter = cloud[i - 1].transform.localScale.x;
                float prevRadius = prevDiameter * 0.5f;
                float currRadius = diameter * 0.5f;

                float overlapFactor = 0.7f;
                float step = (prevRadius + currRadius) * overlapFactor;

                float x = cloud[i - 1].transform.position.x + step;
                float y = r + Random.Range(-r * 0.15f, r * 0.15f);
                float z = r + Random.Range(-r * 0.10f, r * 0.10f);

                cloud[i].transform.position = new Vector3(x, y, z);
            }
        }

        // Create rain system
        for (int i = 0; i < numSphere; i++)
        {
            state[i] = false;
            trackerReboundFall[i] = false;
            spheres[i] = Instantiate(raindrop);
            spheres[i].transform.SetParent(parentTransform);
            spheres[i].transform.position = new Vector3(
                r * Random.Range(-1f, 1f),
                17f,
                r * Random.Range(-1f, 1f) + 12f
            );

            startPos[i] = spheres[i].transform.position;

            reboundL[i] = Instantiate(reboundObj);
            reboundL[i].transform.SetParent(parentTransform);
            reboundL[i].transform.position = new Vector3(
                spheres[i].transform.position.x,
                -3f,
                spheres[i].transform.position.z
            );
            reboundL[i].SetActive(false);

            reboundR[i] = Instantiate(reboundObj);
            reboundR[i].transform.SetParent(parentTransform);
            reboundR[i].transform.position = new Vector3(
                spheres[i].transform.position.x,
                -3f,
                spheres[i].transform.position.z
            );
            reboundR[i].SetActive(false);

            impact[i] = Instantiate(impactObj);
            impact[i].transform.SetParent(parentTransform);
            impact[i].transform.position = new Vector3(
                spheres[i].transform.position.x,
                -5f,
                spheres[i].transform.position.z
            );
            impact[i].SetActive(false);

            fallTime[i] = 0f;
            reboundTime[i] = 0f;
        }
    }

    void Update()
    {
float pianoE = BandEnergy(pianoMinHz, pianoMaxHz);
float hissE  = BandEnergy(hissMinHz, hissMaxHz);

// onset = piano energy rising (note attack)
float onset = pianoE - prevPianoE;
prevPianoE = pianoE;

// gate = piano must dominate rain hiss
float ratio = pianoE / (hissE + 1e-6f);

bool on = (onset > onsetThreshold) && (ratio > ratioThreshold);

if (!wasOn && on && Time.time - lastPianoTime > pianoCooldown)
{
    TriggerDrop();
    lastPianoTime = Time.time;
}

wasOn = on;
    for (int k = 0; k < numSphere; k++)
        {
            if(state[k])
            {
                if (!trackerReboundFall[k])
        {
            fallTime[k] += Time.deltaTime;

            float t = Mathf.Clamp01(fallTime[k] / fallDuration);
            DropletFall(spheres[k], spheres[k].transform.position, t);

            if (spheres[k].transform.position.y <= -3.99f)
            {
                trackerReboundFall[k] = true;
                reboundTime[k] = 0f;

                reboundR[k].SetActive(true);
                reboundL[k].SetActive(true);
                impact[k].SetActive(true);

                spheres[k].SetActive(false);

                Renderer rend = impact[k].GetComponent<Renderer>();
                Material mat = rend.material;

                mat.EnableKeyword("_EMISSION");

                Color randomColor = Color.HSVToRGB(Random.value, 1f, 1f);
                mat.SetColor("_EmissionColor", randomColor * 8f);
            }
        }
        else
        {
            reboundTime[k] += Time.deltaTime;

            float t = reboundTime[k] / reboundDuration;

            ReboundR(reboundR[k], startPos[k], t);
            ReboundL(reboundL[k], startPos[k], t);
            ImpactCircle(impact[k], t);

            if (t >= 1f || reboundR[k].transform.position.y <= -4f + 0.001f)
            {
                reboundTime[k] = 0f;
                trackerReboundFall[k] = false;
                trackerSpace = true;
                fallTime[k] = 0f;

                reboundR[k].SetActive(false);
                reboundL[k].SetActive(false);
                impact[k].SetActive(false);
                state[k] = false;

                spheres[k].SetActive(true);
spheres[k].transform.position = startPos[k];
            }
        }
            }
        }

float cloudEnergy = BandEnergy(cloudMinHz, cloudMaxHz);
cloudSmooth = Mathf.Lerp(cloudSmooth, cloudEnergy, Time.deltaTime * 8f);

float emission = cloudSmooth * 100f;
emission = Mathf.Clamp(emission, 0, 13f);

for (int i = 0; i < cloudBalls; i++)
{
    cloudMat[i].SetColor("_EmissionColor", Color.white * emission);
}
        for (int i = 0; i < cloudBalls; i++)
        {
            cloud[i].transform.position += Vector3.left * Time.deltaTime * cloudSmooth * 80f;
        }
    }

    void DropletFall(GameObject sphere, Vector3 start, float t)
    {
        Vector3 end = new Vector3(start.x, -4f, start.z);
        sphere.transform.position = Vector3.Lerp(start, end, t);
    }

    void ReboundR(GameObject sphere, Vector3 start, float t)
    {
        t = Mathf.Clamp01(t);

        float horizontalDistance = 3.5f;
        float height = 2.5f;

        float x = horizontalDistance * t;
        float y = height * Mathf.Sin(Mathf.PI * t);

        sphere.transform.position = new Vector3(start.x + x, -4f + y, start.z);
    }

    void ReboundL(GameObject sphere, Vector3 start, float t)
    {
        t = Mathf.Clamp01(t);

        float horizontalDistance = 3.5f;
        float height = 2.5f;

        float x = horizontalDistance * t;
        float y = height * Mathf.Sin(Mathf.PI * t);

        sphere.transform.position = new Vector3(start.x - x, -4f + y, start.z);
    }

    void ImpactCircle(GameObject circle, float t)
    {
        t = Mathf.Clamp01(t);
        circle.transform.localScale = new Vector3(t * 5f, 0f, t * 5f);
    }

    void BuildLogMapping()
    {
        float safeMinHz = Mathf.Clamp(minHz, hzPerBin, nyquist);

        for (int i = 0; i < displayBins; i++)
        {
            float t = NormalizedX(i);
            float hz = 0f;

            if (t > 0f)
            {
                // Convert [0,1] -> log-frequency range.
                hz = Log01ToHz(t, safeMinHz, nyquist);
            }

            SetMapping(i, hz);
        }
    }

    float NormalizedX(int index)
    {
        return index / (float)(displayBins - 1);
    }

    float Log01ToHz(float t, float minHz, float maxHz)
    {
        float minLog10 = Mathf.Log10(minHz);
        float maxLog10 = Mathf.Log10(maxHz);
        float logValue = Mathf.Lerp(minLog10, maxLog10, t);
        return Mathf.Pow(10f, logValue);
    }

    void SetMapping(int displayIndex, float hz)
    {
        int fftBin = Mathf.Clamp(Mathf.RoundToInt(hz / hzPerBin), 0, fftSize - 1);
        mappedBins[displayIndex] = fftBin;
    }

    void TriggerDrop()
    {
        int tmp = Random.Range(0, numSphere);
        while(state[tmp] == true)
        {
            tmp = Random.Range(0, numSphere);
        }
        state[tmp] = true;
    }

    float BandEnergy(float minHz, float maxHz)
{
    int half = AudioSpectrum.FFTSIZE / 2;

    int minBin = Mathf.Clamp(Mathf.FloorToInt(minHz / hzPerBin), 0, half - 1);
    int maxBin = Mathf.Clamp(Mathf.CeilToInt(maxHz / hzPerBin), minBin + 1, half);

    float sum = 0f;
    for (int i = minBin; i < maxBin; i++)
        sum += AudioSpectrum.samples[i];

    return sum / (maxBin - minBin);
}
}