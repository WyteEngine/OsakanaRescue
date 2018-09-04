using WyteEngine.Entities;
using UnityEngine;

namespace Xeltica.Osakana
{
	/// <summary>
	/// コンテナの破片パーティクルです．
	/// </summary>
	public class PieceOfContainerParticle : LivableEntity
	{
		const string CharacterId = "particle.piece_of_container";

		public override string WalkAnimationId => CharacterId;

		public override string StayAnimationId => CharacterId;

		public override string JumpAnimationId => CharacterId;

		public override string LandSfxId => null;

		public override string JumpSfxId => null;

		public override string DeathSfxId => null;

		protected override void Start()
		{
			base.Start();
			collider2D.enabled = false;

			if (Camera.CameraRects.Contains(transform.position))
				Kill(this);

			Velocity = new Vector2((Random.Range(-64, 64) / 4f), Random.Range(96, 128));
		}
	}

}