using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class LicenseLoader : MonoBehaviour
{
	[SerializeField]
	TextAsset licenseText;

	Text ui;

	// Use this for initialization
	void Start()
	{
		ui = GetComponent<Text>();
	}

	private void Update()
	{
		if (ui == null)
			ui = GetComponent<Text>();
		if (licenseText != null && ui.text != licenseText.text)
			ui.text = licenseText.text;
	}
}