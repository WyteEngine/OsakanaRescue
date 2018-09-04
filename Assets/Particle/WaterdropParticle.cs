using WyteEngine.Entities;

namespace Xeltica.Osakana
{
	/// <summary>
	/// 水滴のパーティクルです．
	/// </summary>
	public class WaterdropParticle : LivableEntity
	{
		const string CharacterId = "particle.waterdrop";
		public override string WalkAnimationId => CharacterId;

		public override string StayAnimationId => CharacterId;

		public override string JumpAnimationId => CharacterId;

		public override string LandSfxId => null;

		public override string JumpSfxId => null;

		public override string DeathSfxId => null;

		protected override void Start()
		{
			base.Start();
			// 他のEntityとぶつかるのはよくない
			collider2D.isTrigger = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			// 画面から出てたら殺す
			if (!Camera.CameraRects.Contains(transform.position))
				Kill(this);
			// 地面についたら弾ける
			if (IsGrounded())
			{
				Sfx.Play("particle.waterdrop.broken");
				Kill(this);
			}
		}
	}
}