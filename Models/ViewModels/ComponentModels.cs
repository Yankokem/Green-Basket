namespace Green_Basket.Models.ViewModels
{
    public class ButtonModel
    {
        public string Text { get; set; } = string.Empty;
        public string? Href { get; set; }
        public string Variant { get; set; } = "primary"; // primary, secondary, outline, danger, ghost, subtle
        public string Size { get; set; } = "md"; // sm, md, icon
        public string? Icon { get; set; }
        public string? IconRight { get; set; }
        public bool IsSubmit { get; set; } = false;
        public bool FullWidth { get; set; } = false;
        public string? CustomClass { get; set; }
        public string? OnClick { get; set; }
        public string? Title { get; set; }
        public string? ModalTarget { get; set; }
        public int? ModalAssetId { get; set; }
        public string? ModalTitlePrefill { get; set; }
    }

    public class PageHeaderModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? Eyebrow { get; set; }
        public string? BackUrl { get; set; }
        public string? BadgeValue { get; set; }
        public string? BadgeType { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionModalTarget { get; set; }
        public int? ActionModalAssetId { get; set; }
        public string? ActionIcon { get; set; } = "plus";
        public string? RequiredRole { get; set; }
        public string? EditUrl { get; set; }
        public string? EditRequiredRole { get; set; } = "Admin,DRCoordinator";
        public string? DeleteUrl { get; set; }
        public string? DeleteRequiredRole { get; set; } = "Admin";
    }

    public class BadgeModel
    {
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = "priority";
        public bool ShowDot { get; set; } = true;
    }

    public class TableToolbarModel
    {
        public string TargetId { get; set; } = "data-list";
        public string Title { get; set; } = "Records";
        public int Count { get; set; }
        public string SearchPlaceholder { get; set; } = "Search...";
        public string FilterLabel { get; set; } = "All";
        public List<string> FilterOptions { get; set; } = new();
        public string? RightNote { get; set; }
    }

    public class StatCardModel
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public string? Subtext { get; set; }
        public string Icon { get; set; } = "activity";
        public string Tone { get; set; } = "sage"; // sage, slate, rose, amber, blue
    }

    public class EmptyStateModel
    {
        public string Icon { get; set; } = "inbox";
        public string Title { get; set; } = "No records found.";
        public string? Subtitle { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionModalTarget { get; set; }
        public int? ActionModalAssetId { get; set; }
        public string? RequiredRole { get; set; }
    }
}
