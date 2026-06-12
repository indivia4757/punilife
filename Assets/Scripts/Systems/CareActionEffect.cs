public readonly struct CareActionEffect
{
    public readonly int hunger;
    public readonly int happiness;
    public readonly int cleanliness;
    public readonly int energy;
    public readonly int affection;
    public readonly int stress;
    public readonly int coin;
    public readonly int exp;
    public readonly int intelligence;
    public readonly int strength;
    public readonly int sensitivity;
    public readonly int courage;
    public readonly int kindness;
    public readonly string successMessage;

    public CareActionEffect(
        int hunger = 0,
        int happiness = 0,
        int cleanliness = 0,
        int energy = 0,
        int affection = 0,
        int stress = 0,
        int coin = 0,
        int exp = 0,
        int intelligence = 0,
        int strength = 0,
        int sensitivity = 0,
        int courage = 0,
        int kindness = 0,
        string successMessage = "")
    {
        this.hunger = hunger;
        this.happiness = happiness;
        this.cleanliness = cleanliness;
        this.energy = energy;
        this.affection = affection;
        this.stress = stress;
        this.coin = coin;
        this.exp = exp;
        this.intelligence = intelligence;
        this.strength = strength;
        this.sensitivity = sensitivity;
        this.courage = courage;
        this.kindness = kindness;
        this.successMessage = successMessage;
    }
}
