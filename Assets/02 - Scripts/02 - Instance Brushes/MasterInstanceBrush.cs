using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterInstanceBrush : InstanceBrush
{
    public GameObject PalmTree;
    public GameObject Broadleaf;
    public GameObject Conifer;

    private int palmTree_idx;
    private int broadleaf_idx;
    private int conifer_idx;

    public float superMinimalProjectedDistance = 100.0f;
    public float superMaximalSteepness = 50.0f;
    public float superUpperAltitude = 100.0f;
    public float superLowerAltitude = 30.0f;

    public override void callDraw(float x, float z)
    {
        if (PalmTree)
            palmTree_idx = terrain.registerPrefab(PalmTree);
        if (Broadleaf)
            broadleaf_idx = terrain.registerPrefab(Broadleaf);
        if (Conifer)
            conifer_idx = terrain.registerPrefab(Conifer);
        

        Vector3 grid = terrain.world2grid(x, z);
        draw(grid.x, grid.z);
    }

    public void spawnObject(float x, float z, int prefab_idx)
    {
        if (prefab_idx == -1)
            return;
        
        float scale_diff = Mathf.Abs(terrain.max_scale - terrain.min_scale);
        float scale_min = Mathf.Min(terrain.max_scale, terrain.min_scale);
        float scale = (float)CustomTerrain.rnd.NextDouble() * scale_diff + scale_min;
        terrain.spawnObject(terrain.getInterp3(x, z), scale, prefab_idx);
    }

    public override void draw(float x, float z)
    {
        for (int i = 0; i < terrain.getObjectCount(); i++)
        {
            float x2 = terrain.getObjectLoc(i).x;
            float z2 = terrain.getObjectLoc(i).z;
            if ( ((x - x2) * (x - x2) + (z - z2) * (z - z2)) < superMinimalProjectedDistance)
                return;
            if (terrain.getSteepness(x, z) > superMaximalSteepness)
                return;
        }
        if (terrain.getInterp(x, z) < superLowerAltitude)
            spawnObject(x, z, palmTree_idx);
        else if (terrain.getInterp(x, z) > superUpperAltitude)
            spawnObject(x, z, conifer_idx);
        else
            spawnObject(x, z, broadleaf_idx);
    }
}
