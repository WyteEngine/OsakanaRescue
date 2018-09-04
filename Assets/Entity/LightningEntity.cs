using System.Collections;
using WyteEngine.Entities;

namespace Xeltica.Osakana
{
	public class LightningEntity : LivableEntity
	{
		protected override void Start()
		{
			base.Start();
			Sfx.Play("entity.lightning.shoot");
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (IsCollidedWithPlayer())
			{
				Wyte.CurrentPlayer.Damage(this, 1);
			}

			if (!IsAnimating) Kill(this);
		}

		public override string WalkAnimationId => "entity.lightning";
		public override string StayAnimationId => "entity.lightning";
		public override string JumpAnimationId => "entity.lightning";

		public override string LandSfxId => null;
		public override string JumpSfxId => null;
		public override string DeathSfxId => null;

		// Stay
		public override float GravityScale => 0;
	}
}