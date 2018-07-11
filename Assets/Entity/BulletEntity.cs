using UnityEngine;
namespace Xeltica.Osakana
{
	public class BulletEntity : FlyableEntity
	{
		public override string FlyAnimationId => "entity.projectile.bullet";
		public override string WaitingAnimationId => "entity.projectile.bullet";
		public override string DeathSfxId => null;

		float time;

		protected override void OnUpdate()
		{
			base.OnUpdate();

			time += Time.deltaTime;

			if (time > 0.09f)
			{
				ParticleAPI.Instance.Summon("explosion", transform.position);
				time = 0;
			}

			// 無敵でないプレイヤーが来たら爆発
			if (IsCollidedWithPlayer() && Wyte.CurrentPlayer.GodTime <= 0)
			{
				Explode(Wyte.CurrentPlayer, true);
			}

			if (CanKickLeft() || CanKickRight())
				Explode(this);
		}

		protected virtual void Explode(Object interacter, bool attackToThePlayer = false)
		{
			Kill(interacter);
			if (attackToThePlayer)
				Wyte.CurrentPlayer.Damage(this, 1);
			ParticleAPI.Instance.Summon("big_explosion", transform.position);
			Sfx.Play("entity.lightning.shoot");
		}
	}
}