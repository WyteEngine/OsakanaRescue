using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WyteEngine;
using WyteEngine.Inputing;
using WyteEngine.UI;

namespace Xeltica.Osakana
{
	public class KeyConfig : BaseBehaviour
	{
		[SerializeField]
		Text left;
		[SerializeField]
		Text right;
		[SerializeField]
		Text dash;
		[SerializeField]
		Text jump;
		[SerializeField]
		Text action;
		[SerializeField]
		Text menu;

		string l, r, d, j, a, m;

		int state = 0;
		bool stateChanged;
		bool session = false;
		string escape = "";
		// Use this for initialization
		void Start()
		{

		}

		private void OnEnable()
		{
			l = KeyBind.Left;
			r = KeyBind.Right;
			d = KeyBind.Dash;
			j = KeyBind.Jump;
			a = KeyBind.Up;
			m = KeyBind.Pause;
			// 一度エスケープキーを未指定にしておく(設定中に押されると死ぬので)
			var b = KeyBinding.Instance.Binding;
			escape = b.Pause;
			KeyBinding.Instance.Binding = new Keys(b.Left, b.Right, b.Up, b.Down, b.Jump, b.Action, b.Dash, "", b.Menu, b.ExLeft, b.ExRight, b.ForceTouch);
			state = 0;
			session = true;
		}

		private void OnDisable()
		{
			// エスケープキーを戻す
			var b = KeyBinding.Instance.Binding;
			KeyBinding.Instance.Binding = new Keys(b.Left, b.Right, b.Up, b.Down, b.Jump, b.Action, b.Dash, escape, b.Menu, b.ExLeft, b.ExRight, b.ForceTouch);

		}

		// Update is called once per frame
		void Update()
		{
			var key = ((KeyCode[])System.Enum.GetValues(typeof(KeyCode)))
				.Where(k => !k.ToString().StartsWith("Mouse"))
				.Where(Input.GetKeyDown);
			if (key.Any())
			{
				var k = key.First();
				switch (state)
				{
					case 0:
						l = k.toName();
						break;
					case 1:
						r = k.toName();
						break;
					case 2:
						d = k.toName();
						break;
					case 3:
						j = k.toName();
						break;
					case 4:
						a = k.toName();
						break;
					case 5:
						m = k.toName();
						break;
				}
				state++;
				if (state >= 6)
				{
					KeyBinding.Instance.Binding = new Keys(l, r, a, "down", j, "x", d, m, "a", "q", "w", false);
					escape = m;
					ConfigController.Instance.Back();
					session = false;
				}
			}

			left.text = string.Format(I18n["system.config.key.left"], l);
			left.color = state == 0 ? Color.yellow : Color.white;
			right.text = string.Format(I18n["system.config.key.right"], r);
			right.color = state == 1 ? Color.yellow : Color.white;
			dash.text = string.Format(I18n["system.config.key.dash"], d);
			dash.color = state == 2 ? Color.yellow : Color.white;
			jump.text = string.Format(I18n["system.config.key.jump"], j);
			jump.color = state == 3 ? Color.yellow : Color.white;
			action.text = string.Format(I18n["system.config.key.talk"], a);
			action.color = state == 4 ? Color.yellow : Color.white;
			menu.text = string.Format(I18n["system.config.key.menu"], m);
			menu.color = state == 5 ? Color.yellow : Color.white;

		}
	}
}