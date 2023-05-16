using CsvHelper.Configuration;

namespace Infrastructure.Files.Maps;

public class EvaluationTypeQualityRateClassMap : ClassMap<QualityRateDTO>
{
    public EvaluationTypeQualityRateClassMap()
    {
        Map(evaluation => evaluation.AgentId).Name("Id");
        Map(evaluation => evaluation.AgentName).Name("Agent");
       // Map(evaluation => evaluation.ProjectName).Name("Project");
        Map(evaluation => evaluation.Process).Name("Processes");
        Map(evaluation => evaluation.OrdersAudited).Name("Orders audited");
        Map(evaluation => evaluation.QualityRating).Name("Quality rating");
        Map(evaluation => evaluation.Errors).Name("Errors");
    }
}
