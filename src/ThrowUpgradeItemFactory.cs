using UnityEngine;

namespace Unremove;

static class ThrowUpgradeItemFactory
{
    private static Item _item = null!;
    internal static Item Item
    {
        get
        {
            if (!_item) Instantiate();
            return _item;
        }
    }

    private static void Instantiate()
    {
        _item = ScriptableObject.CreateInstance<Item>();

        _item.prefab          = Unremove.Ref("Items/Item Upgrade Player Grab Throw");
        _item.name            = _item.prefab.prefabName;
        _item.itemName        = "Throw Upgrade";
        _item.itemType        = SemiFunc.itemType.item_upgrade;
        _item.itemVolume      = SemiFunc.itemVolume.upgrade;
        _item.maxAmount       = 10;
        _item.maxAmountInShop = 3;
        // Consider extracting Value preset from the game's assets
        var value = _item.value = ScriptableObject.CreateInstance<Value>();
        value.valueMin = 250;
        value.valueMax = 500;
    }
}
