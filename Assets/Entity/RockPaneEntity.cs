using UnityEngine;
using WyteEngine.Entities;

namespace Xeltica.Osakana
{
	public class RockPaneEntity : LivableEntity
	{
		public override string WalkAnimationId => id;

		public override string StayAnimationId => id;

		public override string JumpAnimationId => id;

		public override string LandSfxId => null;

		public override string JumpSfxId => null;

		public override string DeathSfxId => null;

		const string id = "entity.rock.pane";


		protected override void Start()
		{
			base.Start();
			
			Velocity = new Vector2(Random.Range(-32, 32), 96);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			if (IsGrounded())
			{
				Kill(null);
				Sfx.Play("entity.rock.destroy");
			}
		}
	}

	}