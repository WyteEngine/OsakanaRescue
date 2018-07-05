using UnityEngine;
using System.Collections;
using WyteEngine.Entities;

namespace Xeltica.Osakana
{
	public class LightningEntity : LivableEntity
	{
		[SerializeField]
		private float speed;

		public float Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		protected override void Start()
		{
			base.Start();
			Sfx.Play("entity.lightning.shoot");
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			Velocity = new Vector2(0, -Speed);
			if (IsCollidedWithPlayer())
			{
				Wyte.CurrentPlayer.Damage(this, 1);
				Damage(this, 1);
			}
			if (IsGrounded())
				Damage(this, 1);
		}

		protected override IEnumerator OnDeath(Object killer)
		{
			Sfx.Play("entity.lightning.fall");
			return base.OnDeath(killer);
		}

		public override string WalkAnimationId => "entity.lightning";
		public override string StayAnimationId => "entity.lightning";
		public override string JumpAnimationId => "entity.lightning";

		public override string LandSfxId => null;
		public override string JumpSfxId => null;
		public override string DeathSfxId => null;
	}
}