using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WyteEngine.Utility
{
	public class Hidden : BaseBehaviour {

		[SerializeField]
		HiddenFlag hiddenOn = HiddenFlag.SmartDevice;

		// Use this for initialization
		void Start() {
			if ((hiddenOn == HiddenFlag.SmartDevice && IsSmartDevice) || (hiddenOn == HiddenFlag.PC && !IsSmartDevice))
				gameObject.SetActive(false);
		}


		public enum HiddenFlag
		{
			PC, SmartDevice
		}
	}
}