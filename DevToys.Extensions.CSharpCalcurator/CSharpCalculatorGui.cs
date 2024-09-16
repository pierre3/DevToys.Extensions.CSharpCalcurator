using CsvHelper;
using DevToys.Api;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Text;
using static DevToys.Api.GUI;

namespace DevToys.Extensions.CSharpCalculator;

[Export(typeof(IGuiTool))]
[Name("CSharpCalculator")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = (char)61962,
    GroupName = "Calculators",
    ResourceManagerAssemblyIdentifier = nameof(CSharpCalculatorResourceAssemblyIdentifier),
    ResourceManagerBaseName = "DevToys.Extensions.CSharpCalculator.CSharpCalculator",
    ShortDisplayTitleResourceName = nameof(CSharpCalculator.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(CSharpCalculator.LongDisplayTitle),
    DescriptionResourceName = nameof(CSharpCalculator.Description),
    AccessibleNameResourceName = nameof(CSharpCalculator.AccessibleName))]
internal sealed class CSharpCalculatorGui : IGuiTool
{
    private enum GridRows
    {
        Top,
        Middle,
        Bottom
    }
    private enum GridColumns
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    private enum DataSourceType
    {
        Integer,
        Float,
        Decimal,
        String,
        Custom
    }

    private static readonly SettingDefinition<DataSourceType> dataSourceTypeOption
        = new(
            name: $"{nameof(CSharpCalculatorGui)}.{nameof(dataSourceTypeOption)}",
            defaultValue: DataSourceType.Integer);

    private static readonly SettingDefinition<string> customDataDefinitionOption
        = new(
            name: $"{nameof(CSharpCalculatorGui)}.{nameof(customDataDefinitionOption)}",
            defaultValue: "");

    private static readonly SettingDefinition<string> cultureInfoOption
        = new(
            name: $"{nameof(CSharpCalculatorGui)}.{nameof(cultureInfoOption)}",
            defaultValue: CultureInfo.CurrentCulture.Name);

    private static readonly SettingDefinition<bool> hasHeaderOption
        = new(
            name: $"{nameof(CSharpCalculatorGui)}.{nameof(hasHeaderOption)}",
            defaultValue: true);

    [Import]
    private ISettingsProvider _settingsProvider = null!;

    private readonly IUIMultiLineTextInput scriptTextInput = MultiLineTextInput()
        .Title(CSharpCalculator.CSharpScript)
        .Language("csharp");
    private readonly IUIMultiLineTextInput dataSourceTextInput = MultiLineTextInput()
        .Title(CSharpCalculator.DataSource);
    private readonly IUIMultiLineTextInput outputTextInput = MultiLineTextInput()
        .Title(CSharpCalculator.Output)
        .ReadOnly();
    private readonly IUISingleLineTextInput resultTextInput = SingleLineTextInput()
        .Title(CSharpCalculator.Result)
        .AlignVertically(UIVerticalAlignment.Top)
        .ReadOnly();
    private readonly IUISingleLineTextInput customDefinitionTextInput = SingleLineTextInput()
        .Title(CSharpCalculator.CustomTypeDefinition);
    private readonly IUISetting cultureInfoSetting = Setting()
        .Title(CSharpCalculator.CultureInfo)
        .Description(CSharpCalculator.CultureInfoDescription)
        .Icon("FluentSystemIcons", '\uf45f');
    private readonly IUISetting hasHeaderSetting = Setting()
        .Title(CSharpCalculator.HasHeader)
        .Icon("FluentSystemIcons", '\uec58')
        .Description(CSharpCalculator.HasHeaderDescription);
    private readonly IUIButton copyButton = Button()
        .Text(CSharpCalculator.CopyHeader)
        .Icon("FluentSystemIcons", '\uf69d')
        .AlignVertically(UIVerticalAlignment.Bottom);

    private IUIElement CustomDataDefinition()
    {
        var element =
            Grid()
                .Rows((GridRows.Top, Auto))
                .Columns(
                    (GridColumns.Left, new UIGridLength(1, UIGridUnitType.Fraction)),
                    (GridColumns.Center, Auto))
                .Cells(
                    Cell(
                        GridRows.Top,
                        GridColumns.Left,
                        customDefinitionTextInput
                            .Text(_settingsProvider.GetSetting(customDataDefinitionOption))
                            .OnTextChanged(text => _settingsProvider.SetSetting(customDataDefinitionOption, text))),
                    Cell(
                        GridRows.Top,
                        GridColumns.Center,
                        copyButton.OnClick(OnCopyButtonClick)));
        if (_settingsProvider.GetSetting(dataSourceTypeOption) == DataSourceType.Custom)
        {
            customDefinitionTextInput.Enable();
            hasHeaderSetting.Enable();
            if (_settingsProvider.GetSetting(hasHeaderOption))
            {
                copyButton.Enable();
            }
        }
        else
        {
            customDefinitionTextInput.Disable();
            hasHeaderSetting.Disable();
            copyButton.Disable();
        }
        return element;
    }


    private IUISetting DataSourceTypeSetting() =>
        SettingGroup()
            .Title(CSharpCalculator.DataSourceType)
            .Icon("FluentSystemIcons", (char)58465)
            .Description(CSharpCalculator.DataSourceTypeDescription)
            .Handle(
                _settingsProvider,
                dataSourceTypeOption,
                optionValue =>
                {
                    if (optionValue == DataSourceType.Custom)
                    {
                        customDefinitionTextInput.Enable();
                        cultureInfoSetting.Enable();
                        if (_settingsProvider.GetSetting(hasHeaderOption))
                        {
                            copyButton.Enable();
                        }
                        hasHeaderSetting.Enable();
                    }
                    else
                    {
                        customDefinitionTextInput.Disable();
                        cultureInfoSetting.Disable();
                        copyButton.Disable();
                        hasHeaderSetting.Disable();
                    }
                },
                Item(nameof(DataSourceType.Integer), DataSourceType.Integer),
                Item(nameof(DataSourceType.Float), DataSourceType.Float),
                Item(nameof(DataSourceType.Decimal), DataSourceType.Decimal),
                Item(nameof(DataSourceType.String), DataSourceType.String),
                Item(nameof(DataSourceType.Custom), DataSourceType.Custom))
            .WithChildren(
                CultureInfoSetting(),
                hasHeaderSetting
                    .Handle(_settingsProvider, hasHeaderOption, hasHeader =>
                    {
                        if (hasHeader)
                        {
                            copyButton.Enable();
                        }
                        else
                        {
                            copyButton.Disable();
                        }
                    }),
                CustomDataDefinition());

    private IUISetting CultureInfoSetting()
    {
        var items = CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Select(c => Item($"({c.Name}) {c.NativeName}", c.Name)).ToArray();

        var setting = cultureInfoSetting
            .Handle(
                _settingsProvider,
                cultureInfoOption,
                optionValue => { },
                items);
        if (_settingsProvider.GetSetting(dataSourceTypeOption) == DataSourceType.Custom)
        {
            return setting.Enable();
        }
        else
        {
            return setting.Disable();
        }
    }

    private IUIElement LeftPanelChild() =>
        Grid()
            .Rows(
                (GridRows.Top, Auto),
                (GridRows.Middle, Auto),
                (GridRows.Bottom, new UIGridLength(1, UIGridUnitType.Fraction)))
            .Columns((GridColumns.Left, Auto))
            .Cells(
                Cell(
                    GridRows.Top,
                    GridColumns.Left,
                        Button()
                            .AlignHorizontally(UIHorizontalAlignment.Center)
                            .AlignVertically(UIVerticalAlignment.Top)
                            .Icon("FluentSystemIcons", (char)62983)
                            .Text(CSharpCalculator.Run)
                            .OnClick(Calc)),
                Cell(
                    GridRows.Middle,
                    GridColumns.Left,
                    resultTextInput),
                Cell(
                    GridRows.Bottom,
                    GridColumns.Left,
                    scriptTextInput));

    private IUIElement RightPanelChild() =>
        Grid()
            .Rows(
                (GridRows.Top, Auto),
                (GridRows.Middle, new UIGridLength(1, UIGridUnitType.Fraction)))
            .Columns(
                (GridColumns.Left, new UIGridLength(1, UIGridUnitType.Fraction)))
            .Cells(
                Cell(
                    GridRows.Top,
                    GridColumns.Left,
                    Stack()
                        .Horizontal()
                        .WithChildren(
                            Button().Text("Count").OnClick(() => OnCountButtonClick()),
                            Button().Text("Sum").OnClick(() => OnSumButtonClick()),
                            Button().Text("Average").OnClick(() => OnAverageButtonClick()),
                            Button().Text("Output").OnClick(() => OnOutputButtonClick()))),
                Cell(GridRows.Middle,
                    GridColumns.Left,
                    SplitGrid()
                        .Horizontal()
                        .TopPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .BottomPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .WithTopPaneChild(dataSourceTextInput)
                        .WithBottomPaneChild(outputTextInput)));

    public UIToolView View => new(
        Grid()
            .Rows(
                (GridRows.Top, Auto),
                (GridRows.Middle, new UIGridLength(1, UIGridUnitType.Fraction)))
            .Columns((GridColumns.Left, new UIGridLength(1, UIGridUnitType.Fraction)))
            .Cells(
                Cell(
                    GridRows.Top,
                    GridColumns.Left,
                    Stack()
                        .Vertical()
                        .WithChildren(
                            DataSourceTypeSetting())),
                Cell(
                    GridRows.Middle,
                    GridColumns.Left,
                    SplitGrid()
                        .Vertical()
                        .LeftPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .RightPaneLength(new UIGridLength(1, UIGridUnitType.Fraction))
                        .WithLeftPaneChild(LeftPanelChild())
                        .WithRightPaneChild(RightPanelChild()))));

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

    private async ValueTask OnCountButtonClick()
    {
        var dataSourceType = _settingsProvider.GetSetting(dataSourceTypeOption);
        var dataSource = GetDataSource(dataSourceType);
        var command = $$"""
            {{dataSource}}
                .Count()
            """;
        scriptTextInput.Text(command);
        await Calc();
    }

    private async ValueTask OnSumButtonClick()
    {
        var dataSourceType = _settingsProvider.GetSetting(dataSourceTypeOption);
        string dataSource = GetDataSource(dataSourceType);
        var arg = dataSourceType == DataSourceType.Custom
            ? "r => r.Prop"
            : "";
        var command = $$"""
            {{dataSource}}
                .Sum({{arg}})
            """;
        scriptTextInput.Text(command);
        await Calc();
    }

    private async ValueTask OnAverageButtonClick()
    {
        var dataSourceType = _settingsProvider.GetSetting(dataSourceTypeOption);
        string dataSource = GetDataSource(dataSourceType);
        var arg = dataSourceType == DataSourceType.Custom
            ? "r => r.Prop"
            : "";
        var command = $$"""
            {{dataSource}}
                .Average({{arg}})
            """;
        scriptTextInput.Text(command);
        await Calc();
    }

    private async ValueTask OnOutputButtonClick()
    {
        var dataSourceType = _settingsProvider.GetSetting(dataSourceTypeOption);
        string dataSource = GetDataSource(dataSourceType);
        var arg = dataSourceType == DataSourceType.Custom
            ? "ToCsv"
            : "Join, \", \"";
        var command = $$"""
            {{dataSource}}
                .OutAll({{arg}});
            """;
        scriptTextInput.Text(command);
        await Calc();
    }

    private static string GetDataSource(DataSourceType dataSourceType)
    {
        return dataSourceType switch
        {
            DataSourceType.Integer => "ids",
            DataSourceType.Decimal => "dds",
            DataSourceType.Float => "fds",
            DataSourceType.String => "sds",
            DataSourceType.Custom => "Data<R>()",
            _ => ""
        };
    }

    private void OnCopyButtonClick()
    {
        var index = dataSourceTextInput.Text.IndexOfAny(['\r', '\n']);
        if (index >= 0)
        {
            var header = dataSourceTextInput.Text[0..index];
            customDefinitionTextInput.Text(string.Join(", ", header.Split(",").Select(s => $"string {s}")));
        }
    }

    private async ValueTask Calc()
    {
        try
        {

            var defs = _settingsProvider.GetSetting(customDataDefinitionOption);
            var isCustom = _settingsProvider.GetSetting(dataSourceTypeOption) == DataSourceType.Custom;
            var hasHeader = isCustom && _settingsProvider.GetSetting(hasHeaderOption);
            var headers = isCustom
                ? ScriptingHost.GetHeadersFromDefinition(defs) + Environment.NewLine
                : "";
            var additionalCode = isCustom && !string.IsNullOrEmpty(defs)
                ? $"partial record R({defs});" + Environment.NewLine
                : "";

            var index = dataSourceTextInput.Text.IndexOfAny(['\r', '\n']);

            var sourceText = !string.IsNullOrEmpty(dataSourceTextInput.Text) && hasHeader && index >= 0
                ? dataSourceTextInput.Text[index..].TrimStart(['\r', '\n'])
                : dataSourceTextInput.Text;
            var source = headers + sourceText;
            var cultureName = _settingsProvider.GetSetting(cultureInfoOption);
            CultureInfo cultureInfo;
            try
            {
                cultureInfo = new CultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            var output = new StringBuilder();
            using var host = new ScriptingHost(source, cultureInfo, output);

            var script = CSharpScript.Create(
                additionalCode + scriptTextInput.Text,
                ScriptOptions.Default
                    .WithReferences(
                        typeof(object).Assembly,
                        typeof(Enumerable).Assembly,
                        typeof(Library.Extensions).Assembly,
                        typeof(CsvReader).Assembly)
                    .WithImports(
                        "System",
                        "System.Collections.Generic",
                        "System.Globalization",
                        "System.Linq",
                        "CsvHelper",
                        "CsvHelper.Configuration.Attributes",
                        "DevToys.Extensions.CSharpCalculator.Library"),
                typeof(ScriptingHost));
            var context = await script.RunAsync(host);
            resultTextInput.Text(context.ReturnValue?.ToString() ?? "");
            outputTextInput.Text(output.ToString());
        }
        catch (Exception e)
        {
            resultTextInput.Text("E");
            outputTextInput.Text(e.Message);
        }
    }
}
