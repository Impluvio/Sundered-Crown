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
    public string tileName; // use get set method in future
    public bool isLand = false;
    // vars for labelling the tiles with thier coordinates      //
    TextMeshPro tileUI;                                                                 
    Vector2 tileCoordinates;                                    
    float xCoord;
    float yCoord;
    public string writtenCoords;
    public GameObject[] tileNeighbours;

    // vars for getting the neighbouring tiles                  //This could be a class.
    public LandManager landManager;
   


    public void Awake()
    {

        randomNumber = Random.Range(0, 4);
        spriteRenderer = gameObject.GetComponentInParent<SpriteRenderer>();
        //tileName = GetComponentInParent<SpriteRenderer>().sprite.name;    // likely not needed as called in change sprite method 
        tileUI = GetComponent<TextMeshPro>();
        landManager = FindObjectOfType<LandManager>();
        ChangeSprite();                                                    
        GetCoordinates();
        NameTile();
    }

    private void GetCoordinates()
    {
        tileCoordinates = transform.parent.position;

        xCoord = tileCoordinates.x;
        yCoord = tileCoordinates.y;

        writtenCoords = xCoord.ToString() + "," + yCoord.ToString();
        tileUI.text = writtenCoords;
        //Debug.Log(writtenCoords);
        tileName = writtenCoords;
    }

    public void SetNeighbours(GameObject[] neighbours)
    {
        tileNeighbours = neighbours;
    }


    public void setLandOrSea(bool isLand)
    {
        this.isLand = isLand;
        if (isLand == true)
        {
            
            spriteRenderer.sprite = spriteArray[0];
            //Debug.Log("This tile is" + writtenCoords +"sprite changed");
        }
        else
        {
            spriteRenderer.sprite = spriteArray[2];
        }
    }


    private void ChangeSprite()
    {
        if(isLand == true)
        {
            spriteRenderer.sprite = spriteArray[1];
        }
        else
        {
            spriteRenderer.sprite = spriteArray[2];
        }
        
        //tileName = GetComponentInParent<SpriteRenderer>().sprite.name;
    }

    public void NameTile()
    {
        gameObject.transform.root.name = tileName; //this needs fixing as every tile is called grass - i suspect this is due to not naming the sprites.

    }



    void Update()
    {
          
    }

   
}
