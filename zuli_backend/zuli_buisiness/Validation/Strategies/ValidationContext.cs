namespace zuli_Business.Validation.Strategies
{
    public class ValidationContext
    {
        public Dictionary<string, List<string>> Errors { get; } = new();
        public bool HasErrors => Errors.Count > 0;

        public void AddError(string key, string message)
        {
            if (!Errors.ContainsKey(key))
                Errors[key] = [];
            Errors[key].Add(message);
        }
    }
}
