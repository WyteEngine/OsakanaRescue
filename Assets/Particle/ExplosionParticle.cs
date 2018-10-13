using WyteEngine.Entities;
using UnityEngine;

namespace Xeltica.Osakana
{
	/// <summary>
	/// 小規模の爆発パーティクルです．
	/// </summary>
	public class ExplosionParticle : SpriteEntity
	{
		protected override void Start()
		{
			base.Start();
			ChangeSprite("particle.explosion");
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			transform.Translate(Vector2.up * 4 * Time.deltaTime);

			if (!Camera.IsVisible(transform.position))
				Kill(this);

			//アニメーションが終わったらkill
			if (!IsAnimating)
				Kill(this);

		}
	}

}