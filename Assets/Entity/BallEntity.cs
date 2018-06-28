using System.Collections;
using UnityEngine;

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

	public BulletEntity Parent { get; set; }

	private string ballAnimationId;

	protected override void Start()
	{
		base.Start();
		ballAnimationId = $"entity.ball.{Random.Range(0, 3)}";
		Velocity = new Vector2(Wyte.CurrentPlayer != null && Wyte.CurrentPlayer.transform.position.x < transform.position.x ? -80 : 80, 0);
	}

	protected override void OnUpdate()
	{
		// はね続ける
		Jump();

		if (CanKickLeft() || CanKickRight())
		{
			Kill(this);
		}
	}

	protected override IEnumerator OnDeath(Object killer)
	{
		// ボール破裂音
		Sfx.Play("entity.ball.explosion");

		if (!(killer is PlayerController) || Parent == null)
			return base.OnDeath(killer);

		// プレイヤーに踏まれるとUFOを攻撃する
		while (Vector2.Distance(transform.position, Parent.transform.position) > 4)
		{
			transform.position = Vector2.Lerp(transform.position, Parent.transform.position, 5f * Time.deltaTime);
		}
		Sfx.Play("event.explosion");
		Parent.Damage(this, 1);

		return base.OnDeath(killer);
	}
}
