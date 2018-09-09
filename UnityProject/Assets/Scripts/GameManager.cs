using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private List<GameObject> labelList = null;

	private int clickIndex = 0;
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Clicked()
	{
		if (this.clickIndex < 8)
			this.labelList[this.clickIndex++].SetActive(true);
	}
}
