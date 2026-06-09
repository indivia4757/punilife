using System;

[Serializable]
public sealed class ItemData
{
    public int id;
    public string name;
    public ItemType type;
    public int price;
    public int hungerValue;
    public int happinessValue;
    public int cleanlinessValue;
    public int energyValue;
    public int affectionValue;

    public ItemData()
    {
    }

    public ItemData(
        int id,
        string name,
        ItemType type,
        int price,
        int hungerValue = 0,
        int happinessValue = 0,
        int cleanlinessValue = 0,
        int energyValue = 0,
        int affectionValue = 0)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.price = price;
        this.hungerValue = hungerValue;
        this.happinessValue = happinessValue;
        this.cleanlinessValue = cleanlinessValue;
        this.energyValue = energyValue;
        this.affectionValue = affectionValue;
    }
}
