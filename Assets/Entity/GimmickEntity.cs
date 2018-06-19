using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class GimmickEntity : LivableEntity
{

	[Header("Event")]
	[SerializeField]
	[Tooltip("アクション時に実行するイベント ラベル。")]
	string label;
	public string Label
	{
		get { return label; }
		set { label = value; }
	}

	[SerializeField]
	[Tooltip("アクションの対象となるスプライトID．")]
	string targetId;
	public string TargetId
	{
		get { return targetId; }
		set { targetId = value; }
	}

	[Header("Animation Id")]
	[SerializeField]
	string stayAnimId;
	[SerializeField]
	string jumpAnimId;
	[SerializeField]
	string walkAnimId;
	[SerializeField]
	string killedAnimId;
	[SerializeField]
	string steppedAnimId;

	[Header("Sound FX Id")]
	[SerializeField]
	string landSfxId;
	[SerializeField]
	string jumpSfxId;
	[SerializeField]
	string deathSfxId;

	[Header("Entity Setting")]
	[SerializeField]
	float gravityScaleMultiplier = 1;

	public float GravityScaleMultiplier
	{
		get { return gravityScaleMultiplier; }
		set { gravityScaleMultiplier = value; Velocity = new Vector2(Velocity.x, 0); }
	}


	public override string WalkAnimationId => walkAnimId;
	public override string StayAnimationId => stayAnimId;
	public override string JumpAnimationId => jumpAnimId;
	public override string LandSfxId => landSfxId;
	public override string JumpSfxId => jumpSfxId;
	public override string DeathSfxId => deathSfxId;

	public override float GravityScale => charaGravityScale * gravityScaleMultiplier;


	protected override void Start()
	{
		base.Start();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Entity entity = collision.gameObject.GetComponent<Entity>();
		Debug.Log($"Entity: {(entity != null ? entity.ToString() : "null")}");
		if (entity != null)
			HandleInteract(entity);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Entity entity = collision.gameObject.GetComponent<Entity>();
		if (entity != null)
			HandleInteract(entity);
	}

	private void HandleInteract(Entity entity)
	{
		Debug.Log($"{entity.Tag}, {TargetId}, {Label}");
		if (entity.Tag == TargetId && !(string.IsNullOrEmpty(Label)))
		{
			// すくりぷとをじっこうする！
			Novel.Run(Label);
		}
	}

	protected override IEnumerator OnDeath(Object killer)
	{
		if (killer == Wyte.CurrentPlayer)
		{
			// 踏まれた
			ChangeSprite(steppedAnimId);
			rigid.velocity = Velocity = Vector2.zero;
			yield return new WaitForSeconds(3);
		}
	}

	public override void ChangeSprite(string id)
	{
		base.ChangeSprite(id);

		walkAnimId = stayAnimId = jumpAnimId = id;
	}
}
