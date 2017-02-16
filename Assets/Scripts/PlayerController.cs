using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {

	private Player player;
	// Use this for initialization
	void Start () {
		player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			player.ToggleRotation();
		}
	}

}
