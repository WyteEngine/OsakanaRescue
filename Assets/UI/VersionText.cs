using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WyteEngine.UI
{
	[RequireComponent(typeof(Text))]
	public class VersionText : BaseBehaviour
	{
		Text ui;
		// Use this for initialization
		void Start()
		{
			ui = GetComponent<Text>();
			ui.text = $"Version: {Wyte.GameVersion}\nWyte Version: {Wyte.LongVersion}";
		}
	}
}