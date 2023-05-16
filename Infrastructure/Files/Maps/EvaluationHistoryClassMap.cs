using CsvHelper.Configuration;

namespace Infrastructure.Files.Maps
{
    public sealed class EvaluationHistoryClassMap : ClassMap<EvaluationDTO>
    {
        public EvaluationHistoryClassMap()
        {
            Map(evaluation => evaluation.ModificationDateString).Name("Modification date");
            Map(evaluation => evaluation.OriginalDateString).Name("Evaluation date");
            Map(evaluation => evaluation.EvaluationTypeName).Name("Evaluation type");
            Map(evaluation => evaluation.AgentIdCommonName).Name("Agent");
            Map(evaluation => evaluation.EvaluatorIdCommonName).Name("Evaluator");
            Map(evaluation => evaluation.EditorIdCommonName).Name("Editor");
            Map(evaluation => evaluation.CaseNumber).Name("Case number");
            Map(evaluation => evaluation.AccountNumber).Name("Account number");
            Map(evaluation => evaluation.Score).Name("Score");
            Map(evaluation => evaluation.Comments).Name("Comments");
            Map(evaluation => evaluation.EditionNotes).Name("Edition notes");
        }
    }
}
