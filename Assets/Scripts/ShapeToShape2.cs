// UMD IMDM290 
// Instructor: Myungin Lee
// All the same Lerp but using audio

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShapeToShape2 : MonoBehaviour
{
    GameObject[] spheres;
    public Transform parent;
    public int numSphere = 500;
    float time = 0f;
    Vector3[] initPos;
    Vector3[] startPosition, endPosition;
    float lerpFraction; // Lerp point between 0~1
    float t;

    public Texture2D shape1;
    public Texture2D shape2;
    public float scale = 0.5f;
    public int z1, z2;

    // Start is called before the first frame update
    void Start()
    {
        // Assign proper types and sizes to the variables.

        spheres = new GameObject[numSphere];
        initPos = new Vector3[numSphere]; // Start positions
        startPosition = new Vector3[numSphere];
        endPosition = new Vector3[numSphere];

        // Define target positions. Start = shape1, End = shape2
        SetPositions(shape1, startPosition, z1);
        SetPositions(shape2, endPosition, z2);


        // Let there be spheres..
        for (int i = 0; i < numSphere; i++)
        {
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spheres[i].transform.SetParent(parent);

            // Position
            initPos[i] = startPosition[i];
            spheres[i].transform.position = initPos[i];

            spheres[i].transform.localRotation = Quaternion.EulerAngles(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
            spheres[i].transform.localScale = new Vector3(Random.Range(0.3f, 0.5f), Random.Range(0.3f, 0.5f), Random.Range(0.3f, 0.5f));
            // Color
            // Get the renderer of the spheres and assign colors.
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            // HSV color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numSphere; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ***Here, we use audio Amplitude, where else do you want to use?
        // Measure Time 
        // Time.deltaTime = The interval in seconds from the last frame to the current one
        // but what if time flows according to the music's amplitude?
        //time += Time.deltaTime * AudioSpectrum.audioAmp;
        time += Time.deltaTime * AudioSpectrum.audioAmp;
        //Debug.Log(AudioSpectrum.audioAmp);
        // what to update over time?
        for (int i = 0; i < numSphere; i++)
        {
            // Lerp : Linearly interpolates between two points.
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Vector3.Lerp.html
            // Vector3.Lerp(startPosition, endPosition, lerpFraction)

            // lerpFraction variable defines the point between startPosition and endPosition (0~1)
            lerpFraction = Mathf.Sin(AudioSpectrum.audioAmp) * 0.5f + 0.5f;

            // Lerp logic. Update position       
            t = i * 2 * Mathf.PI / numSphere;
            spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction);
            float scale = AudioSpectrum.audioAmp;
            spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
            spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);

            // Color Update over time
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            float hue = (float)(Math.Sqrt(AudioSpectrum.audioAmp % 1)); // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(Mathf.Abs(hue * Mathf.Cos(time)), Mathf.Cos(AudioSpectrum.audioAmp / 10f), 2f + Mathf.Cos(time)); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }
    }

    int NumSolidPixels(Texture2D shape)
    {
        Color[] pixels = shape.GetPixels();
        int count = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a != 0)
            {
                if (IsEdgePixel(shape, pixels, i))
                {
                    count++;
                }
            }
        }
        return count;
    }

    // Sets the positions of the given array to the shape outline
    void SetPositions(Texture2D shape, Vector3[] positions, float z)
    {
        Color[] pixels = shape.GetPixels();
        Vector3[] solidPositions = new Vector3[NumSolidPixels(shape)];

        int width = shape.width;
        int pixelCount = 0;

        //Make array of every relevant pixel's position
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a != 0)
            {
                if (IsEdgePixel(shape, pixels, i) /*&& ((i % interval) == 0)*/ && pixelCount < solidPositions.Length)
                {

                    float x = (i % width) - (width / 2);
                    float y = (i / width) - (shape.height / 2);
                    solidPositions[pixelCount] = new Vector3(x * scale, y * scale, z);
                    //sphereColors[sphereCount] = pixels[i];
                    pixelCount++;
                }
            }
        }

        for (int i = 0; i < numSphere; i++)
        {
            // formula for even gaps: (TOTAL * n) / intervals
            int interval = (solidPositions.Length * i) / numSphere;
            Debug.Log(interval);
            //move to index after the gap
            if (interval >= solidPositions.Length)
            {
                interval = solidPositions.Length - 1;
            }

            // add sphere position
            positions[i] = solidPositions[interval];
        }
    }

    Boolean IsEdgePixel(Texture2D shape, Color[] pixels, int i)
    {
        // checks if pixel is solid & touching a transparent pixel
        if ((i > 0 && pixels[i - 1].a == 0) || (i < pixels.Length - 1 && pixels[i + 1].a == 0) ||
                        (i / shape.width > 0 && pixels[i - shape.width].a == 0) ||
                        (i / shape.width < shape.height - 1 && pixels[i + shape.width].a == 0))
        {
            return true;
        }
        return false;
    }
}
