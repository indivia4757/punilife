public readonly struct EvolutionUpdateResult
{
    public readonly bool stageChanged;
    public readonly bool evolved;
    public readonly PuniStage previousStage;
    public readonly PuniStage currentStage;
    public readonly PuniEvolutionType evolutionType;

    public EvolutionUpdateResult(
        bool stageChanged,
        bool evolved,
        PuniStage previousStage,
        PuniStage currentStage,
        PuniEvolutionType evolutionType)
    {
        this.stageChanged = stageChanged;
        this.evolved = evolved;
        this.previousStage = previousStage;
        this.currentStage = currentStage;
        this.evolutionType = evolutionType;
    }
}
