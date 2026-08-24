using AiBusinessWorkflow.Api.Data;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Data;

public class SampleDataGeneratorTests
{
    [Fact]
    public void GetAll_ShouldReturnNonEmptyList()
    {
        var samples = SampleDataGenerator.GetAll();
        samples.Should().NotBeEmpty();
    }

    [Fact]
    public void GetAll_ShouldReturnAtLeast5Samples()
    {
        var samples = SampleDataGenerator.GetAll();
        samples.Should().HaveCountGreaterOrEqualTo(5);
    }

    [Fact]
    public void GetAll_ShouldReturnUniqueIds()
    {
        var samples = SampleDataGenerator.GetAll();
        var ids = samples.Select(s => s.Id).ToList();
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GetAll_AllSamplesShouldHaveNonEmptyFields()
    {
        var samples = SampleDataGenerator.GetAll();
        foreach (var sample in samples)
        {
            sample.Id.Should().NotBeNullOrWhiteSpace();
            sample.Name.Should().NotBeNullOrWhiteSpace();
            sample.Description.Should().NotBeNullOrWhiteSpace();
            sample.InputData.Should().NotBeNullOrWhiteSpace();
            sample.Goal.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void GetByIndex_WithValidIndex_ShouldReturnSample()
    {
        var sample = SampleDataGenerator.GetByIndex(0);
        sample.Should().NotBeNull();
        sample!.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetByIndex_WithLastIndex_ShouldReturnSample()
    {
        var count = SampleDataGenerator.GetAll().Count;
        var sample = SampleDataGenerator.GetByIndex(count - 1);
        sample.Should().NotBeNull();
    }

    [Fact]
    public void GetByIndex_WithNegativeIndex_ShouldReturnNull()
    {
        var sample = SampleDataGenerator.GetByIndex(-1);
        sample.Should().BeNull();
    }

    [Fact]
    public void GetByIndex_WithOutOfRangeIndex_ShouldReturnNull()
    {
        var count = SampleDataGenerator.GetAll().Count;
        var sample = SampleDataGenerator.GetByIndex(count);
        sample.Should().BeNull();
    }

    [Fact]
    public void GetByIndex_WithLargeIndex_ShouldReturnNull()
    {
        var sample = SampleDataGenerator.GetByIndex(999);
        sample.Should().BeNull();
    }

    [Fact]
    public void GetAll_ShouldReturnSameInstanceOnMultipleCalls()
    {
        var first = SampleDataGenerator.GetAll();
        var second = SampleDataGenerator.GetAll();
        first.Should().BeSameAs(second);
    }
}
