using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoordinateLabeller : MonoBehaviour
{
    Vector2 tilePosition;
    float rawTilePosX;
    float rawTilePosY;
    string writtenCoords;
    float offsetX = 0.708f;         //I hate this - I need a reference to this from hexplacer script rather than these magic numbers.
    float interimOffsetY = 0.281f;  // Ugly 
    float offsetY = 0.561f;         // Ugly code
    float absTilePosX;              // 29/01 solution - use a counter on the placer to give the hexagons relative coordinates. 
    float absTilePosY;
    TextMeshPro tileUI;

    // Start is called before the first frame update
    void Start()
    {
        getTileCoordinates();
        tileUI = GetComponent<TextMeshPro>();
    }


    private void getTileCoordinates()
    {
        tilePosition = transform.parent.position;

        rawTilePosX = tilePosition.x;
        rawTilePosY = tilePosition.y;

        absTilePosX = rawTilePosX / offsetX;


        if (absTilePosX / 2 == 0)
        {
            absTilePosY = rawTilePosY / offsetY;
        }
        else
        {
            absTilePosY = rawTilePosY / interimOffsetY; 
        }

        //Debug.Log(absTilePosX.ToString());
        //Debug.Log(absTilePosY.ToString());

        //writtenCoords = absTilePosX.ToString() + "," + absTilePosY.ToString();
        //writtenCoords = rawTilePosX.ToString() + "," + rawTilePosY.ToString();


    }

    // Update is called once per frame
    public void displayCoords(string coordsToDisplay)
    {
        tileUI.text = coordsToDisplay; 

    }
}
