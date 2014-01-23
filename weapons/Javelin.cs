using UnityEngine;
using System.Collections;
using Holoville.HOTween; 

public class Javelin : MonoBehaviour
{	
	public Unit launcher;
	public Vector3 target;
	protected Animator animator;

	void Start ()
	{
		animator = gameObject.GetComponent<Animator> ();
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		bool isHighAbove = animator.GetCurrentAnimatorStateInfo (0).IsName ("JavelinUpwards");
		if (launcher != null && !isHighAbove) {

			Unit unit = col.gameObject.GetComponent<Unit> ();

			if (unit != null && unit != launcher) {
			
				//how cool that we have a generic unit notion!!!
				unit.changeLifePoints (-1);
				unit.maxMovement -= 0.2F;


				Destroy (gameObject);
			}
		}
	}

	public void LaunchByTo (Unit u, Vector3 target)
	{
		launcher = u;
		this.target = target;
		HOTween.To (transform, 1f, new TweenParms ()
		            .Prop ("position", target, false)
		            .Ease (EaseType.EaseOutCubic)
		            .OnComplete (DestroyJavelin));
	}

	void DestroyJavelin ()
	{
		Destroy (gameObject);
	}

}
