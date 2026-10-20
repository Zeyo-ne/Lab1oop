using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

/// <summary>
/// Пример юнит тестов реализующих успешные сценарии
/// </summary>
public class DummyExampleTests
{
    [Fact]
    public void Tritanium_Instance_CanBeCreated()
    {
        // Arrange:
        const string expectedToStringResult = "Tritanium { }";

        // Act:
        var tritanium = new Tritanium();

        // ToString у Record'ов возвращает строку вида
        // <record type name> { <property name> = <value>, <property name> = <value>, ...}
        string actualToStringResult = tritanium.ToString();

        // Assert:
        Assert.Equal(expectedToStringResult, actualToStringResult);
    }
}