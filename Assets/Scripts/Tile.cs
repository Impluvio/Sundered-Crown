using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    // vars for settting the sprites                            //        
    private SpriteRenderer spriteRenderer;                      
    public Sprite[] spriteArray;[Tooltip("Add sprites Here")]   
    int randomNumber;                                          
    string tileName;
    bool isLand;
    // vars for labelling the tiles with thier coordinates      //
    TextMeshPro tileUI;                                                                 
    Vector2 tileCoordinates;                                    
    float xCoord;
    float yCoord;
    string writtenCoords;
    // vars for getting the neighbouring tiles                  //
    public GameObject tileNeighbourUp;
    public GameObject tileNeighbourDown;
    public GameObject tileNeighbourLeft;
    public GameObject tileNeighbourRight;



    void Start()
    {

        randomNumber = Random.Range(0, 4);
        spriteRenderer = gameObject.GetComponentInParent<SpriteRenderer>();
        tileName = GetComponentInParent<SpriteRenderer>().sprite.name;
        tileUI = GetComponent<TextMeshPro>();
        ChangeSprite();
        NameTile();
        GetCoordinates();
        getNeighbours();
    }

    private void GetCoordinates()
    {
        tileCoordinates = transform.parent.position;

        xCoord = tileCoordinates.x;
        yCoord = tileCoordinates.y;

        writtenCoords = xCoord.ToString() + "," + yCoord.ToString();
        tileUI.text = writtenCoords;
        //Debug.Log(writtenCoords);
    }

    private void ChangeSprite()
    {
        spriteRenderer.sprite = spriteArray[randomNumber];
    }

    public void NameTile()
    {
        gameObject.transform.root.name = tileName;
    }

    public void getNeighbours()
    {
        BoundsInt myBounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        foreach (var b in myBounds.allPositionsWithin)
        {
            Vector3 worldPos = transform.TransformPoint(b);
            Debug.Log(worldPos);
        } 

        //Debug.Log(writtenCoords);
        //Debug.Log(myBounds.allPositionsWithin);


    }
}
