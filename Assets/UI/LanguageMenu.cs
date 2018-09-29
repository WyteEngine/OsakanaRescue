using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WyteEngine;
using WyteEngine.UI.TextFormatting;

namespace Xeltica.Osakana
{
	public class LanguageMenu : BaseBehaviour
	{
		[SerializeField]
		Button template;

		[SerializeField]
		Transform container;

		// Use this for initialization
		void Start()
		{
			if (container == null)
				container = transform;
			foreach (var locale in I18n.Locales)
			{
				AddButton(locale.Key, locale.Value["lang.name"], locale.Value["lang.name.inenglish"]);
			}
		}
		
		void AddButton(string langId, string langName, string langNameInEnglish)
		{
			var btn = Instantiate(template.gameObject, container);
			btn.SetActive(true);
			btn.GetComponent<Button>().onClick.AddListener(() => I18n.Language = langId);
			btn.GetComponentInChildren<Text>().text = TextComponentBuilder.Create().Text(langName).Text(" ").Color(Color.gray).Text("(").Text(langNameInEnglish).Text(")").Finish().ToString();
		}
	}
}