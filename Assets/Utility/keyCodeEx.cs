using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public static class KeyCodeUtil
{
	private static readonly Dictionary<KeyCode, string> specifiedNames = new Dictionary<KeyCode, string> {
		{ KeyCode.Exclaim, "!" },
		{ KeyCode.DoubleQuote, "\"" },
		{ KeyCode.Hash, "#" },
		{ KeyCode.Dollar, "$" },
		{ KeyCode.Ampersand, "&" },
		{ KeyCode.Quote, "'" },
		{ KeyCode.LeftParen, "(" },
		{ KeyCode.RightParen, ")" },
		{ KeyCode.Asterisk, "*" },
		{ KeyCode.Plus, "+" },
		{ KeyCode.Comma, "," },
		{ KeyCode.Minus, "-" },
		{ KeyCode.Period, "." },
		{ KeyCode.Slash, "/" },
		{ KeyCode.Colon, ":" },
		{ KeyCode.Semicolon, ";" },
		{ KeyCode.Less, "<" },
		{ KeyCode.Equals, "=" },
		{ KeyCode.Greater, ">" },
		{ KeyCode.Question, "?" },
		{ KeyCode.At, "@" },
		{ KeyCode.LeftBracket, "[" },
		{ KeyCode.Backslash, "\\" },
		{ KeyCode.RightBracket, "]" },
		{ KeyCode.Caret, "^" },
		{ KeyCode.Underscore, "_" },
		{ KeyCode.BackQuote, "`" },
		{ KeyCode.KeypadPeriod, "[.]" },
		{ KeyCode.KeypadDivide, "[/]" },
		{ KeyCode.KeypadMultiply, "[*]" },
		{ KeyCode.KeypadMinus, "[-]" },
		{ KeyCode.KeypadPlus, "[+]" },
		{ KeyCode.KeypadEnter, "enter" },
		{ KeyCode.KeypadEquals, "equals" },
		{ KeyCode.Print, "print screen" },
	};

	private static readonly HashSet<string> ignoredNames = new HashSet<string> {
		"LeftApple",
		"RightApple",
	};

	private static readonly HashSet<KeyCode> ignoredCodes = new HashSet<KeyCode> {
		KeyCode.None,
	};

	public static KeyCode toCode(string name)
	{
		KeyCode code;
		if (_str2code.TryGetValue(name, out code))
		{
			return code;
		}
		return KeyCode.None;
	}

	public static string toName(this KeyCode code)
	{
		string name;
		if (_code2str.TryGetValue(code, out name))
		{
			return name;
		}
		return "";
	}

	public static KeyCode mouse(int button)
	{
		if (button < 0 || button >= 7) return KeyCode.None;

		var code = KeyCode.Mouse0 + button;
		return code;
	}

	public static KeyCode joystick(int button)
	{
		if (button < 0 || button >= 20) return KeyCode.None;

		var code = KeyCode.JoystickButton0 + button;
		return code;
	}

	public static KeyCode joystick(int num, int button)
	{
		if (button < 0 || button >= 20) return KeyCode.None;

		var code = KeyCode.None;
		switch (num)
		{
			case 1:
				code = KeyCode.Joystick1Button0;
				break;
			case 2:
				code = KeyCode.Joystick2Button0;
				break;
			case 3:
				code = KeyCode.Joystick3Button0;
				break;
			case 4:
				code = KeyCode.Joystick4Button0;
				break;
			default:
				return KeyCode.None;
		}

		code += button;
		return code;
	}

	static KeyCodeUtil()
	{
		foreach (var pair in specifiedNames)
		{
			assign(pair.Key, pair.Value);
		}

		foreach (var name in Enum.GetNames(typeof(KeyCode)))
		{
			assign(name);
		}
	}

	private static void assign(string name)
	{
		if (ignoredNames.Contains(name)) return;

		var code = (KeyCode)Enum.Parse(typeof(KeyCode), name);
		if (ignoredCodes.Contains(code)) return;

		if (_code2str.ContainsKey(code)) return;

		var parts = splitName(name);

		switch (parts[0])
		{
			case "alpha":
				assign(code, parts[1]);
				return;
			case "f":
				assign(code, name.ToLowerInvariant());
				return;
			case "keypad":
				parts = parts.Take(1).ToList();
				name = $"[{string.Join(" ", parts.ToArray())}]";
				assign(code, name);
				return;
		}

		switch (parts.Last())
		{
			case "arrow":
				parts.RemoveAt(parts.Count - 1);
				break;
			case "control":
				parts[parts.Count - 1] = "ctrl";
				break;
			case "command":
				parts[parts.Count - 1] = "cmd";
				break;
			case "super":
				parts[parts.Count - 1] = "super";
				break;
		}

		assign(code, string.Join(" ", parts.ToArray()));
	}

	private static void assign(KeyCode code, string name)
	{
		_str2code[name] = code;
		_code2str[code] = name;
	}

	private static List<string> splitName(string name)
	{
		var parts = name.splitCamelCase();
		for (var i = 0; i < parts.Count; ++i)
		{
			parts[i] = parts[i].ToLowerInvariant();
		}
		return parts;
	}

	private static readonly Dictionary<string, KeyCode> _str2code = new Dictionary<string, KeyCode>();
	private static readonly Dictionary<KeyCode, string> _code2str = new Dictionary<KeyCode, string>();
}

public static class CharExtension
{
    public static bool isNumber(this char c)
    {
        return '0' <= c && c <= '9';
    }

    public static bool isAlphabet(this char c)
    {
        return isLowerAlphabet(c) || isUpperAlphabet(c);
    }
    public static bool isLowerAlphabet(this char c)
    {
        return 'a' <= c && c <= 'z';
    }
    public static bool isUpperAlphabet(this char c)
    {
        return 'A' <= c && c <= 'Z';
    }
}

public static class StringExtension
{
    private enum IdCharType
    {
        Invalid,
        LowerAlphabet,
        UpperAlphabet,
        Number,
        Underscore,
    }

    public static List<string> splitCamelCase(this string str)
    {
        var length = str.Length;
        var texts = new List<string>();

        var last = IdCharType.Invalid;
        var start = 0;

        bool upperSequence = false;

        for(var i = 0; i < length; ++i) {
            var c = str[i];
            var current = c.toCharType();

            Assert.IsTrue(current != IdCharType.Invalid, "Invalid character.");

            if(i == 0) {
                last = current;
                continue;
            }

            if(last == IdCharType.UpperAlphabet) {
                if(current == IdCharType.UpperAlphabet) {
                    upperSequence = true;
                    continue;
                }
                if(upperSequence) {
                    upperSequence = false;
                    if(current == IdCharType.Underscore) {
                        texts.Add(str.Substring(start, i - start));
                        start = i;
                    }
                    else {
                        texts.Add(str.Substring(start, i - start - 1));
                        start = i - 1;
                    }
                    last = current;
                    continue;
                }
                if(current == IdCharType.LowerAlphabet) {
                    last = current;
                    continue;
                }
            }
            upperSequence = false;

            if(last == IdCharType.Underscore) {
                start = i;
                last = current;
                continue;
            }

            if(last != current) {
                texts.Add(str.Substring(start, i - start));
                start = i;
            }

            last = current;
        }

        if(start < length && last != IdCharType.Underscore) {
            texts.Add(str.Substring(start, length - start));
        }

        return texts;
    }

    private static IdCharType toCharType(this char c)
    {
        if(c.isLowerAlphabet()) return IdCharType.LowerAlphabet;
        if(c.isUpperAlphabet()) return IdCharType.UpperAlphabet;
        if(c.isNumber()) return IdCharType.Number;
        if(c == '_') return IdCharType.Underscore;
        return IdCharType.Invalid;
    }
}