using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    public List<GeneticMovement> population = new List<GeneticMovement>();
    public GameObject creaturePrefab;
    public int populationSize = 50;
    public float cutoff = 0.3f;
    public int genomeLength;

    Vector3 spawnPoint = new Vector3(0.0f, 0.11f, 0.0f);

    void InitPopulation()
    {
        for( int i = 0; i < populationSize; ++i)
        {
            GameObject go = Instantiate(creaturePrefab, spawnPoint, Quaternion.identity);
            go.GetComponent<GeneticMovement>().InitCreature(new DNA(genomeLength));
            population.Add(go.GetComponent<GeneticMovement>());
        }
    }
    void NextGeneration()
    {
        int survivorCut = Mathf.RoundToInt(populationSize * cutoff);
        List<GeneticMovement> survivors = new List<GeneticMovement>();
        for(int i = 0; i < survivorCut; ++i)
        {
            survivors.Add(GetFittest());
        }
        for(int i = 0; i < population.Count; i++)
        {
            Destroy(population[i].gameObject);
        }
        population.Clear();
        while(population.Count < populationSize)
        {
            for(int i = 0; i < survivors.Count; i++)
            {
                GameObject go = Instantiate(creaturePrefab, spawnPoint, Quaternion.identity);
                go.GetComponent<GeneticMovement>().InitCreature(new DNA(survivors[i].dna, survivors[Random.Range(0,10)].dna));
                population.Add(go.GetComponent<GeneticMovement>());
                if(population.Count  >= populationSize)
                {
                    break;
                }
            }
        }
        for(int i = 0; i < survivors.Count; i++)
        {
            Destroy(survivors[i].gameObject);
        }
    }
    private void Start()
    {
        InitPopulation();
    }
    private void Update()
    {
        if (!HasActive()){
            NextGeneration();
        }
    }

    GeneticMovement GetFittest()
    {
        float maxFitness = float.MinValue;
        int index = 0;
        for(int i = 0; i < population.Count; i++)
        {
            if(population[i].fitness > maxFitness)
            {
                maxFitness = population[i].fitness;
                index = i;
            }
        }
        GeneticMovement fittest = population[index];
        population.Remove(fittest);
        return fittest;
    }

    bool HasActive()
    {
        for(int i = 0; i < population.Count; i++)
        {
            if (!population[i].hasFinished)
            {
                return true;
            }
        }
        return false;
    }
}
