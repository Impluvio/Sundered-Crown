using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LandManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray; [Tooltip("Add sprites Here")]
    int randomNumber;
    string tileName;
    
    



    // Start is called before the first frame update

    void Start()
    {

        randomNumber = Random.Range(0, 4);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        ChangeSprite();
        tileName = GetComponent<SpriteRenderer>().sprite.name;
        nameTile();
    }

   


    private void ChangeSprite()
    {
        spriteRenderer.sprite = spriteArray[randomNumber];
    }

    public void nameTile()
    {
        this.gameObject.name = tileName;
    }



    // Update is called once per frame
    void Update()
    {
        
    }

}
