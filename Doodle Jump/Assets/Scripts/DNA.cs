using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public List<Vector2> genes = new List<Vector2>();
    public DNA(int genomeLength = 50)
    {
        for(int i = 0; i < genomeLength; i++)
        {
            genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), 0));
        }
    }
    public DNA(DNA parent, DNA parent2, float mutationRate = 0.01f)
    {
        for (int i = 0; i < parent.genes.Count; i++)
        {
            float mutationChance = Random.Range(0.0f, 1.0f);
            if(mutationChance < mutationRate)
            {
                genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), 0));
            }
            else
            {
                int chance = Random.Range(0,2); 
                if (chance == 0)
                {
                    genes.Add(parent.genes[i]);
                }
                else
                {
                    genes.Add(parent2.genes[i]);
                }
            }
        }
    }
    
}
