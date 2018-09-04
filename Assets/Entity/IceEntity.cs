using UnityEngine;
using WyteEngine.Entities;

namespace Xeltica.Osakana
{
	/// <summary>
	/// 氷の Entity です．
	/// </summary>
	public class IceEntity : LivableEntity
	{
		protected override void Start()
		{
			base.Start();
			Velocity = new Vector2(0, -24f);
			direction = Wyte.CurrentPlayer != null && transform.position.x < Wyte.CurrentPlayer.transform.position.x ? SpriteDirection.Right : SpriteDirection.Left;
		}

		[SerializeField]
		private float speed = 48;

		public float Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		protected float SpeedMultiplier => 1f / (1 + AnimationIndex);

		protected override void OnUpdate()
		{
			// 反射
			if (CanKickLeft())
				direction = SpriteDirection.Right;
			if (CanKickRight())
				direction = SpriteDirection.Left;

			Velocity = new Vector2(Speed * SpeedMultiplier, Velocity.y);

			if (!IsAnimating)
			{
				Kill(this);
			}
			base.OnUpdate();
		}

		public override string WalkAnimationId => "entity.ice";
		public override string StayAnimationId => "entity.ice";
		public override string JumpAnimationId => "entity.ice";

		public override string LandSfxId => null;
		public override string JumpSfxId => null;
		public override string DeathSfxId => null;
	}
}