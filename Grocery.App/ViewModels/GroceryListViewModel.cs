using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        [ObservableProperty]
        Client client;

        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel global)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            GroceryLists = new(_groceryListService.GetAll());
            Client = global.Client;
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }

        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            if (Client.Role == Role.Admin) await Shell.Current.GoToAsync(nameof(BoughtProductsView), true);
        }

        [RelayCommand]
        public async Task AddGroceryList()
        {
            string name = await Shell.Current.DisplayPromptAsync(
                "Nieuwe lijst",
                "Naam van de boodschappenlijst:",
                "OK",
                "Annuleer",
                placeholder: "Bijv. Weekend boodschappen",
                maxLength: 80);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var newList = new GroceryList(
                    id: 0, // Wordt overschreven in service
                    name: name,
                    date: DateOnly.FromDateTime(DateTime.Today),
                    color: "#FF6A00", // Default kleur
                    clientId: Client.Id
                );

                _groceryListService.Add(newList);
                RefreshLists();
            }
        }

        private void RefreshLists()
        {
            GroceryLists.Clear();
            foreach (var list in _groceryListService.GetAll())
            {
                GroceryLists.Add(list);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            RefreshLists();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }
    }
}