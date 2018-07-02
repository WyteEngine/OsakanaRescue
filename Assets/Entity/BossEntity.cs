using System.Collections;
using UnityEngine;
using WyteEngine.Entities;
using WyteEngine.Helper;

public abstract class BossEntity : LivableEntity
{
	public override string LandSfxId => null;
	public override string JumpSfxId => null;
	public override string DeathSfxId => null;

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

	bool battleStarted;

	/// <summary>
	/// 戦闘が始まるためのプレイヤーとの距離．
	/// </summary>
	/// <value>The distance to begin.</value>
	public float DistanceToBegin
	{
		get { return distanceToBegin; }
		set { distanceToBegin = value; }
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		// プレイヤーが然るべき距離に入ってきたら試合開始

		if (Wyte.CurrentPlayer != null && Mathf.Abs(transform.position.x - Wyte.CurrentPlayer.transform.position.x) < DistanceToBegin && !battleStarted)
		{
			direction = SpriteDirection.Left;
			StartCoroutine(BeginBattle());
		}
	}

	protected virtual IEnumerator BeginBattle()
	{
		battleStarted = true;
		// 戦闘開始前のイベント実行(もし存在すれば)
		if (!string.IsNullOrEmpty(PreEvent))
		{
			Novel.Run(PreEvent);
			yield return new WaitWhile(() => Novel.Runtime.IsRunning);
		}

		yield return OnBattle();
	}

	protected abstract IEnumerator OnBattle();

	protected IEnumerator Lerp(Vector2 dest, float time)
	{
		var t = 0f;
		//初期値
		var p0 = transform.position;
		while (t < time)
		{
			transform.position = Vector2.Lerp(p0, dest, t / time);
			t += Time.deltaTime;
			yield return null;
		}
		// 念の為
		transform.position = dest;
	}

	protected IEnumerator EaseIn(Vector2 dest, float time)
	{
		var t = 0f;
		//初期値
		var p0 = transform.position;
		while (t < time)
		{
			transform.position = MathHelper.EaseIn(t / time, p0, dest);
			t += Time.deltaTime;
			yield return null;
		}
		// 念の為
		transform.position = dest;
	}

	protected IEnumerator EaseOut(Vector2 dest, float time)
	{
		var t = 0f;
		//初期値
		var p0 = transform.position;
		while (t < time)
		{
			transform.position = MathHelper.EaseOut(t / time, p0, dest);
			t += Time.deltaTime;
			yield return null;
		}
		// 念の為
		transform.position = dest;
	}
}
