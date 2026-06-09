using System.Collections.Generic;

public static class DefaultItemCatalog
{
    public static List<ItemData> Create()
    {
        return new List<ItemData>
        {
            new ItemData(1, "기본 간식", ItemType.Snack, 10, hungerValue: 25, affectionValue: 3),
            new ItemData(2, "프리미엄 간식", ItemType.Snack, 35, hungerValue: 45, happinessValue: 10, affectionValue: 8),
            new ItemData(3, "약", ItemType.Medicine, 50, happinessValue: 5),
            new ItemData(101, "꽃 방", ItemType.RoomTheme, 120),
            new ItemData(201, "작은 꽃", ItemType.Decoration, 80)
        };
    }
}
