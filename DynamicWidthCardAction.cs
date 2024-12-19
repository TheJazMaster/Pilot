﻿using HarmonyLib;
using System;
using System.Reflection;

namespace TheJazMaster.Pilot;

[HarmonyPatch]
public  class DynamicWidthCardAction : CardAction
{
	internal static ModEntry Instance => ModEntry.Instance;

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Card), nameof(Card.RenderAction))]
	private static bool Card_RenderAction_Prefix(G g, State state, CardAction action, bool dontDraw, ref int __result)
	{
		if (action is not DynamicWidthCardAction)
			return true;
		if (action.GetIcon(state) is not { } icon)
			return true;
		if (SpriteLoader.Get(icon.path) is not { } texture)
			return true;

		var box = g.Push();
		__result = 0;

		if (!dontDraw)
			Draw.Sprite(icon.path, box.rect.x + __result, box.rect.y, flipY: icon.flipY);
		__result += texture.Width;

		if (icon.number is { } amount)
		{
			__result += 1;

			if (!dontDraw)
				BigNumbers.Render(amount, box.rect.x + __result, box.rect.y, action.disabled ? Colors.disabledText : icon.color);
			__result += GetCharacterCount(amount) * 6;
		}

		g.Pop();

		return false;
	}

	private static int GetCharacterCount(int number)
	{
		if (number == 0)
			return 1;
		return (int)Math.Log10(Math.Abs(number)) + 1;
	}
}
