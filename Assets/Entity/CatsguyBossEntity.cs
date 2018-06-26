﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatsguyBossEntity : LivableEntity
{

	public override string WalkAnimationId => walkAnim;

	public override string StayAnimationId => stayAnim;

	public override string JumpAnimationId => jumpAnim;

	public override string LandSfxId => null;
	public override string JumpSfxId => null;
	public override string DeathSfxId => null;

	public override float GravityScale => useZeroGravity ? 0 : base.GravityScale;

	[Header("Events")]
	[SerializeField]
	private string preEvent;

	/// <summary>
	/// 戦闘開始前のイベントを取得または設定します．
	/// </summary>
	public string PreEvent
	{
		get { return preEvent; }
		set { preEvent = value; }
	}

	[SerializeField]
	private string postEvent;

	/// <summary>
	/// 戦闘開始前のイベントを取得または設定します．
	/// </summary>
	public string PostEvent
	{
		get { return postEvent; }
		set { postEvent = value; }
	}

	[SerializeField]
	private float distanceToBegin = 128;

	/// <summary>
	/// 戦闘が始まるためのプレイヤーとの距離．
	/// </summary>
	/// <value>The distance to begin.</value>
	public float DistanceToBegin
	{
		get { return distanceToBegin; }
		set { distanceToBegin = value; }
	}

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
	private float dashSpeed = 48;

	public float DashSpeed
	{
		get { return dashSpeed; }
		set { dashSpeed = value; }
	}


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

	/// <summary>
	/// 最大体力を取得します．
	/// </summary>
	/// <value>1フェーズにつき2，3フェーズあるので合計6．</value>
	public override int MaxHealth => 2 * 3;



	protected override void Start()
	{
		base.Start();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		// プレイヤーが然るべき距離に入ってきたら試合開始
		if (Wyte.CurrentPlayer != null && Mathf.Abs(transform.position.x - Wyte.CurrentPlayer.transform.position.x) < DistanceToBegin && !battleStarted)
		{
			StartCoroutine(Battle());
		}
	}

	IEnumerator Battle()
	{
		battleStarted = true;
		// 戦闘開始前のイベント実行(もし存在すれば)
		if (!string.IsNullOrEmpty(PreEvent))
		{
			Novel.Run(PreEvent);
			yield return new WaitWhile(() => Novel.Runtime.IsRunning);
		}
		while (Health > 0)
		{
			if (Health <= 2)
				yield return DoPhase3();
			else if (Health <= 4)
				yield return DoPhase2();
			else
				yield return DoPhase1();
		}
		// 戦闘終了前のイベント実行(もし存在すれば)
		if (!string.IsNullOrEmpty(PostEvent))
		{
			Novel.Run(PostEvent);
			yield return new WaitWhile(() => Novel.Runtime.IsRunning);
		}
	}

	/// <summary>
	/// プレイヤーへ向けて走り，ある程度の距離で滑る．滑ったときに攻撃ができる．
	/// </summary>
	IEnumerator DoPhase1()
	{
		var target = Wyte.CurrentPlayer.transform.position.x;
		var vel = target < transform.position.x ? -dashSpeed : dashSpeed;
		Debug.Log($"s{dashSpeed} t{target} v{vel}");
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
				SetAnimations(AnimationSliding);
				yield return new WaitForSeconds(1);
				SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
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
				SetAnimations(AnimationSliding);
				yield return new WaitForSeconds(1);
				SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
				yield break;
			}
			// 判定
			HandleAttackToThePlayer(1);
			yield return null;
		}
		SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
		time = Time.time;
		while (Time.time - time < 1f)
		{
			if (HandleAttackFromThePlayer(1))
			{
				// 殴られたらプレイヤーを反動で飛ばす
				Wyte.CurrentPlayer.Velocity = new Vector2(Wyte.CurrentPlayer.Velocity.x, 128);
				// そしてたおれこむ
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
		return DoPhase1();
	}

	/// <summary>
	/// UFOに騎乗し，電撃，回復アイテム，ザコ敵を投げます．
	/// </summary>
	/// <returns>The phase3.</returns>
	IEnumerator DoPhase3()
	{
		return DoPhase1();
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
	bool battleStarted;
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