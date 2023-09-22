using MonoMod;

namespace Modding.Patches;

#pragma warning disable CS1591

[MonoModPatch("global::ShopItemStats")]
public class ShopItemStats : global::ShopItemStats
{
    [MonoModIgnore]
    private PlayerData playerData;

    [MonoModIgnore]
    private int notchCost;

    [MonoModReplace]
    private void Start()
    {
        try
        {
            this.cost = int.Parse(Language.Get(this.priceConvo, "Prices"));
        }
        catch { }
        if (this.specialType == 2)
        {
            this.playerData = PlayerData.instance;
            this.notchCost = this.playerData.GetInt(this.notchCostBool);
        }
    }
}
