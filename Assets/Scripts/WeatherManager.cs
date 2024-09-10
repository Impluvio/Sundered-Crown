using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public GameObject Cloud1;
    //add in other prefabs with alternative cloud shapes. 
    public int numberOfClouds = 5;
    public Vector2 worldBounds;
    
    public void GenerateClouds(int MapX, int MapY)
    {
        worldBounds = new Vector2(MapX, MapY);

        for (int i = 1; i < numberOfClouds; i++)
        {
            spawnCloud(worldBounds);
        }


    }
    //lets use an object pool here in the future and have a limited number of clouds instantiated.
    //consider using a separate method to randomise cloud movement
    private void spawnCloud(Vector2 worldBounds)
    {
        Vector2 spawnPosition = new Vector2(UnityEngine.Random.Range(0, worldBounds.x), UnityEngine.Random.Range(0, worldBounds.y));
        GameObject newCloud = Instantiate(Cloud1, spawnPosition, Quaternion.identity);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
