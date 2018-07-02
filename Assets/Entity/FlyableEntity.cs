using UnityEngine;
using WyteEngine.Entities;

public abstract class FlyableEntity : LivableEntity
{
	public override string WalkAnimationId => FlyAnimationId;

	public override string StayAnimationId => State == FlyableEntityState.Fly ? FlyAnimationId : WaitingAnimationId;

	public override string JumpAnimationId => null;

	public override string LandSfxId => null;

	public override string JumpSfxId => null;

	/// <summary>
	/// 飛行中のアニメーションを取得します．
	/// </summary>
	/// <returns></returns>
	public abstract string FlyAnimationId { get; }

	public abstract string WaitingAnimationId { get; }

	/// <summary>
	/// このEntityの現在の飛行状態を取得または設定します．このプロパティに応じて，適切なアニメーションが行われます．
	/// </summary>
	/// <returns></returns>
	public FlyableEntityState State { get; set; }

	public enum FlyableEntityState
	{
		/// <summary>
		/// 飛行中．
		/// </summary>
		Fly,
		/// <summary>
		/// 待機中．
		/// </summary>
		Stay
	}

	/// <summary>
	/// EntityLiving から継承されるメソッドです．単純に，速度ベクトルのX座標を指定します．
	/// </summary>
	/// <param name="rightSpeed">速度ベクトルのX座標を指定します．</param>
	/// <param name="hold">意味を持ちません．</param>
	public override void Move(float rightSpeed, bool hold = true)
	{
		Move(new Vector2(rightSpeed, 0));
	}

	/// <summary>
	/// 指定した値を速度として移動します．
	/// </summary>
	/// <param name="speed"></param>
	public void Move(Vector2 speed)
	{
		Velocity = speed;
		direction = (int)speed.x < 0 ? SpriteDirection.Left : (int)speed.x > 0 ? SpriteDirection.Right : direction;
		if (speed != Vector2.zero)
			State = FlyableEntityState.Fly;
	}

	/// <summary>
	/// その場で停止します．
	/// </summary>
	public void Stop()
	{
		State = FlyableEntityState.Stay;
		// とまる
		Move(Vector2.zero);
	}

	// 飛行可能なので重力影響はない
	public override float GravityScale => Dying ? base.GravityScale : 0;
}