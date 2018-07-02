using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Xeltica.Osakana
{
	public class BatEntity : FlyableEntity
	{
		public override string WaitingAnimationId => "entity.enemy.bat.stay";

		public override string DeathSfxId => null;

		public override string FlyAnimationId => "entity.enemy.bat.fly";
	}
}