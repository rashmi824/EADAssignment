using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPut("adjust/{id}")]
        public ActionResult AdjustInventory(string id, [FromBody] AdjustmentRequest request)
        {
            bool adjusted = _inventoryService.AdjustInventory(id, request.Adjustment);
            if (adjusted)
            {
                return Ok("Inventory adjusted successfully.");
            }
            return BadRequest("Inventory adjustment failed due to validation constraints.");
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateInventory(Inventory inventory)
        {
            bool created = _inventoryService.CreateInventory(inventory);
            if (created)
            {
                return Ok("Inventory created successfully.");
            }
            return BadRequest("Inventory creation failed due to validation constraints.");
        }


        [HttpPut("update/{id}")]
        public ActionResult UpdateTrainSchedule(string id, Inventory updatedInventory)
        {
            bool updated = _inventoryService.UpdateInventory(id, updatedInventory);
            if (updated)
            {
                return Ok("Inventory updated successfully.");
            }
            return BadRequest("Inventory update failed due to reservation not found.");
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeleteInventory(string id)
        {
            bool canceled = _inventoryService.DeleteInventory(id);
            if (canceled)
            {
                return Ok("Inventory deleted successfully.");
            }
            return BadRequest("Inventory delete failed due to Profile not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<Inventory>> GetAllInventory()
        {
            List<Inventory> inventory = _inventoryService.GetAllInventory();
            return Ok(inventory);
        }

        [HttpGet("getLowInv")]
        public ActionResult<List<Inventory>> GetLowInventory()
        {
            List<Inventory> inventories = _inventoryService.GetLowInventory();
            if (inventories.Count != 0)
            {
                return Ok(inventories);
            }
            return NotFound("No inventories found matching the search term.");
        }

    }
}
