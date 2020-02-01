using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineofSight : MonoBehaviour
{
	// called wjen something enters the trigger colider
    private void OnTriggerEnter2D(Collider2D coll){
    	if (coll.CompareTag("Player"))
    	{
    		GetComponentInParent<Enemy>().player = coll.transform;
    		Debug.Log("run at player");
    	}

    }

    
}
