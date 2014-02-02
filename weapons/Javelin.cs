using UnityEngine;
using System.Collections;
using Holoville.HOTween; 

public class Javelin : MonoBehaviour
{	
	public Unit launcher;
	public Vector3 target;
	protected Animator animator;
	AudioSource audioSource;

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
				if (unit.specialGround == null || unit.specialGround.groundType != Ground.GroundType.WOOD) {		
					//how cool that we have a generic unit notion!!!
					unit.ChangeLifePoints (-1);
					unit.maxMovement -= 0.2F;
				}
				DestroyJavelin ();
			}
		}
	}

	public void LaunchByTo (Unit u, Vector3 target)
	{
		//if (audioSource != null && audioClip != null) {
		audioSource = gameObject.GetComponent<AudioSource> ();
		if (audioSource != null) {
			//audioSource.clip = audioClip;
			if (audioSource.clip != null)
				audioSource.Play ();
		}

		launcher = u;
		this.target = target;
		HOTween.To (transform, 1f, new TweenParms ()
		            .Prop ("position", target, false)
		            .Ease (EaseType.EaseOutCubic)
		            .OnComplete (DestroyJavelin));
	}

	private void DestroyJavelin ()
	{
		StartCoroutine (DestroyJavelinWait ());
	}

	IEnumerator DestroyJavelinWait ()
	{

		yield return new WaitForSeconds (2);
		Destroy (gameObject);
	}

}
