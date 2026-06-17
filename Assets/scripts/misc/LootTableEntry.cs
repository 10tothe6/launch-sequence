using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (03/10/2026)
// this class came from supplyrun so it might be old, but it seems to do the job

// an array of this class makes a loot table
[System.Serializable]
public class LootTableEntry
{
    // normally this just references the item array, 
    // but you can make a loot table for anything
    public string itemIndex; // this isn't an index, it's a name!!!

    // these are all added up and divided by the number of entries,
    // like the weights for a loot table could literally just be [1000, 200, 500],
    // it's all relative
    public int weight;

    public LootTableEntry(string name, int weight)
    {
        itemIndex = name;
        this.weight = weight;
    }

    // p-randomly grab a loot table item from a table, respecting weights
    public static string Get(LootTableEntry[] table)
    {
        // probably don't need to use 2 for loops here
        int totalWeights = 0;
        for (int i = 0; i < table.Length; i++) { totalWeights += table[i].weight; }

        // TODO: pRandom?
        int randomValue = Random.Range(1, totalWeights);

        int previousWeightTotal = 0;
        for (int i = 0; i < table.Length; i++)
        {
            if (randomValue >= previousWeightTotal && randomValue <= previousWeightTotal + table[i].weight)
            {
                return table[i].itemIndex;
            }

            previousWeightTotal += table[i].weight;
        }

        return "";
    }

    public static string Get(System.Random pRandom, LootTableEntry[] table)
    {
        // probably don't need to use 2 for loops here
        int totalWeights = 0;
        for (int i = 0; i < table.Length; i++) { totalWeights += table[i].weight; }

        // TODO: pRandom?
        int randomValue = pRandom.Next(1, totalWeights);

        int previousWeightTotal = 0;
        for (int i = 0; i < table.Length; i++)
        {
            if (randomValue >= previousWeightTotal && randomValue <= previousWeightTotal + table[i].weight)
            {
                return table[i].itemIndex;
            }

            previousWeightTotal += table[i].weight;
        }

        return "";
    }
}