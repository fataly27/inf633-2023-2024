using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothBrush : TerrainBrush {

    public float height = 5;

    public override void draw(int x, int z) {
        for (int zi = -radius; zi <= radius; zi++) {
            for (int xi = -radius; xi <= radius; xi++)
            {
                float average = terrain.get(x + xi - 1, z + zi) + terrain.get(x + xi + 1, z + zi) + terrain.get(x + xi, z + zi - 1) + terrain.get(x + xi, z + zi + 1);
                average /= 4.0f;
                terrain.set(x + xi, z + zi, average);
                
            }
        }
    }
}
