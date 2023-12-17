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
                SetTileNeighbours(currentCoords, i, j);                                         // feeds the i & j variables from craft grid method to setTileNeighbours method 
            }
            startOfRow = startOfRow + offsetX;
            startOfColumn = 0f;
        }

        //Debug.Log("No. of Tiles" + coordObjDictionary.Count);
        // foreach reffing each tile 



    }

    private void SetTileNeighbours(Vector3Int currentCoords, int i, int j)
    {
        GameObject[] neighbours = new GameObject[4];

        //Acquire Neighbouring coords.
        Vector3Int upNeighbour = new Vector3Int(i, j + 1, 0);
        Vector3Int downNeighbour = new Vector3Int(i, j - 1, 0);
        Vector3Int leftNeighbour = new Vector3Int(i - 1, j, 0);
        Vector3Int rightNeighbour = new Vector3Int(i + 1, j, 0);

        neighbours[0] = FindObjectByKey(upNeighbour);
        neighbours[1] = FindObjectByKey(downNeighbour);
        neighbours[2] = FindObjectByKey(leftNeighbour);
        neighbours[3] = FindObjectByKey(rightNeighbour);

        Tile currentTile = coordObjDictionary[currentCoords].GetComponentInChildren<Tile>();
        currentTile.SetNeighbours(neighbours);

        Debug.Log($"Tile at {currentCoords} has neighbors: " +
       $"up: {neighbours[0]}, down: {neighbours[1]}, left: {neighbours[2]}, right: {neighbours[3]}");


    }

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
    }

    private void GenerateLandMasses()
    {
        // Debug.Log("code hits Generate Land Masses");
        Vector3Int startTileCoords = new Vector3Int(gridSizeX / 2, gridSizeY / 2, 0);
        GameObject startTile = coordObjDictionary[startTileCoords];
        // Debug.Log("start tile coords =" + startTileCoords);
        int maxSteps = gridSizeX * gridSizeY;
        int currentSteps = 0;

        while (currentSteps < maxSteps)
        {

            // Debug.Log("code hits begining of while loop"); // it does
            Tile currentTile = startTile.GetComponent<Tile>();

            Debug.Log($"The value for key '{startTileCoords}' is: {startTile.name}" );
            
            if(currentTile != null)
            {
                Debug.Log("Enters If statement");
                currentTile.setLandOrSea(true);
                Debug.Log("hits set Land or sea");
                int randomDirection = UnityEngine.Random.Range(0, 3);

                startTile = MoveToNeighbour(startTile, randomDirection);

                currentSteps++;
            }
           else
            {
                Debug.LogError("currentTile is null. Aborting GenerateLandMasses.");
                break; // Exit the loop to prevent an infinite loop
            }
           
            Debug.Log("code hits end of while loop");
        }


    }

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

    }




    //private void PrintDict()
    //{

    //    foreach (KeyValuePair<Vector3Int, GameObject> pair in coordObjDictionary)
    //    {
    //        //Debug.Log("Key: " + pair.Key + ", Value: " + pair.Value.name);
    //    }

    //    foreach (KeyValuePair<Vector3Int, GameObject> pair in coordObjDictionary)
    //    {

    //        Debug.Log("key no:" + pair.Key);


    //        Vector3Int upNeighbourCoords = new Vector3Int(pair.Key.x, pair.Key.y + 1, pair.Key.z);
    //        Vector3Int downNeighbourCoords = new Vector3Int(pair.Key.x, pair.Key.y - 1, pair.Key.z);
    //        Vector3Int rightNeighbourCoords = new Vector3Int(pair.Key.x + 1, pair.Key.y, pair.Key.z);
    //        Vector3Int leftNeighbourCoords = new Vector3Int(pair.Key.x - 1, pair.Key.y, pair.Key.z);


    //        // Debug.Log("Key Neighbours" + upNeighbourCoords + downNeighbourCoords + rightNeighbourCoords + leftNeighbourCoords);

    //        upNeighbourTile = FindObjectByKey(upNeighbourCoords);
    //        downNeighbourTile = FindObjectByKey(downNeighbourCoords);
    //        rightNeighbourTile = FindObjectByKey(rightNeighbourCoords);
    //        leftNeighbourTile = FindObjectByKey(leftNeighbourCoords);

    //        if (rightNeighbourTile != null) { Debug.Log(rightNeighbourTile.name); }


    //    }

    //}





    // Update is called once per frame
    void Update()
    {
       





    }


}
