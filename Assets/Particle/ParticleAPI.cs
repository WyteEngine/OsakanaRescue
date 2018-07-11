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

		public void Summon(string id, Vector2 pos)
		{
			// 生成する
			Instantiate(this[id], pos, transform.rotation);
		}

		public void Summon(string id, float x, float y)
		{
			Summon(id, new Vector2(x, y));
		}

		public IEnumerator Summon(string s, params string[] a)
		{
			string entity;
			float x, y;
			switch (a.Length)
			{
				case 2:
					// SpriteTagをパーティクルIDとする
					entity = s;
					NArgsAssert(float.TryParse(a[0], out x));
					NArgsAssert(float.TryParse(a[1], out y));
					break;
				case 3:
					// 3引数をID，x, yとする
					entity = a[0];
					NArgsAssert(float.TryParse(a[1], out x));
					NArgsAssert(float.TryParse(a[2], out y));
					break;
				default:
					// デフォルトの引数エラーを返す
					NArgsAssert(false);
					yield break;
			}

			Summon(entity, x, y);
		}

	}

	[System.Serializable]
	public struct EntityKeyValue
	{
		public string Id;
		public Entity Entity;
	}
}