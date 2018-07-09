using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WyteEngine;
namespace Xeltica.Osakana
{
	public class Letterbox : BaseBehaviour
	{
		[SerializeField]
		private Transform left;
		public Transform Left
		{
			get { return left; }
			set { left = value; }
		}

		[SerializeField]
		private Transform right;
		public Transform Right
		{
			get { return right; }
			set { right = value; }
		}

		[SerializeField]
		private Transform top;

		public Transform Top
		{
			get { return top; }
			set { top = value; }
		}



		protected override void Update()
		{
			base.Update();
			var cr = Camera.CameraRects;

			var horizontalSize = (cr.width - 320) / 2;

			//			top.transform.localPosition = new Vector2(-cr.width / 2, cr.height / 2);
			//			top.transform.localScale = new Vector2(cr.width, Mathf.Max(0, cr.height - 180));
			top.transform.localScale = new Vector3();
			left.transform.localPosition = new Vector2(-cr.width / 2, cr.height / 2);
			left.transform.localScale = new Vector2(horizontalSize, cr.height);

			right.transform.localPosition = new Vector2(cr.width / 2 - horizontalSize, cr.height / 2);
			right.transform.localScale = new Vector2(horizontalSize, cr.height);

		}
	}
}