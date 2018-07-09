using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WyteEngine;
using WyteEngine.Entities;
using System.Linq;

namespace Xeltica.Osakana
{
	public class ParticleAPI : SingletonBaseBehaviour<ParticleAPI>
	{
		[SerializeField]
		private EntityKeyValue[] particles;

		public EntityKeyValue[] Particles
		{
			get { return particles; }
			set { particles = value; }
		}

		public Entity this[string id]
		{
			get
			{
				var e = particles.FirstOrDefault(p => p.Id == id).Entity;
				if (e == null) Debug.LogWarning($"Particle ID:{id} is not found.");
				return e;
			}
	}

		void Start()
		{
			Novel.Runtime.Register("particle", Summon);
		}

		public IEnumerator Summon(string s, params string[] a)
		{
			Entity entity;
			float x, y;
			switch (a.Length)
			{
				case 2:
					// SpriteTagをパーティクルIDとする
					entity = this[s];
					NArgsAssert(float.TryParse(a[0], out x));
					NArgsAssert(float.TryParse(a[1], out y));
					break;
				case 3:
					// 3引数をID，x, yとする
					entity = this[a[0]];
					NArgsAssert(float.TryParse(a[1], out x));
					NArgsAssert(float.TryParse(a[2], out y));
					break;
				default:
					// デフォルトの引数エラーを返す
					NArgsAssert(false);
					yield break;
			}
			// 生成する
			Instantiate(entity, new Vector2(x, y), transform.rotation);
		}

	}

	[System.Serializable]
	public struct EntityKeyValue
	{
		public string Id;
		public Entity Entity;
	}
}