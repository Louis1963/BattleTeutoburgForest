using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]

public class DragObject : MonoBehaviour {
 


	private Vector3 screenPoint; 
	private Vector3 offset; 
	//this way you can lock on of the axes
	//private float _lockedYPosition;
	
	void OnMouseDown() {

		//Debug.Log ("on me");

		//screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position); // I removed this line to prevent centring

		//_lockedYPosition = screenPoint.y;

		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		Screen.showCursor = false;
 }
	
	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

		//curPosition.x = _lockedYPosition;

		transform.position = curPosition;
	}
	
	void OnMouseUp()
	{
		//Debug.Log ("left me");
		Screen.showCursor = true;
	}
}