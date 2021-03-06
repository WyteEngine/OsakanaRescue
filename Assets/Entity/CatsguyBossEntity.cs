﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WyteEngine.UI;
namespace Xeltica.Osakana
{
	public class CatsguyBossEntity : BossEntity
	{
		public override string WalkAnimationId => walkAnim;

		public override string StayAnimationId => stayAnim;

		public override string JumpAnimationId => jumpAnim;

		public override float GravityScale => useZeroGravity ? 0 : base.GravityScale;

		[SerializeField]
		private string eventOfTrueEnd;

		[SerializeField]
		private string flagOfTrueEnd = "got_the_photoframe";

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
		private GameObject lightning;

		public GameObject Thunder
		{
			get { return lightning; }
			set { lightning = value; }
		}

		[SerializeField]
		private GameObject ice;

		public GameObject Ice
		{
			get { return ice; }
			set { ice = value; }
		}

		[SerializeField]
		private GameObject rock;

		public GameObject Rock
		{
			get { return rock; }
			set { rock = value; }
		}

		private float targetHeightToShootLightning;


		float targetToDown;

		#endregion

		private bool isDoingEvent;

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
			targetHeightToShootLightning = transform.position.y - 16;
			while (Health > 0)
			{
				if (isDoingEvent)
				{
					yield return null;
					continue;
				}
				if (Health <= 4)
					yield return DoPhase3();
				else if (Health <= 8)
					yield return DoPhase2();
				else
					yield return DoPhase1();
			}
		}

		protected override IEnumerator OnDamaged(Object interacter)
		{
			if (Flag.Flags[flagOfTrueEnd] && interacter is BallEntity && Health <= 2)
			{
				if (string.IsNullOrEmpty(eventOfTrueEnd))
					yield break;
				isDoingEvent = true;

				Wyte.CanMove = false;

				yield return Bgm.Stop(2);
				yield return new WaitForSeconds(.5f);

				// true end event を実行する
				GodTime = 0;
				yield return EaseOut(new Vector2(rightTarget, targetToDown), 0.5f);
				yield return new WaitForSeconds(.5f);
				
				Novel.Run(eventOfTrueEnd);
				yield return new WaitWhile(() => Novel.Runtime.IsRunning);
			}
		}

		protected override IEnumerator OnDeath(Object killer)
		{
			Bgm.Stop();
			isDoingEvent = true;
			// アニメーション
			Velocity = Vector2.zero;
			Direction = Wyte.CurrentPlayer != null ? (transform.position.x < Wyte.CurrentPlayer.transform.position.x ? SpriteDirection.Right : SpriteDirection.Left) : Direction;
			var v = new Vector2(Direction == SpriteDirection.Left ? -1 : 1, -4);
			for (int i = 0; i < 24; i++)
			{
				transform.Translate(v);
				ParticleAPI.Instance.Summon("explosion", Random.insideUnitCircle * 64 + (Vector2)transform.position);
				if (i >= 8)
				{
					// blast
					Sfx.Play("entity.guy.blast");
					ParticleAPI.Instance.Summon("big_explosion", Random.insideUnitCircle * 64 + (Vector2)transform.position);

				}
				yield return new WaitForSeconds(i < 8 ? 0.33f : i < 13 ? 0.165f : 0.0825f);
			}
			Sfx.Play("entity.guy.death");

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
			var target = Direction == SpriteDirection.Left ? leftTarget : rightTarget;

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
					Direction = Direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;
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
					Direction = Direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;
					yield break;
				}
				// 判定
				HandleAttackToThePlayer(1);
				yield return null;
			}
			SetAnimations(AnimationStaying, AnimationWalking, AnimationJumping);
			// 反対を向く
			Direction = Direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;

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
			yield return DoPunch(Health <= 6 ? 4 : 3);
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
				bullet.Move(Direction == SpriteDirection.Left ? -bulletSpeed : bulletSpeed);
				SetAnimations(AnimationStaying);
				yield return new WaitForSeconds(Health > 6 ? wait : wait * .7f);
			}
		}

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

			// 50% shoot lightning ; 50% simply move
			yield return Random.value < 0.5f ? SummonLightning() : Move();

			var probability = Random.Range(0, 100);

			// in normal end:
			// 24% ball
			// 16% snake
			// 50% rock
			// 18% heal
			// in true end:
			// 50% ball
			// 20% heal
			// 10% rock

			if (Flag.Flags[flagOfTrueEnd])
			{
				if (probability < 50)
					yield return ThrowBall();
				else if (probability < 50 + 20)
					yield return ThrowHeart();
				else
					yield return ThrowRock();
			}
			else
			{
				if (probability < 24)
				{
					yield return ThrowBall();
				}
				else if (probability < 24 + 16)
				{
					yield return ThrowEnemy();
				}
				else if (probability < 24 + 16 + 50)
				{
					yield return ThrowRock();
				}
				else
				{
					yield return ThrowHeart();
				}
			}

			var time = 0f;
			while (time < 1.5f)
			{
				time += Time.deltaTime;
				if (isDoingEvent) yield break;
				yield return null;
			}

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

			yield return Lerp(destination, 0.6f);

			// 体力増強
			Wyte.Player.Life = Wyte.Player.MaxLife;

			Sfx.Play("entity.player.heal");

			// UFOが降りてくる
			Wyte.CanMove = true;
			SetAnimations(AnimationUfo);
			targetHeightToShootLightning += 32;
			destination = new Vector2(rightTarget, targetToDown);

			yield return EaseOut(destination, 2f);
			Direction = SpriteDirection.Left;
		}

		IEnumerator Move()
		{
			// 少し下よりの中央に移動し
			yield return EaseOut(new Vector2((leftTarget + rightTarget) / 2, targetToDown - 18), !Hard ? 0.7f : 0.5f);
			if (Dying) yield break;
			// 目的地に行く
			yield return EaseOut(new Vector2(Direction == SpriteDirection.Left ? leftTarget : rightTarget, targetToDown), 0.5f);

			if (isDoingEvent) yield break;
			yield return new WaitForSeconds(!Hard ? 0.4f : 0.2f);

			// 振り向く
			Direction = Direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;
		}

		IEnumerator ThrowIce()
		{
			if (isDoingEvent) yield break;
			Sfx.Play("entity.guy.throw");
			Instantiate(ice, transform.position, transform.rotation);
			yield break;
		}

		IEnumerator ThrowRock()
		{
			if (isDoingEvent) yield break;
			Sfx.Play("entity.guy.throw");
			Instantiate(rock, transform.position, transform.rotation);

			if (Hard)
			{
				for (int i = 0; i < 3; i++)
				{
					if (isDoingEvent) yield break;
					yield return new WaitForSeconds(0.1f);
					Sfx.Play("entity.guy.throw");
					Instantiate(rock, transform.position, transform.rotation);
				}
			}
		}

		IEnumerator SummonLightning()
		{
			if (isDoingEvent) yield break;
			var unit = (rightTarget - leftTarget) / 4;
			var middleLeft = new Vector2((leftTarget + rightTarget - unit) / 2, transform.position.y);
			var middleRight = new Vector2((leftTarget + rightTarget + unit) / 2, transform.position.y);
			var leftU = new Vector2(leftTarget, transform.position.y);
			var rightU = new Vector2(rightTarget, transform.position.y);

			Vector2[] navs =
				Direction == SpriteDirection.Left ? new[] { middleRight, middleLeft, leftU } : new[] { middleLeft, middleRight, rightU };

			yield return Charge();

			Instantiate(lightning, transform.position + Vector3.down * 32, default(Quaternion));
			for (int i = 0; i < navs.Length; i++)
			{
				if (isDoingEvent) yield break;
				yield return EaseOut(navs[i], Hard ? 0.2f : 0.4f);
				yield return Charge();
				Instantiate(lightning, new Vector2(transform.position.x, targetHeightToShootLightning), default(Quaternion));
			}

			yield return new WaitForSeconds(0.8f);

			Direction = Direction == SpriteDirection.Left ? SpriteDirection.Right : SpriteDirection.Left;
		}

		IEnumerator Charge()
		{
			Sfx.Play("entity.lightning.fall");
			foreach (var x in new[] { 2, -2, 2, -2 })
			{
				transform.Translate(x, 0, 0);
				yield return new WaitForSeconds(0.08f);
			}
			yield return new WaitForSeconds(0.1f);
		}

		IEnumerator ThrowEnemy()
		{
			if (isDoingEvent) yield break;
			Sfx.Play("entity.guy.throw");
			Instantiate(entityToThrow, transform.position, transform.rotation);
			yield break;
		}

		IEnumerator ThrowHeart()
		{
			if (isDoingEvent) yield break;
			Sfx.Play("entity.guy.throw");
			Instantiate(HealItem, transform.position, transform.rotation);
			yield break;
		}

		IEnumerator ThrowBall()
		{
			if (isDoingEvent) yield break;
			Sfx.Play("entity.guy.throw");
			var ball = Instantiate(BallToShoot, transform.position, transform.rotation).GetComponent<BallEntity>();
			ball.Parent = this;
			yield break;
		}

		#region Helper

		protected bool Hard => Health <= 2;

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
}