using UnityEngine;
using System.Collections;

[System.Serializable]
public enum PlayerEnum {
	p1_,
	p2_,
	p3_,
	p4_
}

public abstract class InputBase : MonoBehaviour
{
	protected bool isActive = true;
	[SerializeField] protected PlayerEnum player = PlayerEnum.p1_;
	// Use this for initialization
	void Start ()
	{
		StartCoroutine(GetInputs());
	}
	
	// Update is called once per frame
	IEnumerator GetInputs ()
	{
		while (isActive)
		{
			if (Input.anyKey)
			{

				// if (Input.GetKey(KeyCode.A)) LeftHorizontal(-1);
				// else if (Input.GetKey(KeyCode.D)) LeftHorizontal(1);
				// else LeftHorizontal(0);

				// if (Input.GetKey(KeyCode.W)) LeftVertical(1);
				// else if (Input.GetKey(KeyCode.S)) LeftVertical(-1);
				// else LeftVertical(0);
				
				// if (Input.GetKey(KeyCode.LeftArrow)) RightHorizontal(-1);
				// else if (Input.GetKey(KeyCode.RightArrow)) RightHorizontal(1);
				// else RightHorizontal(0);

				// if (Input.GetKey(KeyCode.UpArrow)) RightVertical(1);
				// else if (Input.GetKey(KeyCode.DownArrow)) RightVertical(-1);
				// else RightVertical(0);
				LeftHorizontal(Input.GetAxis("LeftHorizontal"));

				LeftVertical(Input.GetAxis("LeftVertical"));
			
				RightHorizontal(Input.GetAxis("RightHorizontal"));
			
				RightVertical(Input.GetAxis("RightVertical"));
			} else {
				LeftHorizontal(Input.GetAxis(player + "LeftHorizontal"));

				LeftVertical(Input.GetAxis(player + "LeftVertical"));
			
				RightHorizontal(Input.GetAxis(player + "RightHorizontal"));
			
				RightVertical(Input.GetAxis(player + "RightVertical"));
			}

			
			yield return new WaitForFixedUpdate();
		}
	}

	protected abstract void LeftHorizontal (float val);
	protected abstract void LeftVertical (float val);
	protected abstract void RightHorizontal (float val);
	protected abstract void RightVertical (float val);
}
