namespace Core.WorkingDirectories;

public class IgnoreRuleSet
{
    public List<string> IncludeRules { get; set; } = new();
    public List<string> ExcludeRules { get; set; } = new();
}