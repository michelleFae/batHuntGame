using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region movement_variables
    public float movespeed;
    float x_input;
    float y_input;
    #endregion

    #region physics_components
    Rigidbody2D playerRB;
    #endregion


    #region animation_components
    Animator anim;
    #endregion


    #region Unity_functions
    //Called once on creation
    private void Awake()
    {
    	playerRB = GetComponent<Rigidbody2D>();

    	anim = GetComponent<Animator>();

    	attackTimer = 0;

        currHealth = maxHealth;

        hpSlider.value = currHealth / maxHealth;

    }


    //Called every frame
    private void Update()
    {
    	if (isAttacking){
    		return;
    	}
    	//access our input values
    	x_input = Input.GetAxisRaw("Horizontal");
    	y_input = Input.GetAxisRaw("Vertical");
    	print(x_input);
    	Move();
    	if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0)
    	{
    		Attack();
    	}
    	else
    	{
    		attackTimer -= Time.deltaTime;
    	}

        //chest interact
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            Interact();
        }

    }
    #endregion

    #region attack_variables
    public float damage;
    // how long you have to wait before you can attack again
    public float attackspeed;
    #endregion

    #region health_variables
    public float maxHealth;
    float currHealth;
    public Slider hpSlider;
    // keeps track if we've actually waiting for attackspeed amt of time
    float attackTimer;

    // so there is a light ause between you throwing sword animation 
    // and the obj gettingdamaged animation
    public float hitboxTiming;
    public float endAnimationTiming;

    bool isAttacking;
    Vector2 curDirection;

    #endregion


    #region movement_functions
    //moves player based on WASD inputs and 'movespeed'
    private void Move()
    {
    	anim.SetBool("Moving", true);
    	//If player is pressing 'D'
    	if (x_input > 0)
    	{
    		//print("D :P");
    		playerRB.velocity = Vector2.right * movespeed;
    		curDirection = Vector2.right;
    	}
    	//If player is pressing 'A'
    	else if (x_input < 0)
    	{
    		print("A :P");
    		playerRB.velocity = Vector2.left * movespeed;
    		curDirection = Vector2.left;
    	}
    	  //If player is pressing 'W'
    	else if (y_input > 0)
    	{
    		print("W :P");
    		playerRB.velocity = Vector2.up * movespeed;
    		curDirection = Vector2.up;
    	}
    	//If player is pressing 'S'
    	else if (y_input < 0)
    	{
    		print("S :P");
    		playerRB.velocity = Vector2.down * movespeed;
    		curDirection = Vector2.down;
    	}
    	else 
    	{   
    		
    		playerRB.velocity = Vector2.zero;
    		anim.SetBool("Moving", false);
    		
    		
    	}

    	//Set animator directional values
    	anim.SetFloat("DirX", curDirection.x);
    	anim.SetFloat("DirY", curDirection.y);
    }
    #endregion

    #region attack_functions
    //Attacks in the direction the player is facing
    private void Attack()
    {
    	Debug.Log("Attacking now!");

    	//Handles all attack animations and calculates hitboxes
    	StartCoroutine(AttackRoutine());

    	attackTimer = attackspeed;
    }

    //handle animations and hitboxes for the attack mech
    IEnumerator AttackRoutine()
    {	//Pause mvmt and freeze player for the duration of the attack
    	isAttacking = true;
    	playerRB.velocity = Vector2.zero;

    	//Start Animation
    	anim.SetTrigger("Attack");

        //start sound effect
        FindObjectOfType<AudioManager>().Play("PlayerAttack");

    	//we will wait for hitboxtiming seconds before continuing whatever code is below this
    	yield return new WaitForSeconds(hitboxTiming); 

    	//create hitbox
    	RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + curDirection, Vector2.one,0f, Vector2.zero,0);
    	foreach (RaycastHit2D hit in  hits)
    	{
    		if (hit.transform.CompareTag("Enemy")){
    			print("damage");
                //damage enemy
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
    		}

    	}

    
    	yield return new WaitForSeconds(endAnimationTiming); 
    	//unfreeze player; reenables mvmt for player after attacking
    	isAttacking = false;
    }
    #endregion

    #region health_functions
    //take damage based on 'value' parameter, which is passed in by caller
    public void TakeDamage(float value)
    {   
        //start sound effect
        FindObjectOfType<AudioManager>().Play("PlayerHurt");

        //Decrement health
        currHealth -= value;
        Debug.Log("Health is now" + currHealth.ToString());

        //Change UI
        hpSlider.value = currHealth / maxHealth;

        //CHECK FOR DEATH
        if (currHealth <= 0){
            Die();
        }

    }

    public void Heal(float value)
    {
        currHealth += value;
        currHealth = Mathf.Min(currHealth, maxHealth);
        Debug.Log("Health is now " + currHealth.ToString());
     
        //Change UI
        hpSlider.value = currHealth / maxHealth;
    }

    //destroys player object and triggers end scene stuff 
    private void Die()
    {   //start sound effect
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        

        //Destroy Gameobject
        Destroy(this.gameObject);

        //Trigger anything we need to end the game, find game manage and lose game
        GameObject gm = GameObject.FindWithTag("GameController");

        gm.GetComponent<GameManager>().LoseGame();

    }
    #endregion

    #region interact_functions
    void Interact()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + curDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0);
        // for each hit, see if u hit a chest
        foreach (RaycastHit2D hit in hits){
            if (hit.transform.CompareTag("Chest"))
            {
                if (hit.transform.GetComponent<Chest>()){
                    hit.transform.GetComponent<Chest>().Interact();
                }
                
            }
            
        }

    }
    #endregion

}
