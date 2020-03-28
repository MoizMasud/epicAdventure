using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameBlock : MonoBehaviour
{
	bool isdoneLevel = false;
	LevelManager LevelManager = null;

	void Awake()
	{
		GameObject g = GameObject.FindGameObjectWithTag("LevelManager");
		if (g != null)
			LevelManager = g.GetComponent<LevelManager>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.layer == 9)
		{
			if (!isdoneLevel)
			{
				isdoneLevel = true;

				if (LevelManager == null)
				{
					GameObject g = GameObject.FindGameObjectWithTag("LevelManager");
					if (g != null)
						LevelManager = g.GetComponent<LevelManager>();
				}

				LevelManager.win();         // this will need to call level up
			}
		}
	}
}
