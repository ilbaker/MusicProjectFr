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
    public Texture2D shape1;
    public float scale = 0.5f;
    int numSphere;
    // Start is called before the first frame update
    void Start()
    {
        numSphere = NumSolidPixels(shape1);

        spheres = new GameObject[numSphere]; // how many spheres
        spheresInitialPositions = new Vector3[numSphere]; // initial positions of the spheres
        sphereColors = new Color[numSphere];

        SetPositions(shape1, spheresInitialPositions);

        // Let there be spheres..
        for (int i = 0; i < numSphere; i++)
        {
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        
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
    void SetPositions(Texture2D shape, Vector3[] positions)
    {
        Color[] pixels = shape.GetPixels();

        int width = shape.width;
        int sphereCount = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a != 0)
            {
                if (IsEdgePixel(shape, pixels, i) /*&& ((i % interval) == 0)*/ && sphereCount < numSphere)
                {

                    float x = (i % width) - (width / 2);
                    float y = (i / width) - (shape.height / 2);
                    positions[sphereCount] = new Vector3(x * scale, y * scale, 100f);
                    //sphereColors[sphereCount] = pixels[i];
                    sphereCount++;
                }
            }
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
