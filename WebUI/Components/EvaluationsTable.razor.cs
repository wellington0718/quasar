using Microsoft.JSInterop;
using Radzen.Blazor;
using SharedKernel.JsHelpers;
namespace WebUI.Components;

public partial class EvaluationsTable
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }
    [CascadingParameter]
    private Error? Error { get; set; }

    [Inject]
    private DialogService? DialogService { get; set; }

    [Inject]
    private IEvaluatorProjectUseCases? EvaluatorProjectUseCases { get; set; }
    [Inject]
    private NotificationService? NotificationService { get; set; }
    [Inject]
    private IEvaluationUseCases? EvaluationUseCases { get; set; }
    [Inject]
    private IJSRuntime? JSRuntime { get; set; }
    [Inject]
    private IFileBuilder? FileBuilder { get; set; }

    [Parameter]
    public List<EvaluationDTO> EvaluationDTOs { get; set; } = new();

    [Parameter]
    public bool ShowEvaluator { get; set; }

    [Parameter]
    public EventCallback<string> OnEvaluationStateChnage { get; set; }

    [Parameter]
    public bool ShowExportButtons { get; set; }


    [Parameter]
    public string? EvaluationStateFilter { get; set; } = "Enabled";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    public bool showActionButtons;
    public bool isUserEvaluator;
    public RadzenDataGrid<EvaluationDTO>? evaluationsGrid;
    private dynamic? parameters;
    private string? loggedInUser;
    private string? pagingSummary;
    private DialogOptions? dialogOptions;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            loggedInUser = (await AuthenticationState)
              .User.Claims
              .First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            showActionButtons = (await EvaluatorProjectUseCases.GetEmployeePermissionsAsync(loggedInUser)).ToList().Any();
            var user = (await EvaluatorProjectUseCases.GetEmployeesAsync(new {EmployeeId = loggedInUser })).FirstOrDefault();
            isUserEvaluator = (await EvaluatorProjectUseCases.IsEmployeeEvaluatorAsync(new { EmployeeId = loggedInUser }));
                

            StateHasChanged();
        }
    }

    protected async override Task OnParametersSetAsync()
    {
        if (EvaluationDTOs?.Any() ?? false)
        {          
            var currentPage = EvaluationDTOs.Count() > 0 ? 1 : 0;
            pagingSummary = $"Displaying page {currentPage} of {Math.Ceiling(EvaluationDTOs.Count / 10m)} (total {EvaluationDTOs.Count()} records)";
            await GetEvaluationScoreDetails();
            StateHasChanged();
        }
        else
        {
            pagingSummary= "Displaying page 0 of 0 (total 0 records)";
        }
    }

    private async Task GetEvaluationScoreDetails()
    {
        foreach (var evaluationDTO in EvaluationDTOs)
        {
            evaluationDTO.EvaluationScoreDetails =
                (await EvaluationUseCases.GetEvaluationScoreDetailAsync(new { evaluationDTO.IsUsingScoreValue, EvaluationId = evaluationDTO.Id })).ToList();

            var score = 100;

            if (evaluationDTO.IsUsingScoreValue)
            {
                evaluationDTO.Score = score -= evaluationDTO.EvaluationScoreDetails.Sum(e => e.ComponentValue);
            }
            else
            {
                evaluationDTO.Score = evaluationDTO.EvaluationScoreDetails.Any(e => !e.Fufilled) ? 0 : score;
            }
        }

        DialogService?.Close(false);
    }

    private void ShowEvaluationScoreDetails(EvaluationDTO evaluationDTO)
    {
        var evaluationScoreDetails = evaluationDTO.EvaluationScoreDetails?? new List<EvaluationScoreDetail>();
        var parameters = new Dictionary<string, object> { { "EvaluationScoreDetails", evaluationScoreDetails }, { "IsUsingScoreValue", evaluationDTO.IsUsingScoreValue } };
        DialogService?.Open<ScoreDetails>("Score details", parameters, new DialogOptions { Style="min-width:500px !important", CloseDialogOnOverlayClick=true });
    }

    private async Task EditRow(EvaluationDTO evaluationDTOToEdit)
    {
        evaluationDTOToEdit.EditionNotes = null;

        var evaluationScoreDetails = evaluationDTOToEdit.EvaluationScoreDetails.Select(evaluationToEdit => new EvaluationScoreDetail
        {
            Id = evaluationToEdit.Id,
            ComponentName = evaluationToEdit.ComponentName,
            ComponentValue = evaluationToEdit.ComponentValue, 
            Fufilled = evaluationToEdit.Fufilled
        }).ToList();

        var evaluationBeforeUpdate = new EvaluationDTO
        {
            Id = evaluationDTOToEdit.Id,
            EvaluationTypeId = evaluationDTOToEdit.EvaluationTypeId,
            AgentId = evaluationDTOToEdit.AgentId,
            AgentName = evaluationDTOToEdit.AgentName,
            EvaluationTypeName = evaluationDTOToEdit.EvaluationTypeName,
            ProjectId = evaluationDTOToEdit.ProjectId,
            EditionNotes = evaluationDTOToEdit.EditionNotes,
            Score = evaluationDTOToEdit.Score,
            EvaluatorId = evaluationDTOToEdit.EvaluatorId,
            EvaluatorName = evaluationDTOToEdit.EvaluatorName,
            CaseNumber = evaluationDTOToEdit.CaseNumber,
            AccountNumber = evaluationDTOToEdit.AccountNumber,
            OriginalDate = evaluationDTOToEdit.OriginalDate,
            ModificationDate = evaluationDTOToEdit.ModificationDate,
            Edited = evaluationDTOToEdit.Edited,
            Enabled = evaluationDTOToEdit.Enabled,
            Comments = evaluationDTOToEdit.Comments,
            EvaluationScoreDetails = evaluationScoreDetails,
            IsUsingScoreValue = evaluationDTOToEdit.IsUsingScoreValue
        };

        dialogOptions = new DialogOptions { CloseDialogOnOverlayClick = true, AutoFocusFirstElement = true, Resizable = true };

        parameters = new Dictionary<string, object>
        {
            { "SelectedEvaluationDTO", evaluationDTOToEdit}
        };

        var isChangesCommitted = await DialogService.OpenAsync<EditEvaluationForm>("Editing evaluation", parameters, dialogOptions);

        if (isChangesCommitted?? false)
            await evaluationsGrid.UpdateRow(evaluationDTOToEdit);
        else
           CancelEvaluationChanges(evaluationDTOToEdit, evaluationBeforeUpdate);
    }

    private static void CancelEvaluationChanges(EvaluationDTO evaluationDTOToEdit, EvaluationDTO evaluationBeforeUpdate)
    {
            evaluationDTOToEdit.Id = evaluationBeforeUpdate.Id;
            evaluationDTOToEdit.EvaluationTypeId = evaluationBeforeUpdate.EvaluationTypeId;
            evaluationDTOToEdit.AgentId = evaluationBeforeUpdate.AgentId;
            evaluationDTOToEdit.AgentName = evaluationBeforeUpdate.AgentName;
            evaluationDTOToEdit.EvaluationTypeName = evaluationBeforeUpdate.EvaluationTypeName;
            evaluationDTOToEdit.ProjectId = evaluationBeforeUpdate.ProjectId;
            evaluationDTOToEdit.EditionNotes = evaluationBeforeUpdate.EditionNotes;
            evaluationDTOToEdit.Score = evaluationBeforeUpdate.Score;
            evaluationDTOToEdit.EvaluatorId = evaluationBeforeUpdate.EvaluatorId;
            evaluationDTOToEdit.EvaluatorName = evaluationBeforeUpdate.EvaluatorName;
            evaluationDTOToEdit.CaseNumber = evaluationBeforeUpdate.CaseNumber;
            evaluationDTOToEdit.AccountNumber = evaluationBeforeUpdate.AccountNumber;
            evaluationDTOToEdit.OriginalDate = evaluationBeforeUpdate.OriginalDate;
            evaluationDTOToEdit.ModificationDate = evaluationBeforeUpdate.ModificationDate;
            evaluationDTOToEdit.Edited = evaluationBeforeUpdate.Edited;
            evaluationDTOToEdit.Enabled = evaluationBeforeUpdate.Enabled;
            evaluationDTOToEdit.Comments = evaluationBeforeUpdate.Comments;
            evaluationDTOToEdit.IsUsingScoreValue = evaluationBeforeUpdate.IsUsingScoreValue;
            evaluationDTOToEdit.EvaluationScoreDetails = evaluationBeforeUpdate.EvaluationScoreDetails;
    }

    private async Task ShowEvaluationHistory(EvaluationDTO evaluationDTO)
    {
        var evaluationChangesHistory = await EvaluationUseCases.GetEvaluationChangesHistoryAsync(new { EvaluationId = evaluationDTO.Id });
        dialogOptions = new DialogOptions() { CloseDialogOnOverlayClick = true, Style="min-width:80% !important" };

        parameters = new Dictionary<string, object>
        {
             { "EvaluationChangesHistory", evaluationChangesHistory}
        };

        DialogService.Open<EvaluationChangesHistoryTable>("Evaluation history", parameters, dialogOptions);
    }

    private async Task<bool> ShowEvaluationStateChangeDialog(EvaluationDTO selectedEvaluation)
    {
        dialogOptions = new DialogOptions() { CloseDialogOnOverlayClick = true, Style="min-width:40% !important" };

        var response = await DialogService.OpenAsync<DisableEvaluationForm>("Are you absolutely sure?",
              new Dictionary<string, object> { { "SelectedEvaluation", selectedEvaluation } }, dialogOptions);
        return response?? false;
    }

    private async Task ChangeEvaluationState(EvaluationDTO selectedEvaluation)
    {
        var response = await ShowEvaluationStateChangeDialog(selectedEvaluation);

        if (response)
        {
            DialogService.Open<Loading>(null);
            var enabled = !selectedEvaluation.Enabled;
            var parameters = new
            {
                EvaluationId = selectedEvaluation.Id,
                selectedEvaluation.EvaluationTypeId,
                selectedEvaluation.Score,
                selectedEvaluation.AgentId,
                EditorId = loggedInUser,
                selectedEvaluation.EvaluatorId,
                selectedEvaluation.OriginalDate,
                ModificationDate = DateTime.Now,
                selectedEvaluation.CaseNumber,
                selectedEvaluation.AccountNumber,
                enabled,
                IsEvaluationCreated = true,
                selectedEvaluation.EditionNotes,
                selectedEvaluation.Comments,
            };

            var isSaved = await EvaluationUseCases.SaveEvaluationAsync(parameters);
            if (isSaved)
            {
               await OnEvaluationStateChnage.InvokeAsync(selectedEvaluation.Id);
                var infoMessage = $"Evaluation was {(enabled ? "enabled" : "disabled") }";
                selectedEvaluation.Enabled = enabled;
                if (!EvaluationStateFilter.Equals("All"))
                    EvaluationDTOs.Remove(selectedEvaluation);
                await evaluationsGrid.Reload();
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = infoMessage, Duration = 2000 });
            }
        }

        DialogService.Close();
    }

    private void OnPage(PagerEventArgs args)
    {
        pagingSummary = $"Displaying page {args.PageIndex +1} of {Math.Ceiling(EvaluationDTOs.Count/10m)} (total {EvaluationDTOs.Count()} records)";
    }

    private async Task ExportExcel()
    {
        DataTable dataTable = new();

        dataTable.Columns.Add("Evaluation date", typeof(string));
        dataTable.Columns.Add("Evaluation type", typeof(string));
        dataTable.Columns.Add("Agent", typeof(string));
        dataTable.Columns.Add("Evaluator", typeof(string));
        dataTable.Columns.Add("Case number", typeof(string));
        dataTable.Columns.Add("Account number", typeof(string));
        dataTable.Columns.Add("Comments", typeof(string));
        dataTable.Columns.Add("Score", typeof(string));
        dataTable.Columns.Add("Enabled", typeof(string));
        dataTable.Columns.Add("Edited", typeof(string));

        EvaluationDTOs?.OrderBy(e => e.OriginalDate)
            .ToList().ForEach(evaluation =>
            {
                dataTable.Rows.Add
                (
                    evaluation.OriginalDate.ToString("MM/dd/yyyy"),
                    evaluation.EvaluationTypeName,
                    evaluation.AgentIdCommonName,
                    evaluation.EvaluatorIdCommonName,
                    evaluation.CaseNumber,
                    evaluation.AccountNumber,
                    evaluation.Comments,
                    evaluation.Score,
                    evaluation.EnabledString,
                    evaluation.EditedString
                );

            });

        var bytes = FileBuilder.BuildExcelFile(dataTable);
        await JSRuntime?.SaveAs("evaluations.xlsx", bytes);
    }

    private async Task ExportCSV()
    {
        var bytes = FileBuilder.BuildCSVFile(EvaluationDTOs?.OrderBy(e => e.OriginalDate), "evaluation");
        await JSRuntime?.SaveAs("evaluations.csv", bytes);
    }
}

