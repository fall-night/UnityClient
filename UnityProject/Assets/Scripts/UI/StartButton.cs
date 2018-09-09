using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
	// Use this for initialization
	void Start () {
		
	}

	public void GoToGameScene()
	{
		SceneManager.LoadScene("Scene/GameScene");
	}

	public void LobbyScene()
	{
		SceneManager.LoadScene("Lobby");
	}
}
