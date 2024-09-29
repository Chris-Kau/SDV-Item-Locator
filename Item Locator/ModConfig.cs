using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections;

public sealed class ModConfig
{
    public SButton openMenuKey { get; set; }
    public List<string>? locateHistory { get; set; }


    public ModConfig()
    {
        this.openMenuKey = SButton.O;
        this.locateHistory = new() { "None", "None", "None", "None", "None"};
    }
}