
namespace WebUI.Components;

public partial class CreateEvaluationTypesComponents
{
    private CreateEvaluationTypeDTO model = new();
    private bool wasComponentCreated;
    private bool componentExists;
    private string ExisitingComponentName = string.Empty;
    [CascadingParameter]
    private Error? Error { get; set; }
    [Inject]
    private IEvaluationTypesUseCases? EvaluationTypesUseCases { get; set; }
    [Parameter]
    public List<EvaluationComponent> Components { get; set; } = new();

    [Parameter]
    public EventCallback<List<EvaluationComponent>> OnAddComponent { get; set; }

    [Parameter]
    public EvaluationType? EvaluationType { get; set; } 
    [Parameter]
    public bool IsUsingScoreValue { get; set; } 

    [Parameter]
    public bool HasAnyComponents { get; set; }

    protected override void OnParametersSet()
    {
        HasAnyComponents = wasComponentCreated || HasAnyComponents;
        model.IsUsingScoreValue = IsUsingScoreValue;
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (EvaluationType is not null)
            {
                Components = (await EvaluationTypesUseCases.GetEvaluationTypeComponentsAsync(new { EvaluationType.IsUsingScoreValue, EvaluationTypeId = EvaluationType.Id })).ToList();
            }

            StateHasChanged();
        }
    }

    private async void AddComponent()
    {
        wasComponentCreated = true;

        Components.Add(new EvaluationComponent
        {
            DeductionValue = model.EvaluationComponent.DeductionValue,
            Name = model.EvaluationComponent.Name,
        });

        await OnAddComponent.InvokeAsync(Components);
        model.EvaluationComponent = new EvaluationComponent();
    }

    private async void HandleComponentChange(EvaluationComponent evaluationComponent)
    {
        var componentsToCompare = Components.Where(component => !component.Id.Equals(evaluationComponent.Id)).ToList();
        componentExists = componentsToCompare.Any(c => c.Name.Trim().Equals(evaluationComponent.Name.Trim(), StringComparison.OrdinalIgnoreCase));

        if (componentExists)
        {
            ExisitingComponentName = evaluationComponent.Name.Trim();
            StateHasChanged();
            return;

        }else
        {
            ExisitingComponentName = "";
        }
        await OnAddComponent.InvokeAsync(Components);
    }

    private async void RemoveComponent(EvaluationComponent component)
    {
        Components.Remove(component);
        await OnAddComponent.InvokeAsync(Components);
        wasComponentCreated = false;
    }
}

