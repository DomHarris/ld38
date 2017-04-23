using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	[SerializeField] Rigidbody2D rbody;
	[SerializeField] Collider2D col;
	InputHandler input = null;
	Transform player = null;
	
	void FixedUpdate()
	{
		if (player != null)
		{
			if (input == null) input = player.GetComponent<InputHandler>();
			if (!input.paused)
			{
				if (!rbody.simulated && col.enabled) rbody.simulated = true;
				rbody.MovePosition(Vector2.MoveTowards(transform.position, player.transform.position, .1f));
			} else if (rbody.simulated) rbody.simulated = false;
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			player = other.transform;
		}
	}
}
