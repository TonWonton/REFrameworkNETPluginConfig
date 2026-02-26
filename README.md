# REFrameworkNETPluginConfig

## Description
REFrameworkNETPluginConfig config plugin/dependency/library for the REFramework C# API. Add config settings to a plugin that can be serialized to and loaded from JSON.
Can be used as a separate .dll dependency (placed in `\GAME_FOLDER\reframework\plugins\managed\dependencies\`) or embedded in a different plugin together in a single .dll file.

## Prerequisites
- REFramework and the REFramework C# API [GitHub REFramework-nightly releases](https://github.com/praydog/REFramework-nightly/releases)

## Features
- Add config entries easily
- Value changed event
- RefValue property for use with ImGui
- Directory and file path already set up (only need to provide file name)
  - Default is `\GAME_FOLDER\reframework\data\` + provided file name + .json
- Save or load in one call/line of code
- Atomic save
- Exception and error handling

## Usage
- Use as dependency or embed/bundle in plugin
  - Reference the .dll dependency (end user has to install the .dll dependency as well)
  - Reference the .cs source and bundle it in a single .dll (compile with constant `EMBEDDED_SOURCE`)

### Example usage
```csharp
namespace MHWilds_DisablePostProcessingEffects
{
	public class DisablePostProcessingEffectsPlugin
	{
		public const string GUID = "MHWilds_DisablePostProcessingEffects"; //File name

		//1. Create Config instance and pass the desired file name as a parameter to the constructor
		private static Config _config = new Config(GUID);

		//2. Declare config variables
		private static ConfigEntry<bool> _colorCorrect = null!;

		//3. Initialize config
		[PluginEntryPoint]
		protected static void Load()
		{
			_colorCorrect = _config.Add("Color correction", true); //Parameters are (string key, T defaultValue)
			_colorCorrect.ValueChanged += ApplySettings; //Register event if needed

			//4. Save to or load from JSON
			_config.LoadFromJson();
			_config.SaveToJson();
		}
	}
}
```

### Example ImGui usage
```csharp
[Callback(typeof(ImGuiDrawUI), CallbackType.Pre)]
public static void PreImGuiDrawUI()
{
	if (ImGui.TreeNode(GUID_AND_V_VERSION))
	{
		//Pass the config value as ref to ImGui and invoke event if value is changed (ImGui returns true)
		ConfigEntry<bool> colorCorrect = _colorCorrect;
		if (ImGui.Checkbox(colorCorrect.Key, ref colorCorrect.RefValue)) { colorCorrect.NotifyValueChanged(); }

		//Optional reset button
		ImGui.SameLine(); if (ImGui.Button("Reset##00")) { colorCorrect.Reset(); }

		//Pop TreeNode at the end
		ImGui.TreePop();
	}
}
```