using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListService : IGroceryListService
    {
        private readonly IGroceryListRepository _groceryRepository;
        private readonly List<GroceryList> _temporaryLists = new();
        private int _tempIdCounter = -1; // Negatieve IDs voor temp lists

        public GroceryListService(IGroceryListRepository groceryRepository)
        {
            _groceryRepository = groceryRepository;
        }

        public List<GroceryList> GetAll()
        {
            var dbLists = _groceryRepository.GetAll();
            var allLists = new List<GroceryList>(dbLists);
            allLists.AddRange(_temporaryLists);
            return allLists;
        }

        public GroceryList Add(GroceryList item)
        {
            item.Id = _tempIdCounter--;
            _temporaryLists.Add(item);
            return item;
        }

        public GroceryList? Delete(GroceryList item)
        {
            throw new NotImplementedException();
        }

        public GroceryList? Get(int id)
        {
            if (id < 0)
            {
                return _temporaryLists.FirstOrDefault(x => x.Id == id);
            }
            return _groceryRepository.Get(id);
        }

        public GroceryList? Update(GroceryList item)
        {
            if (item.Id < 0)
            {
                var existing = _temporaryLists.FirstOrDefault(x => x.Id == item.Id);
                if (existing != null)
                {
                    existing.Name = item.Name;
                    existing.Date = item.Date;
                    existing.Color = item.Color;
                    return existing;
                }
                return null;
            }
            return _groceryRepository.Update(item);
        }
    }
}