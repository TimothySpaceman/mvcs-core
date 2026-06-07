namespace Core.WorkingDirectories;

public class IgnoreRuleSet
{
    public List<string> IncludeRules { get; set; } = new();
    public List<string> ExcludeRules { get; set; } = new();

    public void FillFrom(IgnoreRuleSet source, bool overwrite = false)
    {
        if (overwrite)
        {
            IncludeRules.Clear();
            ExcludeRules.Clear();
        }
        IncludeRules.AddRange(source.IncludeRules);
        ExcludeRules.AddRange(source.ExcludeRules);
    }
}