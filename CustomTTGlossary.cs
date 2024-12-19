using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheJazMaster.Pilot;

[HarmonyPatch]
internal sealed class CustomTTGlossary : TTGlossary
{
	public enum GlossaryType
	{
		midrow, status, cardtrait, action, parttrait, destination, actionMisc, part, env
	}

	private static ModEntry Instance => ModEntry.Instance;

	private static readonly Stack<TTGlossary> ContextStack = new();

	private static int NextID = 0;
	private readonly GlossaryType Type;
	private readonly Func<Spr?> Icon;
	private readonly Func<string> Title;
	private readonly Func<string> Description;
	private readonly IReadOnlyList<Func<object>> Values;

	public CustomTTGlossary(GlossaryType type, Func<string> title, Func<string> description, IEnumerable<Func<object>>? values = null, string? key = null) : this(type, () => null, title, description, values, key) { }

	public CustomTTGlossary(GlossaryType type, Func<Spr?> icon, Func<string> title, Func<string> description, IEnumerable<Func<object>>? values = null, string? key = null) : base($"{Enum.GetName(type)}.customttglossary.{key ?? $"{NextID++}"}")
	{
		Type = type;
		Icon = icon;
		Title = title;
		Description = description;
		Values = values?.ToList() ?? (IReadOnlyList<Func<object>>)Array.Empty<Func<object>>();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TTGlossary), nameof(BuildIconAndText))]
	private static void TTGlossary_BuildIconAndText_Prefix(TTGlossary __instance)
		=> ContextStack.Push(__instance);

	[HarmonyFinalizer]
	[HarmonyPatch(typeof(TTGlossary), nameof(BuildIconAndText))]
	private static void TTGlossary_BuildIconAndText_Finalizer()
		=> ContextStack.Pop();

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TTGlossary), nameof(TryGetIcon))]
	private static bool TTGlossary_TryGetIcon_Prefix(ref Spr? __result)
	{
		if (!ContextStack.TryPeek(out var glossary) || glossary is not CustomTTGlossary custom)
			return true;

		__result = custom.Icon();
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TTGlossary), nameof(MakeNameDescPair))]
	private static bool TTGlossary_MakeNameDescPair_Prefix(string nameColor, ref string __result)
	{
		if (!ContextStack.TryPeek(out var glossary)) return true;

		if (glossary is not CustomTTGlossary custom)
			return true;

		var title = custom.Title();
		__result = $"{(string.IsNullOrEmpty(title) ? "" : $"<c={nameColor}>{custom.Title().ToUpper()}</c>\n")}{BuildString(custom.Description(), custom.Values.Select(v => v()).ToArray())}";
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TTGlossary), nameof(BuildString))]
	private static bool TTGlossary_BuildString_Prefix(ref string __result)
	{
		if (!ContextStack.TryPeek(out var glossary) || glossary is not CustomTTGlossary custom)
			return true;

		object[] args = custom.Values.Select(v => "<c=boldPink>{0}</c>".FF(v().ToString() ?? "")).ToArray();
		__result = string.Format(custom.Description(), args);
		return false;
	}
}
