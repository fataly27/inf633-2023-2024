using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAlgo : MonoBehaviour
{

    [Header("Genetic Algorithm parameters")]
    public int preyPopSize = 100;
    public int predatorPopSize = 100;
    public GameObject preyPrefab;
    public GameObject predatorPrefab;

    [Header("Dynamic elements")]
    public float vegetationGrowthRate = 1.0f;
    public float currentGrowth;

    private List<GameObject> preys;
    private List<GameObject> predators;
    protected Terrain terrain;
    protected CustomTerrain customTerrain;
    protected float width;
    protected float height;
    public Prey[,] preysMap = null;
    public int preysMapLength = 400;

    protected void Start()
    {
        // Retrieve terrain.
        terrain = Terrain.activeTerrain;
        customTerrain = GetComponent<CustomTerrain>();
        width = terrain.terrainData.size.x;
        height = terrain.terrainData.size.z;
        preysMap = new Prey[preysMapLength, preysMapLength];

        // Initialize terrain growth.
        currentGrowth = 0.0f;

        // Initialize animals and predator arrays.
        preys = new List<GameObject>();
        predators = new List<GameObject>();
        for (int i = 0; i < preyPopSize; i++)
        {
            GameObject prey = makeAnimal(AnimalTypes.Prey);
            preys.Add(prey);
        }
        for (int i = 0; i < predatorPopSize; i++)
        {
            GameObject predator = makeAnimal(AnimalTypes.Predator);
            predators.Add(predator);
        }
    }

    void Update()
    {
        // Keeps preys to a minimum.
        while (preys.Count < preyPopSize / 2)
        {
            preys.Add(makeAnimal(AnimalTypes.Prey));
        }
        // Keeps predators to a minimum.
        while (predators.Count < predatorPopSize / 2)
        {
            predators.Add(makeAnimal(AnimalTypes.Predator));
        }
        customTerrain.debug.text = "N� preys: " + preys.Count.ToString() + "\nN� predators: " + predators.Count.ToString();

        // Registers preys positions
        Array.Clear(preysMap, 0, preysMap.Length);
        foreach (GameObject prey in preys)
        {
            // Retrieve prey location in the heighmap
            int dx = (int)((prey.transform.position.x / width) * preysMapLength);
            int dy = (int)((prey.transform.position.z / height) * preysMapLength);

            preysMap[dy, dx] = prey.GetComponent<Prey>();
        }

        // Update grass elements/food resources.
        updateResources();
    }

    /// <summary>
    /// Method to place grass or other resource in the terrain.
    /// </summary>
    public void updateResources()
    {
        Vector2 detail_sz = customTerrain.detailSize();
        int[,] details = customTerrain.getDetails();
        currentGrowth += vegetationGrowthRate;
        while (currentGrowth > 1.0f)
        {
            int x = (int)(UnityEngine.Random.value * detail_sz.x);
            int y = (int)(UnityEngine.Random.value * detail_sz.y);
            details[y, x] = 1;
            currentGrowth -= 1.0f;
        }
        customTerrain.saveDetails();
    }

    /// <summary>
    /// Method to instantiate an animal prefab. It must contain the animal.cs class attached.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject makeAnimal(AnimalTypes type, Vector3 position)
    {
        GameObject animal;

        if (type == AnimalTypes.Predator)
            animal = Instantiate(predatorPrefab, transform);
        else
            animal = Instantiate(preyPrefab, transform);

        animal.GetComponent<Animal>().Setup(customTerrain, this);
        animal.transform.position = position;
        animal.transform.Rotate(0.0f, UnityEngine.Random.value * 360.0f, 0.0f);
        return animal;
    }

    /// <summary>
    /// If makeAnimal() is called without position, we randomize it on the terrain.
    /// </summary>
    /// <returns></returns>
    public GameObject makeAnimal(AnimalTypes type)
    {
        Vector3 scale = terrain.terrainData.heightmapScale;
        float x = UnityEngine.Random.value * width;
        float z = UnityEngine.Random.value * height;
        float y = customTerrain.getInterp(x / scale.x, z / scale.z);
        return makeAnimal(type, new Vector3(x, y, z));
    }

    /// <summary>
    /// Method to add an animal inherited from another. It spawns where the parent was.
    /// </summary>
    /// <param name="parent"></param>
    public void addOffspring(Animal parent)
    {
        if (parent is Prey)
        {
            GameObject prey = makeAnimal(AnimalTypes.Prey, parent.transform.position);
            prey.GetComponent<Prey>().InheritBrain(parent.GetBrain(), true);
            preys.Add(prey);
        }
        if (parent is Predator)
        {
            GameObject predator = makeAnimal(AnimalTypes.Predator, parent.transform.position);
            predator.GetComponent<Predator>().InheritBrain(parent.GetBrain(), true);
            predators.Add(predator);
        }
    }

    /// <summary>
    /// Remove instance of an animal.
    /// </summary>
    /// <param name="animal"></param>
    public void removeAnimal(Animal animal)
    {
        if (animal is Prey)
            preys.Remove(animal.transform.gameObject);
        if (animal is Predator)
            predators.Remove(animal.transform.gameObject);

        Destroy(animal.transform.gameObject);
    }

}
