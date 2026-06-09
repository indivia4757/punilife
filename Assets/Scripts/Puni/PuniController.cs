public sealed class PuniController
{
    private readonly CareSystem careSystem;
    private readonly EvolutionSystem evolutionSystem;
    private readonly DexManager dexManager;
    private readonly GardenManager gardenManager;

    public SaveData Data { get; }

    public PuniController(
        SaveData data,
        CareSystem careSystem,
        EvolutionSystem evolutionSystem,
        DexManager dexManager,
        GardenManager gardenManager)
    {
        Data = data;
        this.careSystem = careSystem;
        this.evolutionSystem = evolutionSystem;
        this.dexManager = dexManager;
        this.gardenManager = gardenManager;
    }

    public bool PerformCare(CareActionType action, out string message)
    {
        bool applied = careSystem.Apply(action, Data, out message);
        if (!applied)
        {
            return false;
        }

        EvolutionUpdateResult evolutionResult = evolutionSystem.UpdateStageAndEvolution(Data, dexManager);
        gardenManager.UpdateGardenLevel(Data, dexManager);
        if (evolutionResult.evolved)
        {
            message = $"{PuniText.EvolutionName(Data.evolutionType)}로 진화했어요.";
        }
        else if (evolutionResult.stageChanged)
        {
            message = $"{PuniText.StageName(Data.stage)}로 성장했어요.";
        }

        return true;
    }

    public EvolutionUpdateResult RefreshProgress()
    {
        EvolutionUpdateResult evolutionResult = evolutionSystem.UpdateStageAndEvolution(Data, dexManager);
        gardenManager.UpdateGardenLevel(Data, dexManager);
        return evolutionResult;
    }
}
