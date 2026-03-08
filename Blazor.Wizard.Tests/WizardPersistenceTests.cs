using Blazor.Wizard.Core;
using Blazor.Wizard.Extensions;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.Persistence;
using Blazor.Wizard.ViewModels;
using FluentAssertions;

namespace Blazor.Wizard.Tests;

public class WizardPersistenceTests
{
    private class TestModel : IWizardDataModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private class TestResult
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private class TestModelBuilder : IWizardModelBuilder<TestResult>
    {
        public TestResult Build(IWizardData data)
        {
            data.TryGet<TestModel>(out var model);
            return new TestResult { Name = model?.Name ?? string.Empty, Age = model?.Age ?? 0 };
        }
    }

    [Fact]
    public async Task SaveStateAsync_SavesCurrentStepIndex()
    {
        var storage = new MemoryWizardStateStorage();
        var viewModel = new WizardViewModel<IWizardStep, WizardData, TestResult>(new TestModelBuilder());
        viewModel.Initialize(new[] { () => new BaseStepLogic<TestModel>(typeof(TestModel)) });
        await viewModel.StartAsync();

        await viewModel.SaveStateAsync("test-key", storage);

        var json = await storage.LoadAsync("test-key");
        json.Should().NotBeNull();
        json.Should().Contain("\"CurrentStepIndex\":0");
    }

    [Fact]
    public async Task LoadStateAsync_RestoresStepIndex()
    {
        var storage = new MemoryWizardStateStorage();
        var viewModel = new WizardViewModel<IWizardStep, WizardData, TestResult>(new TestModelBuilder());
        viewModel.Initialize(new[] 
        { 
            () => new BaseStepLogic<TestModel>(typeof(TestModel)),
            () => new BaseStepLogic<TestModel>(typeof(TestModel))
        });
        await viewModel.StartAsync();
        await viewModel.NextAsync();
        await viewModel.SaveStateAsync("test-key", storage);

        var newViewModel = new WizardViewModel<IWizardStep, WizardData, TestResult>(new TestModelBuilder());
        newViewModel.Initialize(new[] 
        { 
            () => new BaseStepLogic<TestModel>(typeof(TestModel)),
            () => new BaseStepLogic<TestModel>(typeof(TestModel))
        });
        var loadedIndex = await newViewModel.LoadStateAsync("test-key", storage);

        loadedIndex.Should().Be(1);
        newViewModel.Flow?.Index.Should().Be(1);
    }

    [Fact]
    public async Task SaveAndLoadState_PreservesWizardData()
    {
        var storage = new MemoryWizardStateStorage();
        var viewModel = new WizardViewModel<IWizardStep, WizardData, TestResult>(new TestModelBuilder());
        viewModel.Initialize(new[] { () => new BaseStepLogic<TestModel>(typeof(TestModel)) });
        await viewModel.StartAsync();
        
        viewModel.Data.Set(new TestModel { Name = "John", Age = 30 });
        await viewModel.SaveStateAsync("test-key", storage);

        var newViewModel = new WizardViewModel<IWizardStep, WizardData, TestResult>(new TestModelBuilder());
        newViewModel.Initialize(new[] { () => new BaseStepLogic<TestModel>(typeof(TestModel)) });
        var loadedIndex = await newViewModel.LoadStateAsync("test-key", storage);

        loadedIndex.Should().Be(0);
        newViewModel.Data.TryGet<TestModel>(out var model).Should().BeTrue();
        model.Name.Should().Be("John");
        model.Age.Should().Be(30);
    }

    [Fact]
    public async Task LoadStateAsync_ReturnsFalse_WhenNoStateExists()
    {
        var storage = new MemoryWizardStateStorage();
        var viewModel = new WizardViewModel<IWizardStep, WizardData, TestResult>(new TestModelBuilder());
        viewModel.Initialize(new[] { () => new BaseStepLogic<TestModel>(typeof(TestModel)) });

        var loadedIndex = await viewModel.LoadStateAsync("non-existent-key", storage);

        loadedIndex.Should().Be(-1);
    }

    [Fact]
    public async Task ClearStateAsync_RemovesState()
    {
        var storage = new MemoryWizardStateStorage();
        var viewModel = new WizardViewModel<IWizardStep, WizardData, TestResult>(new TestModelBuilder());
        viewModel.Initialize(new[] { () => new BaseStepLogic<TestModel>(typeof(TestModel)) });
        await viewModel.StartAsync();
        await viewModel.SaveStateAsync("test-key", storage);

        await WizardPersistenceExtensions.ClearStateAsync("test-key", storage);

        var json = await storage.LoadAsync("test-key");
        json.Should().BeNull();
    }

    [Fact]
    public async Task MemoryStorage_IsThreadSafe()
    {
        var storage = new MemoryWizardStateStorage();
        var tasks = new List<Task>();

        for (int i = 0; i < 100; i++)
        {
            var key = $"key-{i}";
            tasks.Add(Task.Run(async () =>
            {
                await storage.SaveAsync(key, $"value-{key}");
                var value = await storage.LoadAsync(key);
                value.Should().Be($"value-{key}");
            }));
        }

        await Task.WhenAll(tasks);
    }
}
