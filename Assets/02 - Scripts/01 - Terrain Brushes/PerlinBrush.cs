using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinBrush : TerrainBrush {

    public float magnitude = 3;

    public override void draw(int x, int z)
    {
        for (int zi = -radius; zi <= radius; zi++)
        {
            for (int xi = -radius; xi <= radius; xi++)
            {
                terrain.set(x + xi, z + zi, terrain.get(x + xi, z + zi) + magnitude * Mathf.PerlinNoise((float)(x + xi) * 0.05f, (float)(z + zi) * 0.05f));
            }
        }
    }
}
