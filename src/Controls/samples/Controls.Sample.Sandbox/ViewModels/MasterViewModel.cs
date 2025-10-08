using CommunityToolkit.Mvvm.ComponentModel;
using MauiClient.Models;
using MauiClient.Services;
using System.Collections.ObjectModel;

namespace MauiClient.ViewModels;

public partial class MasterViewModel : ObservableObject
{
    private readonly MonkeyService service;

    public MasterViewModel(MonkeyService service)
    {
        this.service = service;
    }


    public ObservableCollection<Monkey> Monkeys { get; } = new();


    public async Task ReloadAsync()
    {
        Monkeys.Clear();

        var monkeys = await service.GetMonkeys();

        foreach (var monkey in monkeys ?? Enumerable.Empty<Monkey>())
            Monkeys.Add(monkey);
    }
}
