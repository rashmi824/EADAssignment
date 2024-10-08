using Backend.Models;
using MongoDB.Driver;

namespace Backend.Services
{
    public class InventoryService
    {
        private readonly IMongoCollection<Inventory> _inventoryCollection;


        public InventoryService(IMongoDatabase database)
        {
            _inventoryCollection = database.GetCollection<Inventory>("inventory");
        }

        public bool AdjustInventory(string inventoryId, int quantity)
        {
            try
            {
                var inventory = _inventoryCollection.Find(i => i.ID == inventoryId).FirstOrDefault();

                if (inventory == null)
                {
                    return false;
                }

                // Update stock level
                inventory.StockLevel += quantity;

                // Update stock status based on new stock level
                if (inventory.StockLevel < 1)
                {
                    inventory.StockStatus = "Out of Stock";
                }
                else if (inventory.StockLevel < 100)
                {
                    inventory.StockStatus = "Low Stock";
                }
                else
                {
                    inventory.StockStatus = "In Stock";
                }

                // Replace the updated inventory record in the collection
                _inventoryCollection.ReplaceOne(i => i.ID == inventoryId, inventory);

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception if necessary
                return false;
            }
        }

        public bool CreateInventory(Inventory inventory)
        {
            try
            {
                Inventory newinventory = new Inventory
                {
                    ProductID = inventory.ProductID,
                    ProductName = inventory.ProductName,
                    StockLevel = inventory.StockLevel,
                    StockStatus = inventory.StockStatus
                };
                _inventoryCollection.InsertOne(newinventory);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateInventory(string id, Inventory updatedInventory)
        {
            var existingInventory = _inventoryCollection
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (existingInventory != null)
            {
                updatedInventory.ID = id;
                _inventoryCollection.ReplaceOne(r => r.ID == id, updatedInventory);
                return true;
            }

            return false;
        }

        public bool DeleteInventory(string id)
        {
            var inventoryToDelete = _inventoryCollection
            .Find(r => r.ID == id)
            .FirstOrDefault();

            if (inventoryToDelete != null)
            {
                //Logic to make sure that there is no pending orders
                //if ()
                //{
                _inventoryCollection.DeleteOne(r => r.ID == id);
                return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            return false;
        }

        public List<Inventory> GetAllInventory()
        {
            return _inventoryCollection.AsQueryable().ToList();
        }

        public List<Inventory> GetLowInventory()
        {
            return _inventoryCollection
                .Find(r => r.StockLevel < 100)
                .ToList();
        }


    }
}
