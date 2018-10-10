using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTest : MonoBehaviour
{
    [SerializeField]
    private GameObject _quad;

    private Texture2D _texture;

    private void Start()
    {
        int width = 512;
        int height = 512;
        float frequency = 32f;
        int octaves = 5;

        _texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        uint seed = 1000;
        PerlinNoise noise = new PerlinNoise(seed);

        Color[] pixels = new Color[width * height];
        float fx = (float)width / frequency;
		float fy = (float)height / frequency;
        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % width;
            int y = i / width;
            float n = noise.OctaveNoise(x / fx, y / fy, octaves);
            float c = Mathf.Clamp(218f * (0.5f + n * 0.5f), 0f, 255f) / 255f;
            pixels[i] = new Color(c, c, c, 1f);
        }

        _texture.SetPixels(0, 0, width, height, pixels);
        _texture.Apply();

        Renderer renderer = _quad.GetComponent<Renderer>();
        renderer.material.mainTexture = _texture;
    }
}