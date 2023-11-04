using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Predator : Animal
{
    protected Prey[,] preysMap = null;
    protected override void Start()
    {
        base.Start();
    }
    public override void Setup(CustomTerrain ct, GeneticAlgo ga)
    {
        base.Setup(ct, ga);
        preysMap = ga.preysMap;
    }
    protected override bool TryEating()
    {
        // Retrieve animal location in the heighmap
        int dx = (int)((tfm.position.x / terrainSize.x) * genetic_algo.preysMapLength);
        int dy = (int)((tfm.position.z / terrainSize.y) * genetic_algo.preysMapLength);

        // If the prey is located in the dimensions of the terrain and over a grass position (details[dy, dx] > 0), it eats it.
        bool success = (dx >= 0) && dx < (preysMap.GetLength(1)) && (dy >= 0) && (dy < preysMap.GetLength(0)) && preysMap[dy, dx] != null;

        if (success)
        {
            genetic_algo.removeAnimal(preysMap[dy, dx]);
            preysMap[dy, dx] = null;
        }

        return success;
    }
    protected override void UpdateVision()
    {
        float startingAngle = -((float)nEyes / 2.0f) * stepAngle;
        float ratio = genetic_algo.preysMapLength / terrainSize.x;

        for (int i = 0; i < nEyes; i++)
        {
            Quaternion rotAnimal = tfm.rotation * Quaternion.Euler(0.0f, startingAngle + (stepAngle * i), 0.0f);
            Vector3 forwardAnimal = rotAnimal * Vector3.forward;

            float sx = tfm.position.x * ratio;
            float sy = tfm.position.z * ratio;
            vision[i] = 1.0f;
            bool targetDetected = false;

            // Interate over vision length.
            for (float distance = 1.0f; distance < maxVision; distance += 0.5f)
            {
                // Position where we are looking at.
                float px = (sx + (distance * forwardAnimal.x * ratio));
                float py = (sy + (distance * forwardAnimal.z * ratio));

                if (px < 0)
                    px += genetic_algo.preysMapLength;
                else if (px >= genetic_algo.preysMapLength)
                    px -= genetic_algo.preysMapLength;
                if (py < 0)
                    py += genetic_algo.preysMapLength;
                else if (py >= genetic_algo.preysMapLength)
                    py -= genetic_algo.preysMapLength;

                if ((int)px >= 0 && (int)px < preysMap.GetLength(1) && (int)py >= 0 && (int)py < preysMap.GetLength(0) && preysMap[(int)py, (int)px] != null)
                {
                    vision[i] = distance / maxVision;
                    targetDetected = true;
                    break;
                }
            }

            
            if (targetDetected)
                Debug.DrawLine(tfm.position, tfm.position + maxVision * forwardAnimal, Color.red);
            else
                Debug.DrawLine(tfm.position, tfm.position + maxVision * forwardAnimal, Color.gray);
            
        }
    }
}
