using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WyteEngine;

namespace Xeltica.Osakana
{

	public class EasterEgg : SingletonBaseBehaviour<EasterEgg>
	{

		string buffer = "";
		bool isEnabled = false;
		// Use this for initialization
		void Start()
		{
			Map.MapChanged += (map) => isEnabled = map.name == "Title";
		}

		// Update is called once per frame
		void Update()
		{
			if (!isEnabled)
				return;

			if (Input.GetKeyDown(KeyCode.UpArrow))
				buffer += 'U';
			if (Input.GetKeyDown(KeyCode.DownArrow))
				buffer += 'D';
			if (Input.GetKeyDown(KeyCode.LeftArrow))
				buffer += 'L';
			if (Input.GetKeyDown(KeyCode.RightArrow))
				buffer += 'R';
			if (Input.GetKeyDown(KeyCode.B))
				buffer += 'B';
			if (Input.GetKeyDown(KeyCode.A))
				buffer += 'A';

			if (buffer.Length > 10)
				buffer = buffer.Remove(0, buffer.Length - 10);

			if (buffer == "UUDDLRLRBA")
			{
				buffer = "";
				Camera.SwitchToPlayerCamera();
				Novel.Run("bat");
			}
		}
	}
}