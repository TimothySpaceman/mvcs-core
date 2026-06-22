using System.Text.RegularExpressions;

namespace Core.WorkingDirectories;

public class IgnoreRuleSetParser
{
    private static readonly Regex ValidPattern = new(
        @"^(?!.*[|<>\0])"
        + @"(?!\/?\.\.([\/\\]|$))"
        + @"(?![a-zA-Z]:)"
        + @"(?!\/)"
        + @"(?!.*[\/\\]\.\.[\/\\])"
        + @"(?!.*(?<![\/\\])\*\*(?![\/\\]|$))"
        + @".*$",
        RegexOptions.Compiled
    );

    private static readonly Regex BalancedBrackets = new(
        @"^[^\[]*(\[[^\]]+\][^\[]*)*$",
        RegexOptions.Compiled
    );

    private static bool IsComment(string pattern) => pattern.TrimStart().StartsWith('#');
    private static bool IsNegated(string pattern) => pattern.StartsWith('!');
    
    private static bool IsValidPattern(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return false;

        var trimmed = raw.Trim();
        var isNegated = IsNegated(trimmed);
        var pattern = isNegated ? trimmed.Substring(1) : trimmed;
        if (string.IsNullOrEmpty(pattern) || IsNegated(pattern)) return false;

        return ValidPattern.IsMatch(pattern) && BalancedBrackets.IsMatch(pattern);
    }

    public static void HydrateIgnoreRuleSet(IgnoreRuleSet target, IEnumerable<string> rules, bool reset = false)
    {
        if (reset)
        {
            target.IncludeRules.Clear();
            target.ExcludeRules.Clear();
        }
        
        foreach (var rule in rules)
        {
            if(IsComment(rule) || !IsValidPattern(rule)) continue;
            
            var trimmed = rule.Trim();   
            if(IsNegated(trimmed)) target.IncludeRules.Add(trimmed.Substring(1));
            else target.ExcludeRules.Add(trimmed);
        }
    }
}