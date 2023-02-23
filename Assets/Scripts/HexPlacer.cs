using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexPlacer : MonoBehaviour
{
    
    
    // maybe what we need to do is to see how many times 708 goes into the coordinates to calc the x axis.
    // find a way of getting the width of the tile
    // consider axial coordinates
    [SerializeField] int gridSizeX;
    [SerializeField] int gridSizeY;
    [SerializeField] GameObject hexTile;
    float offsetX = 0.708f;
    float interimOffsetY = 0.281f;
    float offsetY = 0.561f;
    float xAxis = 0f;
    float yAxis = 0f;

    // relative coordinates.
    TextMeshPro tileUI;
    int counterY = 0;
    int counterX = 0;
    bool stepBaseLine = false;
    string writtenCoords;
    CoordinateLabeller coordinateLabeller;
    



    Vector3 spawnPoint; 
    // Start is called before the first frame update
    void Start()
    {
        
        CraftGrid();
        
    }

    private void CraftGrid()
    {
        spawnPoint = new Vector3(0, 0, 0);

        for (int i = 0; i < gridSizeX; i++)
        {

            for (int i2 = 0; i2 < gridSizeY; i2++)
            {
                spawnPoint = new Vector3(xAxis, yAxis, 0); // move to bottom if doesnt work
                Instantiate(hexTile, spawnPoint, Quaternion.identity);
                yAxis = yAxis + offsetY; // adds on the width of one tile to the y axis.

                //writtenCoords = counterX.ToString() + "," + counterY.ToString();
                //conveyCoords();
                //counterY++;


            }

            stepBaseLine = !stepBaseLine;
            xAxis = xAxis + offsetX;
            if (stepBaseLine == true)
            {
                yAxis = interimOffsetY;
            }
            else
            {
                yAxis = 0f;
            }

            counterX++;
            counterY = 0;



        }
    }



    private void conveyCoords()
    {
        coordinateLabeller.displayCoords(writtenCoords);
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }
}
