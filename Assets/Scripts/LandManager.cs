using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;


public class LandManager : MonoBehaviour
{
    [SerializeField] int gridSizeX;
    [SerializeField] int gridSizeY;
    [SerializeField] GameObject squareTile;
    float offsetX = 1.0f;
    float offsetY = 1.0f;
    float startOfRow = 0f;
    float startOfColumn = 0f;
    Tile tile;


    Vector3 spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        CraftGrid();


    }

    private void CraftGrid()
    {
        // spawnPoint = new Vector3(0, 0, 0);

        for (int i = 0; i < gridSizeX; i++)
        {

            for (int j = 0; j < gridSizeY; j++)
            {
                spawnPoint = new Vector3(startOfRow, startOfColumn, 0); // move to bottom if doesnt work
                Instantiate(squareTile, spawnPoint, Quaternion.identity);
                startOfColumn = startOfColumn + offsetY; // adds on the width of one tile to the y axis.
            }
            startOfRow = startOfRow + offsetX;
            startOfColumn = 0f;
        }
    }


    // Update is called once per frame
    void Update()
    {




    }


}
