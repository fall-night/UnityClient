using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{

	[SerializeField] private GameObject popUp;
	
	public void Attack() 
	{
		this.popUp.SetActive(false);
		Debug.Log("Attack!!");
	}
}
