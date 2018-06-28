﻿using System.Collections;
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

	[SerializeField]
	private GameObject healItem;

	public GameObject HealItem
	{
		get { return healItem; }
		set { healItem = value; }
	}

	[SerializeField]
	private GameObject balltoShoot;

	public GameObject BallToShoot
	{
		get { return balltoShoot; }
		set { balltoShoot = value; }
	}

	[SerializeField]
	private GameObject thunder;

	public GameObject Thunder
	{
		get { return thunder; }
		set { thunder = value; }
	}

	[SerializeField]
	private GameObject ice;

	public GameObject Ice
	{
		get { return ice; }
		set { ice = value; }
	}


	float targetToDown;

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
		// 体力が6以下なら4回，違えば3回パンチ
		yield return DoPunch(Health < 6 ? 4 : 3);
		// 体力が6以下ならさらに4回パンチする
		if (Health <= 6)
		{
			yield return new WaitForSeconds(0.5f);
			yield return DoPunch(4);
		}

	}

	IEnumerator DoPunch(int count)
	{
		const float wait = 0.1f;
		for (int i = 0; i < count; i++)
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
	/// ただ左右に動く
	/// </summary>
	const int PatternMove = 5;



	/// <summary>
	/// UFOに騎乗し，電撃，回復アイテム，ザコ敵を投げます．
	/// </summary>
	/// <returns>The phase3.</returns>
	IEnumerator DoPhase3()
	{
		// 準備
		if (!ridedOnTheUfo)
		{
			useZeroGravity = true;
			yield return PrePhase3();

			// UFOに乗っているフラグをオンにしておく(PrePhase3() を呼ばないために)
			ridedOnTheUfo = true;
			yield break;
		}

		// 行動の抽選

		switch (Random.Range(0, 6))
		{
			case PatternBall:
				Instantiate(BallToShoot);
				break;
			case PatternLife:

				break;
			case PatternLightning: 
				yield return 
				break;
			case PatternEnemy: 

				break;
			case PatternIce:

				break;
			case PatternMove:
				// 少し下よりの中央に移動し
				yield return EaseOut(new Vector2((leftTarget + rightTarget) / 2, targetToDown - 18), 0.7f);
				// 目的地に行く
				yield return EaseOut(new Vector2(direction == SpriteDirection.Left ? leftTarget : rightTarget, targetToDown), 0.8f);

				yield return new WaitForSeconds(0.4f);

				// 振り向く
				direction = direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;
				break;
		}

		// 毎回少し休む
		yield return new WaitForSeconds(1.5f);

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
		targetToDown = transform.position.y + 64;
		Velocity = Vector2.zero;
		float targetToUp = Camera.CameraRects.yMax - 24;

		Vector2 destination = new Vector2(transform.position.x, targetToUp);

		yield return EaseIn(destination, 0.6f);

		// 体力増強
		Wyte.Player.MaxLife = 8;
		Wyte.Player.Life = 8;
		Wyte.CanMove = false;
		yield return MessageContoller.Instance.Say(null, "ふしぎな　ちからが　みなぎる...\nさいだいHPが　おおきく　あがった！");

		// UFOが降りてくる
		Wyte.CanMove = true;
		SetAnimations(AnimationUfo);

		destination = new Vector2(rightTarget, targetToDown);

		yield return EaseOut(destination, 1.5f);
		direction = SpriteDirection.Left;
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