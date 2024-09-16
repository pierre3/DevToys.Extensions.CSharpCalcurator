using DevToys.Api;
using System.ComponentModel.Composition;

namespace DevToys.Extensions.CSharpCalculator;

[Export(typeof(GuiToolGroup))]
[Name("Calculators")]
[Order(After = PredefinedCommonToolGroupNames.Converters)]
internal class CalculatorsGroup : GuiToolGroup
{
    [ImportingConstructor]
    internal CalculatorsGroup()
    {
        IconFontName = "FluentSystemIcons";
        IconGlyph = (char)61962;
        DisplayTitle = CSharpCalculator.MyGroupDisplayTitle;
        AccessibleName = CSharpCalculator.MyGroupAccessibleName;
    }
}