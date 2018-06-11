using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEntity : FlyableEntity
{
	public override string StayAnimationId => "entity.enemy.bat.stay";

	public override string DeathSfxId => null;

	public override string FlyAnimationId => "entity.enemy.bat.fly";
}
