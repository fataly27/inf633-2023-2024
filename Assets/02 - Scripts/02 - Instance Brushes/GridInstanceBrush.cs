using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInstanceBrush : InstanceBrush
{
    [Range(-180.0f, 180.0f)]
    public float angle = 0.0f;
    public int nbRows = 4;
    public int nbCols = 4;

    public override void draw(float x, float z)
    {
        float radian_angle = Mathf.PI * angle / 180.0f;

        for (int i = 0; i < nbRows; i++)
        {
            for (int j = 0; j < nbCols; j++)
            {
                float xOffset = radius * (float)(2 * i + 1 - nbRows);
                float zOffset = radius * (float)(2 * j + 1 - nbCols);

                float rotatedXOffset = xOffset * Mathf.Cos(radian_angle) + zOffset * Mathf.Sin(radian_angle);
                float rotatedZOffset = zOffset * Mathf.Cos(radian_angle) - xOffset * Mathf.Sin(radian_angle);

                spawnObject(x + rotatedXOffset, z + rotatedZOffset);
            }
        }
    }
}
