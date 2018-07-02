using UnityEngine;
using WyteEngine.Entities;
using WyteEngine.Entities.AI;

namespace Xeltica.Osakana
{
	/// <summary>
	/// ヒーリングアイテム．
	/// </summary>
	[RequireComponent(typeof(AIWalkAndWrapOnTheWall))]
	public class HeartEntity : LivableEntity
	{
		public override string WalkAnimationId => heartAnimId;

		public override string StayAnimationId => heartAnimId;

		public override string JumpAnimationId => heartAnimId;

		public override string LandSfxId => null;

		public override string JumpSfxId => null;

		public override string DeathSfxId => null;

		const string heartAnimId = "entity.heart";

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (IsCollidedWithPlayer())
			{
				Sfx.Play("entity.player.heal");
				Wyte.CurrentPlayer.Heal(this, 1);
				// 自殺しておく
				Kill(Wyte.CurrentPlayer);
			}
		}
	}
}