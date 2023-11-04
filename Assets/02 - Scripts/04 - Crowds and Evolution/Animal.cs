using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum AnimalTypes
{
    Prey,
    Predator
}

public abstract class Animal : MonoBehaviour
{

    [Header("Animal parameters")]
    public float swapRate = 0.01f;
    public float mutateRate = 0.01f;
    public float swapStrength = 10.0f;
    public float mutateStrength = 0.5f;
    public float maxAngle = 10.0f;

    [Header("Energy parameters")]
    public float maxEnergy = 10.0f;
    public float lossEnergy = 0.2f;
    public float gainEnergy = 10.0f;
    protected float energy;

    [Header("Sensor - Vision")]
    public float maxVision = 25.0f;
    public float stepAngle = 6.0f;
    public int nEyes = 8;

    protected int[] networkStruct;
    protected SimpleNeuralNet brain = null;

    // Terrain.
    protected CustomTerrain terrain = null;
    protected int[,] details = null;
    protected Vector2 detailSize;
    protected Vector2 terrainSize;

    // Animal.
    protected Transform tfm;
    protected float[] vision;
    public Color animalColour;

    // Genetic alg.
    protected GeneticAlgo genetic_algo = null;

    // Renderer.
    protected Material mat = null;

    protected virtual void Start()
    {
        // Network: 1 input per receptor, 1 output per actuator.
        vision = new float[nEyes];
        networkStruct = new int[] { nEyes, 5, 1 };
        energy = maxEnergy;
        tfm = transform;

        // Renderer used to update animal color.
        // It needs to be updated for more complex models.
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
            mat = renderer.material;
    }

    void Update()
    {
        // In case something is not initialized...
        if (brain == null)
            brain = new SimpleNeuralNet(networkStruct);
        if (terrain == null)
            return;
        if (details == null)
        {
            UpdateSetup();
            return;
        }

        // For each frame, we lose lossEnergy
        energy -= lossEnergy;

        // If the animal manages to eat, it gains energy and spawns an offspring.
        if (TryEating())
        {
            energy += gainEnergy;
            if (energy > maxEnergy)
                energy = maxEnergy;

            genetic_algo.addOffspring(this);
        }

        // If the energy is below 0, the animal dies.
        if (energy < 0)
        {
            energy = 0.0f;
            genetic_algo.removeAnimal(this);
        }

        // Update the color of the animal as a function of the energy that it contains.
        if (mat != null)
            mat.color = animalColour * (energy / maxEnergy);

        // 1. Update receptor.
        UpdateVision();

        // 2. Use brain.
        float[] output = brain.getOutput(vision);

        // 3. Act using actuators.
        float angle = (output[0] * 2.0f - 1.0f) * maxAngle;
        tfm.Rotate(0.0f, angle, 0.0f);
    }

    /// <summary>
    /// Calculate distance to the nearest food resource, if there is any.
    /// </summary>
    protected abstract void UpdateVision();

    /// <summary>
    /// Returns true if the animal can currently eat
    /// </summary>
    protected abstract bool TryEating();

    public virtual void Setup(CustomTerrain ct, GeneticAlgo ga)
    {
        terrain = ct;
        genetic_algo = ga;
        UpdateSetup();
    }

    protected void UpdateSetup()
    {
        detailSize = terrain.detailSize();
        Vector3 gsz = terrain.terrainSize();
        terrainSize = new Vector2(gsz.x, gsz.z);
        details = terrain.getDetails();
    }

    public void InheritBrain(SimpleNeuralNet other, bool mutate)
    {
        brain = new SimpleNeuralNet(other);
        if (mutate)
            brain.mutate(swapRate, mutateRate, swapStrength, mutateStrength);
    }
    public SimpleNeuralNet GetBrain()
    {
        return brain;
    }
    public float GetHealth()
    {
        return energy / maxEnergy;
    }

}
