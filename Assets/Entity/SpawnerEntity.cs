using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WyteEngine.Entities;

namespace Xeltica.Osakana
{
	/// <summary>
	/// 他の Entity をクローンする Entityです．
	/// </summary>
	public class SpawnerEntity : Entity
	{
		[SerializeField]
		private Entity entityToSummon;

		/// <summary>
		/// 召喚する Entity を取得または設定します．
		/// </summary>
		/// <value>The entity to summon.</value>
		public Entity EntityToSummon
		{
			get { return entityToSummon; }
			set { entityToSummon = value; }
		}

		[SerializeField]
		private float timeSpan = 1;

		/// <summary>
		/// 召喚する時間間隔を取得または設定します．
		/// </summary>
		/// <value>The time span.</value>
		public float TimeSpan
		{
			get { return timeSpan; }
			set { timeSpan = value; }
		}

		[SerializeField]
		private int count;

		/// <summary>
		/// 召喚する回数を取得または設定します．0で無限回になります．
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get { return count; }
			set { count = value; }
		}

		float timer;
		int counter;

		protected override void OnUpdate()
		{
			base.OnUpdate();
			// 無限ループでなくて，カウントが終わったら帰るよ
			if (counter >= Count && Count != 0)
				return;
			
			// 時間が来たよ
			if (timer >= TimeSpan)
			{
				// クローンをつくるよ
				Instantiate(entityToSummon, transform.position, transform.rotation);
				// タイマーを初期化するよ
				timer = 0;
				// カウントするよ
				counter++;
			}
			// 時を刻むよ
			timer += Time.deltaTime;
		}
	}
}