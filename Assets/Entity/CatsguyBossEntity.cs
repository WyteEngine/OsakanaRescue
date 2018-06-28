using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatsguyBossEntity : BossEntity
{
	public override string WalkAnimationId => walkAnim;

	public override string StayAnimationId => stayAnim;

	public override string JumpAnimationId => jumpAnim;

	public override float GravityScale => useZeroGravity ? 0 : base.GravityScale;

	#region phase 1
	[Header("Phase1")]
	[SerializeField]
	private float distanceToSlide = 48;

	/// <summary>
	/// Phase 1の，滑り出すまでのプレイヤーとの距離を取得または設定します．
	/// </summary>
	/// <value>The distance to slide.</value>
	public float DistanceToSlide
	{
		get { return distanceToSlide; }
		set { distanceToSlide = value; }
	}

	[SerializeField]
	private float totalDistance = 240;

	public float TotalDistance
	{
		get { return totalDistance; }
		set { totalDistance = value; }
	}

	private float leftTarget, rightTarget;


	[SerializeField]
	private float dashSpeed = 48;

	public float DashSpeed
	{
		get { return dashSpeed; }
		set { dashSpeed = value; }
	}

	#endregion

	#region phase 2
	[Header("Phase 2")]
	[SerializeField]
	private float timeToWait;

	/// <summary>
	/// Phase 2の，パンチまでの待ち時間を取得または設定します．
	/// </summary>
	/// <value>The time to wait.</value>
	public float TimeToWait
	{
		get { return timeToWait; }
		set { timeToWait = value; }
	}

	[SerializeField]
	private float timeToPunch = 3;

	public float TimeToPunch
	{
		get { return timeToPunch; }
		set { timeToPunch = value; }
	}

	private float timeCacheForPunch;

	[SerializeField]
	private BulletEntity bulletEntity;

	public BulletEntity BulletToShoot
	{
		get { return bulletEntity; }
		set { bulletEntity = value; }
	}

	[SerializeField]
	private BallEntity balltoShoot;

	public BallEntity BallToShoot
	{
		get { return balltoShoot; }
		set { balltoShoot = value; }
	}


	[SerializeField]
	private float bulletSpeed = 64;

	public float BulletSpeed
	{
		get { return bulletSpeed; }
		set { bulletSpeed = value; }
	}
	#endregion

	#region phase 3
	[Header("Phase 3")]
	[SerializeField]
	private GameObject entityToThrow;

	/// <summary>
	/// 投げる敵Entityを取得します．
	/// </summary>
	/// <value>The entity to throw.</value>
	public GameObject EntityToThrow
	{
		get { return entityToThrow; }
		set { entityToThrow = value; }
	}
	#endregion

	/// <summary>
	/// 最大体力を取得します．
	/// </summary>
	/// <value>1フェーズ毎に体力が4あり，フェーズは3つあるので，合計12．</value>
	public override int MaxHealth => 4 * 3;

	bool ridedOnTheUfo = false;

	protected override IEnumerator OnBattle()
	{
		float x = transform.position.x;
		leftTarget = x - totalDistance;
		rightTarget = x;

		while (Health > 0)
		{
			if (Health <= 4)
				yield return DoPhase3();
			else if (Health <= 8)
				yield return DoPhase2();
			else
				yield return DoPhase1();
		}
	}

	protected override IEnumerator OnDeath(Object killer)
	{
		// 戦闘終了前のイベント実行(もし存在すれば)
		if (!string.IsNullOrEmpty(PostEvent))
		{
			Novel.Run(PostEvent);
			yield return new WaitWhile(() => Novel.Runtime.IsRunning);
		}
		yield return base.OnDeath(killer);
	}

	/// <summary>
	/// プレイヤーへ向けて走り，ある程度の距離で滑る．滑ったときに攻撃ができる．
	/// </summary>
	IEnumerator DoPhase1()
	{
		var target = direction == SpriteDirection.Left ? leftTarget : rightTarget;

		var vel = target < transform.position.x ? -dashSpeed : dashSpeed;

		SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
		Move(vel);
		// 対象位置に達するまで繰り返す
		while (vel < 0 ? target < transform.position.x : transform.position.x < target)
		{
			// 判定
			HandleAttackToThePlayer(1);
			if (HandleAttackFromThePlayer(1))
			{
				// 殴られたらプレイヤーを反動で飛ばす
				Wyte.CurrentPlayer.Velocity = new Vector2(Wyte.CurrentPlayer.Velocity.x, 80);
				// そしてたおれこむ
				Move(0);
				SetAnimations(AnimationSliding);
				yield return new WaitForSeconds(1);
				SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
				// 反対を向く
				direction = direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;
				yield break;
			}
			if (Mathf.Abs(target - transform.position.x) < distanceToSlide)
			{
				Move(vel * 1.5f);
				SetAnimations(AnimationSliding);
			}
			yield return null;
		}

		Move(0);
		float time = Time.time;
		while (Time.time - time < 0.5f)
		{
			if (HandleAttackFromThePlayer(1))
			{
				// 殴られたらプレイヤーを反動で飛ばす
				Wyte.CurrentPlayer.Velocity = new Vector2(Wyte.CurrentPlayer.Velocity.x, 128);
				// そしてたおれこむ
				Move(0);
				SetAnimations(AnimationSliding);
				yield return new WaitForSeconds(1);
				SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
				// 反対を向く
				direction = direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;
				yield break;
			}
			// 判定
			HandleAttackToThePlayer(1);
			yield return null;
		}
		SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
		// 反対を向く
		direction = direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;

		time = Time.time;
		while (Time.time - time < 1f)
		{
			if (HandleAttackFromThePlayer(1))
			{
				// 殴られたらプレイヤーを反動で飛ばす
				Wyte.CurrentPlayer.Velocity = new Vector2(Wyte.CurrentPlayer.Velocity.x, 128);
				// そしてたおれこむ
				Move(0);
				SetAnimations(AnimationSliding);
				yield return new WaitForSeconds(1);
				SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
				yield break;
			}
			// 判定
			HandleAttackToThePlayer(1);
			yield return null;
		}
	}

	/// <summary>
	/// プレイヤーに追尾し，ある程度のタイミングでパンチを行う．パンチまでは待ち時間がある．
	/// </summary>
	IEnumerator DoPhase2()
	{
		if (Wyte.CurrentPlayer == null) yield break;
		// 追尾
		SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
		while (timeCacheForPunch < timeToPunch)
		{
			Move(Mathf.Sign(Wyte.CurrentPlayer.transform.position.x - transform.position.x) * DashSpeed);
			timeCacheForPunch += Time.deltaTime;
			// 待機中は殴れる
			if (HandleAttackFromThePlayer(1))
			{
				// 殴られたらプレイヤーを反動で飛ばす
				Wyte.CurrentPlayer.Velocity = new Vector2(Wyte.CurrentPlayer.Velocity.x, 128);
				// そしてたおれこむ
				Move(0);
				SetAnimations(AnimationSliding);
				yield return new WaitForSeconds(1);
				SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
				yield break;
			}
			yield return null;
		}

		// 待機
		timeCacheForPunch = 0;
		SetAnimations(AnimationPunchPrepare, AnimationPunchPrepare, AnimationPunchPrepare);
		Move(0);
		var time = Time.time;
		Sfx.Play("entity.guy.scope");

		while (Time.time - time < TimeToWait)
		{
			// 待機中は殴れる
			if (HandleAttackFromThePlayer(1))
			{
				// 殴られたらプレイヤーを反動で飛ばす
				Wyte.CurrentPlayer.Velocity = new Vector2(Wyte.CurrentPlayer.Velocity.x, 128);
				// そしてたおれこむ
				Move(0);
				SetAnimations(AnimationSliding);
				yield return new WaitForSeconds(1);
				SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
				yield break;
			}
			yield return null;
		}
		const float wait = 0.1f;
		// パンチーコングって知ってる？
		// パ　ン　チ　ー　コ　ン　グだ二度と間違えるなくそが
		for (int i = 0; i < 3; i++)
		{
			// 手を前に
			SetAnimations(AnimationPunch);
			Sfx.Play("entity.guy.punch");
			yield return new WaitForSeconds(wait);

			var bullet = Instantiate(BulletToShoot.gameObject, transform.position, default(Quaternion)).GetComponent<BulletEntity>();

			// 手を後ろに
			bullet.Move(direction == SpriteDirection.Left ? -bulletSpeed : bulletSpeed);
			SetAnimations(AnimationStaying);
			yield return new WaitForSeconds(wait);
		}
	}

	/// <summary>
	/// 雷を発射する
	/// </summary>
	const int PatternLightning = 0;
	/// <summary>
	/// ボールを発射する
	/// </summary>
	const int PatternBall = 1;
	/// <summary>
	/// ザコ敵を召喚する
	/// </summary>
	const int PatternEnemy = 2;
	/// <summary>
	/// 体力回復を投げる
	/// </summary>
	const int PatternLife = 3;
	/// <summary>
	/// 氷を投げる
	/// </summary>
	const int PatternIce = 4;



	/// <summary>
	/// UFOに騎乗し，電撃，回復アイテム，ザコ敵を投げます．
	/// </summary>
	/// <returns>The phase3.</returns>
	IEnumerator DoPhase3()
	{
		useZeroGravity = true;
		// 準備
		if (!ridedOnTheUfo)
		{
			yield return PrePhase3();

			ridedOnTheUfo = true;
			yield break;
		}

		// 行動の抽選

	}

	IEnumerator PrePhase3()
	{
		// 速度と無敵時間をリセット
		Move(0);
		GodTime = 0;
		yield return new WaitForSeconds(2);

		// 高くジャンプ
		// Velocity プロパティはカメラ外に出ると機能しないので，自前で動かす
		SetAnimations(AnimationJumping);
		float targetToDown = transform.position.y + 32;
		Velocity = Vector2.zero;
		float targetToUp = Camera.CameraRects.yMax - 24;
		var velocity = new Vector2(0, 128);
		while (targetToUp > transform.position.y)
		{
			transform.Translate(velocity * Time.deltaTime);
			yield return null;
		}
		velocity = Vector2.zero;

		// 体力増強
		Wyte.Player.MaxLife = 8;
		Wyte.Player.Life = 8;
		Wyte.CanMove = false;
		yield return MessageContoller.Instance.Say(null, "ふしぎな　ちからが　みなぎる...\nさいだいHPが　おおきく　あがった！");

		// UFOが降りてくる
		Wyte.CanMove = true;
		SetAnimations(AnimationUfo);
		velocity = new Vector2(0, -128);
		while (targetToDown < transform.position.y)
		{
			transform.Translate(velocity * Time.deltaTime);
			yield return null;
		}
		velocity = Vector2.zero;
	}

	#region Helper

	protected void SetAnimations(string stay, string walk, string jump)
	{
		stayAnim = stay;
		walkAnim = walk;
		jumpAnim = jump;
	}

	protected void SetAnimations(string anim)
	{
		SetAnimations(anim, anim, anim);
	}

	/// <summary>
	/// プレイヤーが攻撃しようとしているかどうかを取得します．
	/// </summary>
	/// <returns>プレイヤーと衝突していて，プレイヤーが下降しているかどうか．</returns>
	protected virtual bool PlayerIsAttacking() => IsCollidedWithPlayer() && Wyte.CurrentPlayer.Velocity.y < 0;

	/// <summary>
	/// プレイヤーがダメージを受ける条件が揃っているかどうか取得します．
	/// </summary>
	/// <returns>プレイヤーと衝突していて，プレイヤーが下降していないかどうか．</returns>
	protected virtual bool PlayerWillBeDamaged() => IsCollidedWithPlayer() && Wyte.CurrentPlayer.Velocity.y >= 0;

	/// <summary>
	/// 条件が揃った場合，プレイヤーから攻撃を受けます．
	/// </summary>
	public virtual bool HandleAttackFromThePlayer(int amount)
	{
		if (!PlayerIsAttacking()) return false;
		Damage(Wyte.CurrentPlayer, amount);
		Sfx.Play("entity.player.step");
		return true;
	}

	/// <summary>
	/// 条件が揃った場合，プレイヤーを攻撃します．
	/// </summary>
	public virtual bool HandleAttackToThePlayer(int amount)
	{
		if (!PlayerWillBeDamaged()) return false;
		Wyte.CurrentPlayer.Damage(this, amount);
		return true;
	}


	#endregion

	#region private
	string walkAnim = "entity.guy.walk";
	string stayAnim = "entity.guy.stay";
	string jumpAnim = "entity.guy.jump";
	bool useZeroGravity;
	#endregion

	#region const
	const string AnimationWalking = "entity.guy.walk";
	const string AnimationStaying = "entity.guy.stay";
	const string AnimationJumping = "entity.guy.jump";
	const string AnimationDeath = "entity.guy.death";
	const string AnimationPunch = "entity.guy.punch";
	const string AnimationPunchPrepare = "entity.guy.punch.prepare";
	const string AnimationSliding = "entity.guy.slide";
	const string AnimationUfo = "entity.guy.ufo";
	#endregion
}