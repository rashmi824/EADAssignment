using Backend.Models;
using MongoDB.Driver;

namespace Backend.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Inventory> _inventoryCollection;


        public ProductService(IMongoDatabase database)
        {
            _productCollection = database.GetCollection<Product>("product");
            _inventoryCollection = database.GetCollection<Inventory>("inventory");
        }

        public bool CreateProduct(Product product, int stock)
        {
            using var session = _productCollection.Database.Client.StartSession();

            try
            {
                session.StartTransaction();

                // Check if the ProductID is already in use
                var existingProduct = _productCollection.Find(p => p.ProductID == product.ProductID).FirstOrDefault();
                if (existingProduct != null)
                {
                    // ProductID already exists, abort the transaction and return false
                    session.AbortTransaction();
                    Console.WriteLine("ProductID already exists.");
                    return false;
                }

                // Create the product
                Product newProduct = new Product
                {
                    ProductID = product.ProductID,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category,
                    Image = product.Image,
                    Status = 1
                };

                _productCollection.InsertOne(newProduct);

                // Create the inventory entry
                Inventory newInventory = new Inventory
                {
                    ProductID = newProduct.ProductID,
                    ProductName = newProduct.Name,
                    StockLevel = stock,
                    StockStatus = stock > 100 ? "In Stock" : "Low Stock"
                };

                _inventoryCollection.InsertOne(newInventory);

                session.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if something goes wrong
                session.AbortTransaction();
                Console.WriteLine(ex.Message);
                return false;
            }
        }



        public bool UpdateProduct(string id, Product updatedProduct)
        {
            var existingProduct = _productCollection
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (existingProduct != null)
            {
                updatedProduct.ID = id;
                _productCollection.ReplaceOne(r => r.ID == id, updatedProduct);
                return true;
            }

            return false;
        }

        public bool DeleteProduct(string id)
        {
            var productToDelete = _productCollection
                     .Find(r => r.ID == id)
                     .FirstOrDefault();

            if (productToDelete != null)
            {
                _productCollection.DeleteOne(r => r.ID == id);
                _inventoryCollection.DeleteOne(r => r.ProductID == productToDelete.ProductID);
                return true;
            }
            return false;

        }

        public List<Product> GetAllProducts()
        {
            return _productCollection.AsQueryable().ToList();
        }

        public List<Product> GetProductById(string id)
        {
            return _productCollection
                .Find(r => r.ID == id)
                .ToList();
        }


        public bool UpdateProductStatus(string id, int status)
        {
            var existingProduct = _productCollection
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (existingProduct != null)
            {
                var updateDefinition = Builders<Product>.Update
                    .Set(p => p.Status, status);

                var updateResult = _productCollection.UpdateOne(r => r.ID == id, updateDefinition);

                return true;
            }

            return false;
        }

        public List<Product> GetProducts()
        {
            //Get all product IDs from the inventory collection
            var inventoryProductIDs = _inventoryCollection.AsQueryable()
                                         .Select(inventory => inventory.ProductID)
                                         .ToList();

            //Get products that are NOT in the inventory collection
            var availableProducts = _productCollection.AsQueryable()
                                       .Where(product => !inventoryProductIDs.Contains(product.ProductID))
                                       .ToList();

            return availableProducts;
        }



    }
}
