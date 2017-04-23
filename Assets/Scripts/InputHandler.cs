using UnityEngine;
using System.Collections;
public class InputHandler : InputBase
{
	[SerializeField] ScoreManager score;
	[SerializeField] AudioManager audioManager;
	private bool _paused = true;
	public bool paused { get { return _paused; } set { _paused = value; rbody.simulated = !value; } }
	[SerializeField] bool crazyMode = false;
	[SerializeField] float speed = 5;
	[SerializeField] float firerate = .1f;
	float timer = 0;
	[SerializeField] ParticleSystem ps;
	[SerializeField] ParticleCollision pcol;

	[SerializeField] Rigidbody2D rbody;

	float rightX = 0;
	float rightY = 0;

	[SerializeField] float jumpTimer = 0;
	[SerializeField] LayerMask groundLayer;

	bool tripleShot = false;
	float jumpTimerMax = 0.1f;

	float powerupTimer = 0;
	protected override void LeftHorizontal (float val)
	{
		if (!paused)
		{
			rbody.velocity += Vector2.right * val * speed;
			if (rbody.velocity.x > 5) rbody.velocity = new Vector2 (5, rbody.velocity.y);
			if (rbody.velocity.x < -5) rbody.velocity = new Vector2 (-5, rbody.velocity.y);
		}
	}
	protected override void LeftVertical (float val)
	{
		if (!paused)
		{
			if (!crazyMode)
			{
				if (jumpTimer < jumpTimerMax && val > 0)
				{
					rbody.velocity += Vector2.up * val * speed;
					jumpTimer += Time.fixedDeltaTime;
				} else if (val < 0)
				{
					rbody.velocity += Vector2.up * val * speed;
				}
			} else
				rbody.velocity += Vector2.up * val * speed;
		}
	}
	protected override void RightHorizontal (float val)
	{
		if (!paused)
			rightX = val;
	}
	protected override void RightVertical (float val)
	{
		if (!paused)
			rightY = val;
	}

	void Update ()
	{
		if (!paused)
		{
			timer += Time.deltaTime;
			if ((rightX != 0 || rightY != 0) && timer >= firerate)
			{
				timer = 0;
				ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
				ep.velocity = new Vector3 (rightX, rightY) * 8;
				if (!crazyMode)
					ep.position = transform.position;
				ep.startColor = new Color32 ((byte)Random.Range(0, 255), (byte)Random.Range(0, 255),(byte)Random.Range(0, 255),255);
				ps.Emit(ep, crazyMode ? 50 : 1);

				if (tripleShot)
				{
					ep.position += Vector3.up * 0.2f;
					ps.Emit(ep, 1);
					ep.position -= Vector3.up * 0.4f;
					ps.Emit(ep, 1);
				}
			}

			if (Physics2D.OverlapCircle(transform.position + Vector3.down * 0.25f, 0.1f, groundLayer))
			{
				jumpTimer = 0;
			}

			if (crazyMode)
			{
				powerupTimer += Time.deltaTime;
				if ( powerupTimer > 5)
					crazyMode = false;
			}
		}
	}

	public void Powerup()
	{
		crazyMode = true;
		powerupTimer = 0;
	}

	IEnumerator PowerupCountdown ()
	{
		yield return new WaitForSeconds(5);
		crazyMode = false;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (!paused)
		{
			if (other.gameObject.CompareTag("Enemy"))
			{
				other.gameObject.SetActive(false);
				score.UpdateTimer(-20);
				audioManager.PlaySound("hit");
			}
		}
	}


	public void IncreaseFirerate( )
	{
		firerate -= 0.02f;
		if (firerate < 0.01f) firerate = 0.01f;
	}

	public void IncreaseRadius ()
	{
		pcol.IncreaseRadius();
	}

	public void IncreaseJumpTimer()
	{
		jumpTimerMax += 0.1f;
	}

	public void TripleShot ()
	{
		tripleShot = true;
	}
}
