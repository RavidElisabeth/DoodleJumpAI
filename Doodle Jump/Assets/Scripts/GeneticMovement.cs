using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticMovement : MonoBehaviour
{
    public float creatureSpeed;
    public float pathMultiplier;
    int pathIndex = 0;
    public DNA dna;
    public bool hasFinished = false;
    bool hasBeenInitialized = false;

    Vector2 nextPoint;

    private void Start()
    {
        InitCreature(new DNA());
    }
    public void InitCreature(DNA newDNA)
    {
        dna = newDNA;
        nextPoint = transform.position;
        hasBeenInitialized = true;
    }
    private void Update()
    {
        if (hasBeenInitialized && !hasFinished)
        {
            if(pathIndex == dna.genes.Count)
            {
                
            }
            if((Vector2)transform.position == nextPoint)
            {
                nextPoint = (Vector2)transform.position + dna.genes[pathIndex];
                pathIndex++;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, creatureSpeed * Time.deltaTime);
            }
        }
    }

    void End()
    {
        hasFinished = true;
    }
    public float fitness
    {
        get
        {
            float height = transform.position.y;
            return height / 10000;
        }
    }
}
