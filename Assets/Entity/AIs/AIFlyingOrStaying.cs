using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WyteEngine.Entities.AI;
using WyteEngine.Helper;

namespace Xeltica.Osakana
{
	namespace WyteEngine.Entities.AI
	{
		public class AIFlyingOrStaying : AIBaseBehaviour
		{
			enum State
			{
				Stay,
				Track,
				ReturnBack
			}

			[SerializeField]
			private float speed;

			public float Speed
			{
				get { return speed; }
				set { speed = value; }
			}


			const float DistanceToTrack = 91;
			const float DistanceToUntrack = 136;

			State state = State.Stay;

			protected override void OnInitialize()
			{
				OnUpdate = new ActionNode(Do);
			}

			void Do(Entity e)
			{
				if (!(e is FlyableEntity) || Wyte.CurrentPlayer == null)
					return;
				var fe = e as FlyableEntity;
				var fp = fe.transform.position;
				var player = Wyte.CurrentPlayer.transform.position;
				switch (state)
				{
					case State.Stay:
						fe.Stop();

						// 近づいたらトラッキングする
						if (Vector3.Distance(fp, player) < DistanceToTrack)
							state = State.Track;
						break;
					case State.Track:
						fe.Move(fe.transform.position.GetTargetingVelocity(player, Speed));

						// 遠ざかったらトラッキングをやめる
						if (Vector3.Distance(fp, player) > DistanceToUntrack)
							state = State.ReturnBack;

						break;
					case State.ReturnBack:
						// 頭をぶつけたらぶらさがる
						fe.Move(Vector2.up * Speed);
						if (fe.IsCeiling())
							state = State.Stay;
						break;
				}
			}
		}
	}
}