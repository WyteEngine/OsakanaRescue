// using http://nn-hokuson.hatenablog.com/entry/2017/05/09/202141

using UnityEngine;
using System.Collections;
using WyteEngine;
using System;

namespace Xeltica.Osakana
{
	public class PostEffect : BaseBehaviour
	{
		public Material sepia;

		private bool enabled;

		private void Start()
		{
			Novel.Runtime.Register("sepia", SwitchSepia);
		}

		private IEnumerator SwitchSepia(string spriteTags, string[] args)
		{
			if (args.Length > 0)
				enabled = args[0].ToLower() == "on";
			yield break;
		}

		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (!enabled)
				Graphics.Blit(src, dest);
			else
				Graphics.Blit(src, dest, sepia);
		}
	}
}