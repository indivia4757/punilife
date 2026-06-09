using System.Collections.Generic;

public static class DefaultItemCatalog
{
    public static List<ItemData> Create()
    {
        return new List<ItemData>
        {
            new ItemData(1, "Basic Snack", ItemType.Snack, 10, hungerValue: 25, affectionValue: 3),
            new ItemData(2, "Premium Snack", ItemType.Snack, 35, hungerValue: 45, happinessValue: 10, affectionValue: 8),
            new ItemData(3, "Medicine", ItemType.Medicine, 50, happinessValue: 5),
            new ItemData(101, "Flower Room", ItemType.RoomTheme, 120),
            new ItemData(201, "Tiny Flower", ItemType.Decoration, 80)
        };
    }
}
