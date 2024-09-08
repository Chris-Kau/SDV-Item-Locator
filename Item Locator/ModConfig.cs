using StardewModdingAPI;
using StardewModdingAPI.Utilities;

public sealed class ModConfig
{
    public SButton openMenuKey { get; set; }

    public List<String> locateHistory { get; set; }

    public ModConfig()
    {
        this.locateHistory = new();
        this.openMenuKey = SButton.O;
    }
}