﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all player behavior data during game (Movement/Score/Life)
public class Player : MonoBehaviour
{
    public float moveSpeed, jumpHeight, jumpCount, jumpTime, footRadius; //Movement
    public LayerMask whatIsGround;
    public Transform feetPos;
    private Vector3 startPos;
    private float jumpTimeCounter;
    private bool isJumping;

    public bool dead; //Life

    public int score, numOfCoins; //Score
    public int highScore, totalCoins, lastScore; //Lifetime scores

    // Start is called before the first frame update
    void Start()
    {
        LoadPlayer();
        dead = false;
        score = 0;
        numOfCoins = 0;
        startPos = transform.position;
        
    }
    // FixedUpdate is called every fixed frame-rate frame
    void FixedUpdate()
    {
        if (!GetComponent<Renderer>().IsVisibleFrom(Camera.main)) //if player is no longer in view of camera (i.e. player crashed too many times)
            dead = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            totalCoins += numOfCoins;
            lastScore = score;

            if (highScore < score)
                highScore = score;

            SavePlayer();
            gameObject.SetActive(false);
        }
        else if (!UIManager.gameIsPaused)
        {
           
            Vector3 currentPos = Movement();

            float distRan = currentPos.x - startPos.x;

            if (distRan > score)
            {
                score++;

                if (score % 100 == 0) //every 100 points
                    moveSpeed+=.5f;

            }
        }
    } 

    //players movement data
    Vector3 Movement()
	{
        transform.Translate(Vector2.right * Time.deltaTime * moveSpeed); //player auto run

        bool jumpButtonPressed = (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)); //player tapped the screen / pressed space button
        bool onGround = (Physics2D.OverlapCircle(feetPos.position, footRadius, whatIsGround) && !isJumping);

        if (onGround)
        {
            jumpCount = 0; //player jump recharges when hitting ground
            GetComponent<Animator>().SetTrigger("Run"); //Run animation
        }

        if (jumpButtonPressed && jumpCount < 2) //player can only double jump before hitting ground
        {
            jumpCount++;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse); //player jump
            GetComponent<Animator>().SetTrigger("Jump"); //jump animation
        }

        /* Length of time holding jump button affects how high in the air player goes */
        bool jumpButtonHeld = (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space));

        if (jumpButtonHeld && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse); //player jump
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
                GetComponent<Animator>().SetTrigger("Land"); //land animation
            }
        }

        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0)) && isJumping) //lets go of jump button
        { 
            isJumping = false;
            GetComponent<Animator>().SetTrigger("Land"); //land animation
        }

        return transform.position;
    }

    // Sent when another object enters a trigger collider attached to this object
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "deathblock")
        { //player touches out-of-bounds block
            dead = true;
        }
        else if (col.gameObject.tag == "coin")
        { //player touches coin
            FindObjectOfType<AudioManager>().Play("coin");
            numOfCoins++;
        }
      
    }
    
    // Sent when an incoming collider makes contact with this object's collider
    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "enemy") //player runs into enemy
            dead = true;
      
    }

    // Message recieved after killing enemy
    void KilledEnemy()
    {
        numOfCoins += 25;
    }

    // Saves player data
    void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    // Loads player data
    void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        highScore = data.highScore;
        totalCoins = data.totalCoins;
        lastScore = data.lastScore;
    }

}
