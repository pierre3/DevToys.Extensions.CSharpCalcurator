using DevToys.Api;
using System.ComponentModel.Composition;

namespace DevToys.Extensions.CSharpCalculator;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(CSharpCalculatorResourceAssemblyIdentifier))]
internal sealed class CSharpCalculatorResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        return ValueTask.FromResult<FontDefinition[]>([]);
    }
}