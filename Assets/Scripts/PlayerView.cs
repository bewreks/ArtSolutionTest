using UnityEngine;

public class PlayerView : MonoBehaviour
{
	[SerializeField] private Animator controller;
	
	private static readonly int Walking = Animator.StringToHash("Walking");

	public void StartWalking()
	{
		controller.SetBool(Walking, true);
	}

	public void StopWalking()
	{
		controller.SetBool(Walking, false);
	}
}