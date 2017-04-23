using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	[SerializeField] float shakeAmount;

	public void Shake ()
	{
		StartCoroutine(ShakeRoutine());
	}

	IEnumerator ShakeRoutine ()
	{
		transform.localPosition += new Vector3 (Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount));
		yield return new WaitForFixedUpdate();
		transform.localPosition = Vector3.back;
	}
}
