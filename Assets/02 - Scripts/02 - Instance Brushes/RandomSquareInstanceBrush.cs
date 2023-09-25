using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSquareInstanceBrush : InstanceBrush
{
    public override void draw(float x, float z)
    {
        float xOffset = Random.Range(-radius, radius);
        float zOffset = Random.Range(-radius, radius);

        spawnObject(x + xOffset, z + zOffset);
    }
}
