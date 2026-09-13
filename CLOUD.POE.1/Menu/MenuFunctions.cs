
using Azure;
using CLOUD.POE._1.Models;
using CLOUD.POE._1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CLOUD.POE._1.Functions.Menu
{
    public class MenuFunctions
    {
        private readonly MenuStorageService _menuStorageService;

        public MenuFunctions(MenuStorageService menuStorageService)
        {
            _menuStorageService = menuStorageService;
        }

        [Function("CreateMenuItem")]
        public async Task<IActionResult> CreateMenuItem(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "menu")]
            HttpRequest request)
        {
            // Read the JSON request body.
            CreateMenuItemRequest? model =
                await request.ReadFromJsonAsync<CreateMenuItemRequest>();

            if (model == null)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message = "A menu item request body is required."
                    }
                );
            }

            // These fields are required to create an item.
            if (string.IsNullOrWhiteSpace(model.Category) ||
                string.IsNullOrWhiteSpace(model.Id) ||
                string.IsNullOrWhiteSpace(model.Name))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message = "Category, id and name are required."
                    }
                );
            }

            if (model.Price < 0)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message = "Price cannot be negative."
                    }
                );
            }

            // Convert the request into an Azure Table entity.
            MenuItemEntity entity = new()
            {
                PartitionKey = model.Category.Trim(),
                RowKey = model.Id.Trim(),
                Name = model.Name.Trim(),
                Description = model.Description?.Trim() ?? string.Empty,
                Price = model.Price,
                IsAvailable = model.IsAvailable
            };

            try
            {
                await _menuStorageService.AddMenuItemAsync(entity);
            }
            catch (RequestFailedException exception)
                when (exception.Status == 409)
            {
                return new ConflictObjectResult(
                    new
                    {
                        message =
                            "A menu item with this category and ID already exists."
                    }
                );
            }

            return new ObjectResult(
                new
                {
                    message = "Menu item created successfully.",
                    item = ConvertToResponse(entity)
                })
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        [Function("GetAllMenuItems")]
        public async Task<IActionResult> GetAllMenuItems(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "menu")]
            HttpRequest request)
        {
            List<MenuItemEntity> entities =
                await _menuStorageService.GetAllAsync();

            List<MenuItemResponse> items =
                entities
                    .Select(ConvertToResponse)
                    .ToList();

            return new OkObjectResult(items);
        }

        [Function("GetMenuItemsByCategory")]
        public async Task<IActionResult> GetMenuItemsByCategory(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "menu/category/{category}")]
            HttpRequest request,
            string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message = "A category is required."
                    }
                );
            }

            List<MenuItemEntity> entities =
                await _menuStorageService
                    .GetByCategoryAsync(category);

            List<MenuItemResponse> items =
                entities
                    .Select(ConvertToResponse)
                    .ToList();

            return new OkObjectResult(items);
        }

        [Function("UpdateMenuItem")]
        public async Task<IActionResult> UpdateMenuItem(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "put",
                Route = "menu/{category}/{id}")]
            HttpRequest request,
            string category,
            string id)
        {
            // Read the fields that should be changed.
            UpdateMenuItemRequest? model =
                await request.ReadFromJsonAsync<UpdateMenuItemRequest>();

            if (model == null)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message = "An update request body is required."
                    }
                );
            }

            if (model.Price == null &&
                model.IsAvailable == null)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message =
                            "Provide a price, availability status, or both."
                    }
                );
            }

            if (model.Price.HasValue &&
                model.Price.Value < 0)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        message = "Price cannot be negative."
                    }
                );
            }

            MenuItemEntity? existingItem =
                await _menuStorageService.GetAsync(
                    category,
                    id
                );

            if (existingItem == null)
            {
                return new NotFoundObjectResult(
                    new
                    {
                        message = "Menu item not found."
                    }
                );
            }

            if (model.Price.HasValue)
            {
                existingItem.Price = model.Price.Value;
            }

            if (model.IsAvailable.HasValue)
            {
                existingItem.IsAvailable =
                    model.IsAvailable.Value;
            }

            await _menuStorageService.UpdateAsync(existingItem);

            return new OkObjectResult(
                new
                {
                    message = "Menu item updated successfully.",
                    item = ConvertToResponse(existingItem)
                }
            );
        }

        [Function("DeleteMenuItem")]
        public async Task<IActionResult> DeleteMenuItem(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "delete",
                Route = "menu/{category}/{id}")]
            HttpRequest request,
            string category,
            string id)
        {
            bool deleted =
                await _menuStorageService.DeleteAsync(
                    category,
                    id
                );

            if (!deleted)
            {
                return new NotFoundObjectResult(
                    new
                    {
                        message = "Menu item not found."
                    }
                );
            }

            return new OkObjectResult(
                new
                {
                    message = "Menu item deleted successfully."
                }
            );
        }

        private static MenuItemResponse ConvertToResponse(
            MenuItemEntity entity)
        {
            // Convert Azure field names into API-friendly names.
            return new MenuItemResponse
            {
                Category = entity.PartitionKey,
                Id = entity.RowKey,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                IsAvailable = entity.IsAvailable
            };
        }
    }
}
