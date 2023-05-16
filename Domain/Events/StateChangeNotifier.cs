namespace Domain.Events
{
    public class StateChangeNotifier
    {
        public event System.Action OnChange;

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
