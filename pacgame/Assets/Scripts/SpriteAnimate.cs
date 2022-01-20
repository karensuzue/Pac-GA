using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimate : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites; // frames

    private float switchTime = 0.125f; // time between each frame

    private int frameNumber = 0; // current frame 

    public bool loop = true;

    // Start is called before the first frame update
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (loop) 
        {
            InvokeRepeating("AdvanceFrame", switchTime, switchTime);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private  void AdvanceFrame()
    {
        frameNumber++;

        if (frameNumber >= sprites.Length) 
        {
            frameNumber = 0;
        }

        if (frameNumber >= 0 && frameNumber < sprites.Length) {
            spriteRenderer.sprite = sprites[frameNumber];
        }
    }

}
