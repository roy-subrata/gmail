namespace GmailApp.Models;

public class EmailTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DefaultSubject { get; set; } = string.Empty;
    public List<TemplateField> Fields { get; set; } = new();
    public Func<Dictionary<string, string>, string> BuildHtml { get; set; } = _ => string.Empty;
}

public class TemplateField
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public string DefaultValue { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
}
