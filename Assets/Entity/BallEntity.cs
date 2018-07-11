using System.Collections;
using UnityEngine;
using WyteEngine.Entities;
using WyteEngine.Entities.AI;

namespace Xeltica.Osakana
{

	/// <summary>
	/// よく跳ねる柔らかいボール
	/// </summary>
	[RequireComponent(typeof(AISteppableEnemy))]
	public class BallEntity : LivableEntity
	{
		public override string WalkAnimationId => ballAnimationId;

		public override string StayAnimationId => ballAnimationId;

		public override string JumpAnimationId => ballAnimationId;

		public override string LandSfxId => null;

		public override string JumpSfxId => "entity.ball.bounce";

		public override string DeathSfxId => null;

		public CatsguyBossEntity Parent { get; set; }

		private string ballAnimationId;

		// 3回まで跳ね返りを許す
		private int reflectionCount = 3;

		protected override void Start()
		{
			base.Start();
			ballAnimationId = $"entity.ball.{Random.Range(0, 3)}";
			Velocity = new Vector2(Wyte.CurrentPlayer != null && Wyte.CurrentPlayer.transform.position.x < transform.position.x ? -80 : 80, 0);
		}

		protected override void OnUpdate()
		{
			if (IsGrounded() && !prevIsGrounded)
			{
				Velocity = new Vector2(Velocity.x, 96);
				Sfx.Play(JumpSfxId);
			}

			if (CanKickLeft() || CanKickRight())
			{
				reflectionCount--;
				// 反転
				Velocity *= Vector2.left;
				if (reflectionCount <= 0)
					Kill(this);
				else
					Sfx.Play(JumpSfxId);
			}
			base.OnUpdate();
		}

		protected override IEnumerator OnDeath(Object killer)
		{
			// ボール破裂音
			Sfx.Play("entity.ball.explosion");

			if (!(killer is PlayerController) || Parent == null)
			{
				yield return base.OnDeath(killer);
				yield break;
			}

			// プレイヤーに踏まれるとUFOを攻撃する
			while (Vector2.Distance(transform.position, Parent.transform.position) > 32)
			{
				transform.position = Vector2.Lerp(transform.position, Parent.transform.position, .1f);
				yield return null;
			}
			Sfx.Play("event.explosion");
			Parent.Damage(this, 1);

			yield return base.OnDeath(killer);
		}
	}
}