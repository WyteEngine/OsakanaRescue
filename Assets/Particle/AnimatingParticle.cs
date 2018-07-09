using UnityEngine;
using WyteEngine.Entities;

namespace Xeltica.Osakana
{
	public class AnimatingParticle : SpriteEntity
	{
		[SerializeField]
		private string spriteId;

		public string SpriteId
		{
			get { return spriteId; }
			set { spriteId = value; }
		}

		protected override void Start()
		{
			base.Start();
			ChangeSprite(spriteId);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!IsAnimating)
				Kill(this);
		}
	}

}