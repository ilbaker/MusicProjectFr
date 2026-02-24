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
    public Transform parent;
    Vector3[] spheresInitialPositions;
    Color[] sphereColors;
    public Texture2D shape1;
    public float scale = 0.5f;
    int numSphere = 300;
    // Start is called before the first frame update
    void Start()
    {
        //numSphere = NumSolidPixels(shape1);

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
            spheres[i].transform.SetParent(parent);

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
        Vector3[] solidPositions = new Vector3[NumSolidPixels(shape)];

        int width = shape.width;
        int pixelCount = 0;

        //Make array of every relevant pixel's position
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a != 0)
            {
                if (IsEdgePixel(shape, pixels, i) && pixelCount < solidPositions.Length)
                {

                    float x = (i % width) - (width / 2);
                    float y = (i / width) - (shape.height / 2);
                    solidPositions[pixelCount] = new Vector3(x * scale, y * scale, 100);
                    //sphereColors[sphereCount] = pixels[i];
                    pixelCount++;
                }
            }
        }

        for (int i = 0; i < numSphere; i++)
        {
            // formula for even gaps: (TOTAL * n) / intervals
            int interval = (solidPositions.Length * i) / numSphere;
            //Debug.Log(solidPositions.Length + " pixels, " + i + " spheres. interval = " + interval); 
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
