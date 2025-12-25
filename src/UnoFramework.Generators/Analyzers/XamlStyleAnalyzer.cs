// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace UnoFramework.Generators.Analyzers;

/// <summary>
/// Analyzer that verifies all XAML elements have a Style attribute defined.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class XamlStyleAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic ID for missing style.
    /// </summary>
    public const string DiagnosticId = "UNO0001";

    private const string Category = "Design";

    private static readonly LocalizableString Title =
        "XAML element is missing a Style";

    private static readonly LocalizableString MessageFormat =
        "Element '{0}' is missing a Style attribute";

    private static readonly LocalizableString Description =
        "All XAML UI elements should have a Style attribute defined for consistent styling.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description,
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    /// <summary>
    /// XAML namespaces that indicate WinUI/UWP/Uno Platform XAML.
    /// </summary>
    private static readonly HashSet<string> XamlNamespaces = new(StringComparer.OrdinalIgnoreCase)
    {
        "http://schemas.microsoft.com/winfx/2006/xaml/presentation",
        "using:Microsoft.UI.Xaml.Controls",
        "using:Windows.UI.Xaml.Controls"
    };

    /// <summary>
    /// Elements that typically don't need styles (layout containers, etc.).
    /// </summary>
    private static readonly HashSet<string> ExcludedElements = new(StringComparer.OrdinalIgnoreCase)
    {
        // Layout containers
        "Grid",
        "StackPanel",
        "RelativePanel",
        "Canvas",
        "Border",
        "Viewbox",
        "ScrollViewer",
        "ScrollContentPresenter",
        "ItemsPresenter",
        "ContentPresenter",
        "Frame",
        "Page",
        "UserControl",
        "Window",
        "Application",
        "ResourceDictionary",
        "AutoLayout",

        // Row/Column definitions
        "RowDefinition",
        "RowDefinitions",
        "ColumnDefinition",
        "ColumnDefinitions",

        // Resources and templates
        "Style",
        "Setter",
        "DataTemplate",
        "ControlTemplate",
        "ItemsPanelTemplate",
        "Storyboard",
        "VisualState",
        "VisualStateGroup",
        "VisualStateManager.VisualStateGroups",
        "VisualTransition",
        "KeyFrame",
        "DoubleAnimation",
        "ColorAnimation",
        "ObjectAnimationUsingKeyFrames",
        "DoubleAnimationUsingKeyFrames",
        "DiscreteObjectKeyFrame",
        "LinearDoubleKeyFrame",
        "SplineDoubleKeyFrame",
        "EasingDoubleKeyFrame",
        "StateTrigger",
        "AdaptiveTrigger",

        // Data binding and converters
        "Binding",
        "x:Bind",
        "StaticResource",
        "ThemeResource",
        "TemplateBinding",

        // Shapes (often styled inline for simplicity)
        "Path",
        "PathGeometry",
        "PathFigure",
        "LineSegment",
        "BezierSegment",
        "ArcSegment",
        "PolyLineSegment",
        "PolyBezierSegment",
        "QuadraticBezierSegment",
        "PolyQuadraticBezierSegment",
        "EllipseGeometry",
        "RectangleGeometry",
        "GeometryGroup",
        "CombinedGeometry",
        "TransformGroup",
        "RotateTransform",
        "ScaleTransform",
        "TranslateTransform",
        "SkewTransform",
        "MatrixTransform",
        "CompositeTransform",

        // Brushes and colors
        "SolidColorBrush",
        "LinearGradientBrush",
        "RadialGradientBrush",
        "ImageBrush",
        "GradientStop",
        "AcrylicBrush",

        // Effects
        "Shadow",
        "ThemeShadow",
        "DropShadow",

        // Primitive types
        "Run",
        "Span",
        "Bold",
        "Italic",
        "Underline",
        "LineBreak",
        "InlineUIContainer",
        "Paragraph",

        // Menu and context items (styled via parent)
        "MenuFlyoutItem",
        "MenuFlyoutSubItem",
        "MenuFlyoutSeparator",
        "ToggleMenuFlyoutItem",
        "RadioMenuFlyoutItem",

        // Navigation items
        "NavigationViewItem",
        "NavigationViewItemHeader",
        "NavigationViewItemSeparator",
        "PivotItem",
        "TabViewItem",

        // Misc
        "ToolTip",
        "FlyoutPresenter",
        "MenuFlyoutPresenter",
        "Popup",
        "SymbolIcon",
        "FontIcon",
        "BitmapIcon",
        "PathIcon",
        "ImageIcon",

        // Data items (auto-styled)
        "ComboBoxItem",
        "ListBoxItem",
        "ListViewItem",
        "GridViewItem",
        "TreeViewItem",

        // Column definitions for data grids
        "DataGridTextColumn",
        "DataGridTemplateColumn",
        "DataGridCheckBoxColumn",
        "DataGridComboBoxColumn"
    };

    /// <summary>
    /// Elements that MUST have a style for consistent design.
    /// </summary>
    private static readonly HashSet<string> RequiredStyleElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "Button",
        "TextBlock",
        "TextBox",
        "ComboBox",
        "ListView",
        "GridView",
        "CheckBox",
        "RadioButton",
        "ToggleSwitch",
        "Slider",
        "ProgressBar",
        "ProgressRing",
        "Image",
        "MediaPlayerElement",
        "MediaElement",
        "CalendarDatePicker",
        "DatePicker",
        "TimePicker",
        "AutoSuggestBox",
        "PasswordBox",
        "RichEditBox",
        "NumberBox",
        "NavigationView",
        "TabView",
        "TreeView",
        "Expander",
        "InfoBar",
        "TeachingTip",
        "ContentDialog",
        "Flyout",
        "MenuFlyout",
        "CommandBar",
        "AppBar",
        "AppBarButton",
        "AppBarToggleButton",
        "SplitButton",
        "DropDownButton",
        "ToggleButton",
        "HyperlinkButton",
        "RepeatButton",
        "RatingControl",
        "ColorPicker",
        "PersonPicture",
        "Pivot",
        "FlipView",
        "Hub",
        "HubSection",
        "SemanticZoom",
        "SplitView",
        "TwoPaneView",
        "Card",
        "CardContentControl",
        "Chip",
        "ChipGroup",
        "Divider",
        "LoadingView",
        "NavigationBar",
        "ResponsiveView",
        "SafeArea",
        "ShadowContainer",
        "TabBar",
        "ZoomContentControl",
        "DrawerControl"
    };

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private static void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        ImmutableArray<AdditionalText> xamlFiles = context.Options.AdditionalFiles
            .Where(f => f.Path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
            .ToImmutableArray();

        foreach (AdditionalText xamlFile in xamlFiles)
        {
            AnalyzeXamlFile(context, xamlFile);
        }
    }

    private static void AnalyzeXamlFile(CompilationAnalysisContext context, AdditionalText xamlFile)
    {
        SourceText? sourceText = xamlFile.GetText(context.CancellationToken);
        if (sourceText is null)
            return;

        string xamlContent = sourceText.ToString();

        try
        {
            XDocument doc = XDocument.Parse(xamlContent, LoadOptions.SetLineInfo);

            // Check if this is a XAML file we should analyze (not a ResourceDictionary only file)
            XElement? root = doc.Root;
            if (root is null)
                return;

            // Skip ResourceDictionary files
            if (root.Name.LocalName == "ResourceDictionary")
                return;

            AnalyzeElement(context, xamlFile.Path, root, sourceText);
        }
        catch (XmlException)
        {
            // Ignore XML parsing errors - the XAML compiler will report those
        }
    }

    private static void AnalyzeElement(
        CompilationAnalysisContext context,
        string filePath,
        XElement element,
        SourceText sourceText)
    {
        string elementName = element.Name.LocalName;

        // Check if this element requires a style
        if (RequiredStyleElements.Contains(elementName))
        {
            // Check for Style attribute
            bool hasStyle = element.Attributes()
                .Any(a => a.Name.LocalName.Equals("Style", StringComparison.OrdinalIgnoreCase));

            if (!hasStyle)
            {
                // Get line info for the diagnostic
                IXmlLineInfo lineInfo = element;
                int line = lineInfo.HasLineInfo() ? lineInfo.LineNumber : 1;
                int column = lineInfo.HasLineInfo() ? lineInfo.LinePosition : 1;

                // Create a location for the diagnostic
                TextSpan textSpan = GetTextSpanForLine(sourceText, line, column, elementName.Length);
                Location location = Location.Create(filePath, textSpan, new LinePositionSpan(
                    new LinePosition(line - 1, column - 1),
                    new LinePosition(line - 1, column - 1 + elementName.Length)));

                Diagnostic diagnostic = Diagnostic.Create(Rule, location, elementName);
                context.ReportDiagnostic(diagnostic);
            }
        }

        // Recursively check child elements
        foreach (XElement child in element.Elements())
        {
            AnalyzeElement(context, filePath, child, sourceText);
        }
    }

    private static TextSpan GetTextSpanForLine(SourceText sourceText, int line, int column, int length)
    {
        try
        {
            if (line < 1 || line > sourceText.Lines.Count)
                return new TextSpan(0, 0);

            TextLine textLine = sourceText.Lines[line - 1];
            int start = textLine.Start + Math.Max(0, column - 1);
            int end = Math.Min(start + length, textLine.End);

            return TextSpan.FromBounds(start, end);
        }
        catch
        {
            return new TextSpan(0, 0);
        }
    }
}
