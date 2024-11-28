using UnityEngine;

public class CellVisualManager : MonoBehaviour
{
    [Header("ITEMS VISUAL")]
    public Sprite stairSprite;
    public Sprite potionSprite;
    public Sprite swordSprite;


    public Sprite UpdateItemVisuel(ItemTypeEnum itemType)
    {
        Sprite spriteItem = null;
        if (itemType == ItemTypeEnum.None)
        {
            return null;
        }
        else if (itemType == ItemTypeEnum.Potion)
        {
            spriteItem = potionSprite;
        }
        else if (itemType == ItemTypeEnum.Sword)
        {
            spriteItem = swordSprite;
        }
        return spriteItem;
    }
}
