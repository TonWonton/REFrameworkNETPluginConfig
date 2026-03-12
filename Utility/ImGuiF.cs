#nullable enable
using System;
using System.Collections.Generic;
using Hexa.NET.ImGui;


namespace REFrameworkNETPluginConfig.Utility
{
	internal static class EnumValueAndNames<T> where T : struct, Enum
	{
		public static readonly T[] values = (T[])Enum.GetValues(typeof(T));
		public static readonly string[] names = Array.ConvertAll(values, value => value.ToString());
	}

	internal static class ImGuiF
	{
		//Variables
		private static float _indentSize = 24f;
		public static float IndentSize { get { return _indentSize; } set { _indentSize = value; } }
		private static float _resetTextSize = 0f;
		public static float ResetTextSize { get { return _resetTextSize != 0f ? _resetTextSize : _resetTextSize = ImGui.CalcTextSize("Reset").X; } }

		//Config
		public static ConfigEntry<T> GetValue<T>(this ConfigEntry<T> configEntry, out T value) { value = configEntry.Value; return configEntry; }

		//ImGui
		private static Dictionary<int, string> _resetButtonLabels = new Dictionary<int, string>();
		private static string GetResetButtonLabel(ref int labelNr)
		{
			if (_resetButtonLabels.TryGetValue(labelNr, out string? label) == false)
			{
				label = "Reset##" + labelNr;
				_resetButtonLabels[labelNr] = label;
			}

			labelNr++;
			return label;
		}

		public static void Category(string text) { ImGui.NewLine(); ImGui.SeparatorText(text); }
		public static void SubCategory(string text) { ImGui.Indent(_indentSize); ImGui.Spacing(); ImGui.SeparatorText(text); }
		public static void EndSubCategory() { ImGui.Unindent(_indentSize); }

		public static ConfigEntry<T> Indent<T>(this ConfigEntry<T> configEntry, float indent) { ImGui.Indent(indent); return configEntry; }
		public static ConfigEntry<T> Unindent<T>(this ConfigEntry<T> configEntry, float indent) { ImGui.Unindent(indent); return configEntry; }
		public static ConfigEntry<T> Spacing<T>(this ConfigEntry<T> configEntry) { ImGui.Spacing(); return configEntry; }

		public static ConfigEntry<T> BeginDisabled<T>(this ConfigEntry<T> configEntry, bool condition) { ImGui.BeginDisabled(condition); return configEntry; }
		public static ConfigEntry<T> EndDisabled<T>(this ConfigEntry<T> configEntry) { ImGui.EndDisabled(); return configEntry; }

		public static ConfigEntry<T> Combo<T>(this ConfigEntry<T> configEntry) where T : struct, Enum
		{
			T[] values = EnumValueAndNames<T>.values;
			string[] names = EnumValueAndNames<T>.names;
			int currentIndex = Array.IndexOf(values, configEntry.Value);

			if (ImGui.Combo(configEntry.Key, ref currentIndex, names, names.Length)) configEntry.Set(values[currentIndex]);

			return configEntry;
		}

		public static ConfigEntry<bool> Checkbox(this ConfigEntry<bool> configEntry)
		{
			if (ImGui.Checkbox(configEntry.Key, ref configEntry.RefValue)) configEntry.NotifyValueChanged();
			return configEntry;
		}

		public static ConfigEntry<float> DragFloat(this ConfigEntry<float> configEntry, float vSpeed, float vMin, float vMax)
		{
			if (ImGui.DragFloat(configEntry.Key, ref configEntry.RefValue, vSpeed, vMin, vMax)) configEntry.NotifyValueChanged();
			return configEntry;
		}

		public static ConfigEntry<T> ResetButton<T>(this ConfigEntry<T> configEntry, ref int labelNr)
		{
			float buttonWidth = ResetTextSize + ImGui.GetStyle().FramePadding.X * 2;

			ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonWidth);
			if (ImGui.Button(GetResetButtonLabel(ref labelNr))) configEntry.Reset();

			return configEntry;
		}
	}
}