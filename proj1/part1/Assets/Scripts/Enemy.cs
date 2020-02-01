using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    #region movement_variables
    public float movespeed;
    #endregion

    #region physics_components
    Rigidbody2D enemyRB;
    #endregion


    // Access to the player itself
    #region targeting_variables
    public Transform player;
    #endregion

    #region health_variables
    public float maxHealth;
    float currHealth;
    #endregion

    #region health_functions
    //take damage based on 'value' parameter, which is passed in by caller
    public void TakeDamage(float value)
    {           //start sound effect
        FindObjectOfType<AudioManager>().Play("BatHurt");
        //Decrement health
        currHealth -= value;
        Debug.Log("Health is now" + currHealth.ToString());

        //Change UI

        //CHECK FOR DEATH
        if (currHealth <= 0){
            print("oops it died");
            Die();
        }

    }
    void Die(){
        Destroy(this.gameObject);
    }

    #endregion



    #region Unity_functions
    private void Awake()
    {
    	enemyRB = GetComponent<Rigidbody2D>();
        currHealth = maxHealth;


    }
    private void Update(){
    	///check to see if we know whee the player is
    	if (player == null) {
    		//don't know where player is
    		return;
    	}
    	Move();
    }
    #endregion


    #region attack_variables
    public float explosionDamage;
    public float explosionRadius;

    //to store prefab
    public GameObject explosionObj;
    #endregion


    #region movement_functions
    //Move directly at player
    private void Move(){
    	//Calculate mvmt vector
    	//Player pos - enemy pos = diection player relative to enemy
        Vector2 direction = player.position - transform.position;

        enemyRB.velocity = direction.normalized * movespeed; //mormalizing to remove extra speed with larger difference
    

    }

    #endregion

    #region attack_functions

	//Recasts box for player and causes damage, spawns explosion prefab
    private void Explode()
    {   
        //start sound effect
        FindObjectOfType<AudioManager>().Play("Explosion");
    	RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);
    	foreach (RaycastHit2D hit in hits)
    	{
    		if (hit.transform.CompareTag("Player"))
    		{
    			//Cause damage to player
                hit.transform.GetComponent<PlayerController>().TakeDamage(explosionDamage);

    			Debug.Log("Hit Player with explosion");

    			//spawns explosion prefab
    			Instantiate(explosionObj, transform.position, transform.rotation);




    		}
    	}
    	
    	//deactivate the obj
        print("explosion time!");
    	
    	
    }


    // called wjen something collides with the trigger colider
    private void OnCollisionEnter2D(Collision2D coll){
    	if (coll.transform.CompareTag("Player"))
    	{   
    		Explode();
            Destroy(this.gameObject);
    	}

    }

    #endregion

}
