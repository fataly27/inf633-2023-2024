using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeBrush : TerrainBrush {

    public float magnitude = 0.5f;

    public override void draw(int x, int z)
    {
        int extent = 3 * radius;
        for (int zi = -extent; zi <= extent; zi++)
        {
            for (int xi = -extent; xi <= extent; xi++)
            {
                if (xi * xi + zi * zi <= extent * extent)
                {
                    float squaredDist = xi * xi + zi * zi;
                    float weight = Mathf.Exp(-squaredDist / (float)(2 * radius * radius));
                    float weight2 = 2.0f * Mathf.Exp(-squaredDist / (float)(2 * radius * radius) * 4.0f);
                    terrain.set(x + xi, z + zi, terrain.get(x + xi, z + zi) + (weight - weight2) * magnitude);
                }
            }
        }
    }
}
