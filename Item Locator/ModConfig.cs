using StardewModdingAPI;
using StardewModdingAPI.Utilities;

public sealed class ModConfig
{
    public SButton openMenuKey { get; set; }

    public ModConfig()
    {
        this.openMenuKey = SButton.O;
    }
}