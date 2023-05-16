using Microsoft.JSInterop;
using Radzen.Blazor;
using SharedKernel.JsHelpers;

namespace WebUI.Components;
public partial class EvaluationChangesHistoryTable
{
    [CascadingParameter]
    private Error? Error { get; set; }

    [Inject]
    private DialogService? DialogService { get; set; }
    [Inject]
    private IJSRuntime? JSRuntime { get; set; }
    [Inject]
    private IFileBuilder? FileBuilder { get; set; }

    [Parameter]
    public List<EvaluationDTO> EvaluationChangesHistory { get; set; } = new(); 
    
    private string? pagingSummary;
    public RadzenDataGrid<EvaluationDTO>? evaluationChangesHistoryGrid;
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (EvaluationChangesHistory?.Any() ?? false)
            {
                var currentPage = EvaluationChangesHistory.Count() > 0 ? 1 : 0;
                pagingSummary = $"Displaying page {currentPage} of {Math.Ceiling(EvaluationChangesHistory.Count/10m)} (total {EvaluationChangesHistory.Count()} records)";
            }
            else
            {
                pagingSummary= "Displaying page 0 of 0 (total 0 records)";
            }

            StateHasChanged();
        }
    }

    public async Task ExportExcel()
    {
        DataTable dataTable = new();

        dataTable.Columns.Add("Modification date", typeof(string));
        dataTable.Columns.Add("Evaluation date", typeof(string));
        dataTable.Columns.Add("Evaluation type", typeof(string));
        dataTable.Columns.Add("Agent", typeof(string));
        dataTable.Columns.Add("Evaluator", typeof(string));
        dataTable.Columns.Add("Editor", typeof(string));
        dataTable.Columns.Add("Account number", typeof(string));
        dataTable.Columns.Add("Case number", typeof(string));
        dataTable.Columns.Add("Score", typeof(string));
        dataTable.Columns.Add("Comments", typeof(string));
        dataTable.Columns.Add("Edition notes", typeof(string));

        EvaluationChangesHistory?.OrderBy(e => e.ModificationDate)
            .ToList().ForEach(evaluationHistory =>
            {
                dataTable.Rows.Add
                (
                    evaluationHistory.ModificationDateString,
                    evaluationHistory.OriginalDateString,
                    evaluationHistory.EvaluationTypeName,
                    evaluationHistory.AgentIdCommonName,
                    evaluationHistory.EvaluatorIdCommonName,
                    evaluationHistory.EditorIdCommonName,
                    evaluationHistory.AccountNumber,
                    evaluationHistory.CaseNumber,
                    evaluationHistory.Score,
                    evaluationHistory.Comments,
                    evaluationHistory.EditionNotes
                );

            });
        var bytes = FileBuilder.BuildExcelFile(dataTable);
        await JSRuntime?.SaveAs("History.xlsx", bytes);
    }

    private void OnPage(PagerEventArgs args)
    {
        pagingSummary = $"Displaying page {args.PageIndex +1} of {Math.Ceiling(EvaluationChangesHistory.Count/10m)} (total {EvaluationChangesHistory.Count()} records)";
    }

    public async Task ExportCSV()
    {
        var bytes = FileBuilder.BuildCSVFile(EvaluationChangesHistory?.OrderBy(e => e.ModificationDate), "history");
        await JSRuntime?.SaveAs("History.csv", bytes);
    }
}
