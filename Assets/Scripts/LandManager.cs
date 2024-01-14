using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class LandManager : MonoBehaviour
{
    [SerializeField] int gridSizeX;                                     // the grid to iterate on X
    [SerializeField] int gridSizeY;                                     // the grid to iterate on Y
    [SerializeField] GameObject squareTile;                             // Tile prefab
    [SerializeField] int noOfLargeWalkers;                              // number of random walkers generated
    [SerializeField] int noOfSmallWalkers;
    [SerializeField] int cellAutIterations;
    [SerializeField] int numberOfMountainWalkers = 4;
    private int[,] heightMap;
    private int walkerSteps; 
    float offsetX = 1.0f;                                               // The constant that the grid moves on by as the loop runs
    float offsetY = 1.0f;                                               // see above but for Y
    float startOfRow = 0f;                                              // starting point
    float startOfColumn = 0f;                                           // starting point. 
    Tile tile;
    public Dictionary<Vector3Int, GameObject> coordObjDictionary = new Dictionary<Vector3Int, GameObject>();
    GameObject objectToBeSpawned;




    //GameObject upNeighbourTile;
    //GameObject downNeighbourTile; 
    //GameObject leftNeighbourTile;
    //GameObject rightNeighbourTile;

    Vector3 spawnPoint; //check if this is required

    // Start is called before the first frame update
    void Start()
    {
        CraftGrid();
        SetTileNeighbours();
        GenerateLandMasses();
        // PrintDict();

    }

    private void CraftGrid()
    {
        // spawnPoint = new Vector3(0, 0, 0);

        for (int i = 0; i < gridSizeX; i++)
        {

            for (int j = 0; j < gridSizeY; j++)
            {
                spawnPoint = new Vector3(startOfRow, startOfColumn, 0);                         // sets spawnpoint.
                objectToBeSpawned = Instantiate(squareTile, spawnPoint, Quaternion.identity);   // instantiates the object & sets object to be spawned
                startOfColumn += offsetY;                                                       // adds on the width of one tile to the y axis.
                Vector3Int currentCoords = Vector3Int.FloorToInt(spawnPoint);                   // floor to int removes float issues ready to pass into dict
                coordObjDictionary.Add(currentCoords, objectToBeSpawned);                       // adds object as value and coords as key
                                                     // feeds the i & j variables from craft grid method to setTileNeighbours method 
            }
            startOfRow = startOfRow + offsetX;
            startOfColumn = 0f;
        }

        //Debug.Log("No. of Tiles" + coordObjDictionary.Count);
        // foreach reffing each tile 



    } // functions normally

    private void SetTileNeighbours()
    {
        //this method need to iterate over the dictionary which contains all the elements of the grid. 
        // it needs to find the neighbours for each of the tiles in the list, feed these into an array and pass this array to the tile to store
        // it needs a current tile, to hold the tile to have its neighbours searched, and it needs to not pass in neighbours that do not exitst edge tiles would have 3 neighbours
        // and corners would have only 2. 

        

        foreach (KeyValuePair<Vector3Int, GameObject> pair in coordObjDictionary)
        {

            Vector3Int coordinates = pair.Key;
            GameObject tileObject = pair.Value;

            GameObject[] neighbours = new GameObject[4];

            //grab neighbours - then check neighbours are in bounds.  x y z 

            Vector3Int upOneTileCoord = new Vector3Int(coordinates.x, coordinates.y + 1, 0);
            Vector3Int downOneTileCoord = new Vector3Int(coordinates.x, coordinates.y - 1, 0);
            Vector3Int leftOneTileCoord = new Vector3Int(coordinates.x - 1, coordinates.y, 0);
            Vector3Int rightOneTileCoord = new Vector3Int(coordinates.x + 1, coordinates.y, 0);

            if (upOneTileCoord.x >= 0 && coordinates.x < gridSizeX && coordinates.y >= 0 && coordinates.y < gridSizeY)
            {
                neighbours[0] = FindObjectByKey(upOneTileCoord);
            }
            else
            {
            }

            if (upOneTileCoord.x >= 0 && coordinates.x < gridSizeX && coordinates.y >= 0 && coordinates.y < gridSizeY)
            {
                neighbours[1] = FindObjectByKey(downOneTileCoord);
            }
            else
            {
            }
            if (upOneTileCoord.x >= 0 && coordinates.x < gridSizeX && coordinates.y >= 0 && coordinates.y < gridSizeY)
            {
                neighbours[2] = FindObjectByKey(leftOneTileCoord);
            }
            else
            {
            }
            if (upOneTileCoord.x >= 0 && coordinates.x < gridSizeX && coordinates.y >= 0 && coordinates.y < gridSizeY)
            {
                neighbours[3] = FindObjectByKey(rightOneTileCoord);
            }
            else
            {
            }


            //get neighbours, and check if neighbours are in bounds
            Tile currentTile = coordObjDictionary[coordinates].GetComponentInChildren<Tile>();
            currentTile.SetNeighbours(neighbours);

            // checked it is setting the neighbours correctly using script below. 

            //foreach (GameObject value in neighbours)
            //{

            //    if(value != null)
            //    {
            //        Debug.Log("the current tile" + currentTile.writtenCoords + "has these neighbours: " + value.transform.position);
            //    }
                
            //}

        }

    } // functions normally 

    private GameObject FindObjectByKey(Vector3Int key)
    {
        if (coordObjDictionary.TryGetValue(key, out GameObject neighbourTile))
        {
            return neighbourTile;
        }
        else
        {
            return null;
        }
    } // functions normally

    private void GenerateLandMasses()
    {
        int largeWalker = gridSizeX * gridSizeY;
        int smallWalker = UnityEngine.Random.Range(gridSizeX * gridSizeY / 30, gridSizeX * gridSizeY / 20);

        for (int i = 0; i < noOfLargeWalkers; i++)
        {
            RandomWalker(largeWalker);
        }

        for(int i = 0; i < noOfSmallWalkers; i++)
        {
            RandomWalker(smallWalker);
        }

        CellularAutomata(cellAutIterations);

    } //functions normally

    private void RandomWalker(int maxSteps)
    {
        int randomSeedX = UnityEngine.Random.Range(0, gridSizeX);
        int randomSeedY = UnityEngine.Random.Range(0, gridSizeY);

        Vector3Int startTileCoords = new Vector3Int(randomSeedX, randomSeedY, 0);
        GameObject startTile = coordObjDictionary[startTileCoords];

        int currentSteps = 0;

        while (currentSteps < maxSteps)
        {
            Tile currentTile = startTile.GetComponentInChildren<Tile>();


            if(currentTile != null)
            {
                
                
                currentTile.setLandOrSea(true);
                int randomDirection = UnityEngine.Random.Range(0, 4);
                startTile = MoveToNeighbour(startTile, randomDirection);
                currentSteps++;
                //Debug.Log(currentSteps);
            }
            else
            {
                Debug.LogError("currentTile is null. Aborting GenerateLandMasses.");
                break; // Exit the loop to prevent an infinite loop
            }
           
            //Debug.Log("code hits end of while loop");
        }


    } // functions normally (rename) // Sets number of walkers to create the map

    private GameObject MoveToNeighbour(GameObject currentTile, int direction)
    {
        Tile tile = currentTile.GetComponentInChildren<Tile>();
        if (tile != null && tile.tileNeighbours[direction] != null && direction >= 0 && direction < tile.tileNeighbours.Length)
        {
            GameObject nextTile  = tile.tileNeighbours[direction];

            if (nextTile != null)
            {
                return nextTile;
            }

        }
        
            return currentTile;

    } // functions normally (rename) 

    private void CellularAutomata(int cellAutIterations)
    {
        for (int i = 0; i < cellAutIterations; i++)
        {
            Dictionary<Vector3Int, GameObject> newState = new Dictionary<Vector3Int, GameObject>(coordObjDictionary);

            foreach (KeyValuePair<Vector3Int, GameObject> pair in coordObjDictionary)
            {
                Vector3Int coordinates = pair.Key;
                GameObject tileObject = pair.Value;
                
                Tile currentTile = tileObject.GetComponentInChildren<Tile>();
                int landCount = CountLandNeighbours(coordinates); // this is the source of a null error 

                if (landCount >= 3)
                {
                    newState[coordinates].GetComponentInChildren<Tile>().setLandOrSea(true);
                }
                if(landCount < 2)
                {
                    newState[coordinates].GetComponentInChildren<Tile>().setLandOrSea(false);
                }

            }

            coordObjDictionary = new Dictionary<Vector3Int, GameObject>(newState);

            foreach (var pair in newState)
            {
                Debug.Log($"Key: {pair:Key}, Value: {pair.Value}");
            }

        }
    } // functions normally 

    private int CountLandNeighbours(Vector3Int coordinates) //counts the neighbours
    {
        int landCount = 0;

        Tile currentTileToCountNeighboursFor = coordObjDictionary[coordinates].GetComponentInChildren<Tile>();

        Debug.Log("hits post landCount");
        foreach(GameObject neighbour in currentTileToCountNeighboursFor.tileNeighbours)
        {

            Debug.Log("enters foreach loop countlandneighbours");
            if (neighbour != null)
            {
                Tile neighbourTile = neighbour.GetComponentInChildren<Tile>();

                if (neighbour != null && neighbourTile.isLand)
                {
                    Debug.Log("enters if statement");
                    landCount++;
                    Debug.Log("land count : " + landCount);
                }
            }
                
        }
        return landCount;
    }

    private void GenerateMountainLines() // being developed 
    {
        //how far the walker will go
        //how to bias the walker so it tends in one direction
        //the walker needs to appear randomly
        //store the line in an array or list (for creating branches)
        // 
        
        heightMap = new int[gridSizeX, gridSizeY]; //stores the 

        for (int i = 0; i < numberOfMountainWalkers; i++)
        {

            int spawnCoordsX = UnityEngine.Random.Range(0, gridSizeX);
            int spawnCoordsY = UnityEngine.Random.Range(0, gridSizeY);
           // MountainWalkers(spawnCoordsX, spawnCoordsY);
        }

    }

    //private void MountainWalkers(int spawnCoordsX, int spawnCoordsY)
    //{
    //    int x = spawnCoordsX;
    //    int y = spawnCoordsY;
    //    walkerSteps = UnityEngine.Random.Range(gridSizeX * gridSizeY / 10, gridSizeX * gridSizeY / 2);

    //    for (int j = 0; j < walkerSteps;)
    //    {
    //        heightMap[x, y] = 1;

    //        GameObject currentTile = coordObjDictionary



    //    }
    //}






    // Update is called once per frame
    void Update()
    {
       






    }


}
