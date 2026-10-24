namespace Green_Basket.Models.ViewModels
{
    public class ButtonModel
    {
        public string Text { get; set; } = string.Empty;
        public string? Href { get; set; }
        public string Variant { get; set; } = "primary"; // primary, secondary, danger, outline, ghost
        public string? Icon { get; set; } // e.g. "plus", "edit", "trash", "arrow-left", "shield"
        public bool IsSubmit { get; set; } = false;
        public string? CustomClass { get; set; }
        public string? OnClick { get; set; }
    }

    public class PageHeaderModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionIcon { get; set; } = "plus";
        public string? RequiredRole { get; set; } // e.g. "Admin,DRCoordinator"
    }

    public class BadgeModel
    {
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = "priority"; // "priority", "status", "result", "comparison"
    }
}
