public static class PuniText
{
    public static string StageName(PuniStage stage)
    {
        return stage switch
        {
            PuniStage.Egg => "알",
            PuniStage.Baby => "아기 푸니",
            PuniStage.Young => "어린 푸니",
            PuniStage.Evolved => "성장한 푸니",
            _ => "푸니"
        };
    }

    public static string EvolutionName(PuniEvolutionType type)
    {
        return type switch
        {
            PuniEvolutionType.Sunny => "햇살 푸니",
            PuniEvolutionType.Scholar => "학자 푸니",
            PuniEvolutionType.Brave => "용감 푸니",
            PuniEvolutionType.Forest => "숲의 푸니",
            PuniEvolutionType.Shadow => "그림자 푸니",
            _ => "푸니"
        };
    }
}
