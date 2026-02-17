// UMD IMDM290 
// Instructor: Myungin Lee
// This tutorial introduce a way to draw spheres and align them in a circle with colors.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateShape : MonoBehaviour
{
    GameObject[] spheres;
    Vector3[] spheresInitialPositions;
    Color[] sphereColors;
    public Texture2D shape;
    float scale = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        int numSphere = NumSolidPixels();
        spheres = new GameObject[numSphere]; // how many spheres
        spheresInitialPositions = new Vector3[numSphere]; // initial positions of the spheres
        sphereColors = new Color[numSphere];

        SetPositions();

        // Let there be spheres..
        for (int i = 0; i < numSphere; i++)
        {
            float r = 10f; // radius of the circle
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // Initial positions of the spheres. make it in circle with r radius.
            // https://www.cuemath.com/geometry/unit-circle/
            //spheresInitialPositions[i] = new Vector3(r * Mathf.Sin(i * 2 * Mathf.PI / numSphere), r * Mathf.Cos(i * 2 * Mathf.PI / numSphere), 10f);

            // Initial positions on pixel grid
            spheres[i].transform.position = spheresInitialPositions[i];

            // Get the renderer of the spheres and assign colors.
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            // hsv color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numSphere; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            sphereRenderer.material.color = color;
            //sphereRenderer.material.color = sphereColors[i];
        }
    }

    int NumSolidPixels()
    {
        Color[] pixels = shape.GetPixels();
        int count = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a != 0)
            {
                if (IsEdgePixel(pixels, i))
                {
                    count++;
                }
            }
        }
        return count;
    }

    void SetPositions()
    {
        Color[] pixels = shape.GetPixels();

        int width = shape.width;
        int sphereCount = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a != 0)
            {
                if (IsEdgePixel(pixels, i))
                {

                    float x = (i % width) - (width / 2);
                    float y = (i / width) - (shape.height / 2);
                    spheresInitialPositions[sphereCount] = new Vector3(x * scale, y * scale, 100f);
                    sphereColors[sphereCount] = pixels[i];
                    sphereCount++;
                }
            }
        }
    }

    Boolean IsEdgePixel(Color[] pixels, int i)
    {
        if ((i > 0 && pixels[i - 1].a == 0) || (i < shape.width - 1 && pixels[i + 1].a == 0) ||
                        (i / shape.width > 0 && pixels[i - shape.width].a == 0) ||
                        (i / shape.width < shape.height - 1 && pixels[i + shape.width].a == 0))
        {
            return true;
        }
        return false;
    }
}
