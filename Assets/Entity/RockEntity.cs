using System.Collections;
using UnityEngine;
using WyteEngine.Entities;
using WyteEngine.Entities.AI;

namespace Xeltica.Osakana
{
	public class RockEntity : LivableEntity
	{
		public override string WalkAnimationId => id;

		public override string StayAnimationId => id;

		public override string JumpAnimationId => id;

		public override string LandSfxId => null;

		public override string JumpSfxId => null;

		public override string DeathSfxId => null;

		[SerializeField]
		GameObject rockPane;

		const string id = "entity.rock";
		

		protected override void Start()
		{
			base.Start();

			var y = Mathf.Abs(transform.position.y - Wyte.CurrentPlayer.transform.position.y);

			var g = Mathf.Abs(GravityScale);

			var t = Mathf.Sqrt(2 * g * y) / g;

			var x =Wyte.CurrentPlayer.transform.position.x - transform.position.x;

			Velocity = new Vector2(x / t, 0);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (IsCollidedWithPlayer())
			{
				Wyte.CurrentPlayer.Damage(this, 1);
				// 自殺しておく
				Kill(Wyte.CurrentPlayer);
			}

			if (IsGrounded())
			{
				Kill(null);
			}
		}

		protected override IEnumerator OnDeath(Object killer)
		{
			var panes = Random.Range(1, 4);
			for (int i = 0; i < panes; i++)
			{
				// 気持ち高めなとこでスポーンさせる
				Instantiate(rockPane, transform.position + Vector3.up * 8, transform.rotation);
			}
			Sfx.Play("entity.rock.destroy");
			return base.OnDeath(killer);
		}
	}

	}