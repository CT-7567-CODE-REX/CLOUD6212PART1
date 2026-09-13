using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using CLOUD.POE._1.Models;
using Microsoft.Extensions.Configuration;

namespace CLOUD.POE._1.Services
{
    public class MenuStorageService
    {
        private readonly TableClient _tableClient;

        public MenuStorageService(IConfiguration configuration)
        {
            // Read the Azurite/Table Storage connection string.
            string connectionString =
                configuration["MenuStorageConnection"]
                ?? "UseDevelopmentStorage=true";

            // Connect to the MenuItems table.
            _tableClient = new TableClient(
                connectionString,
                "MenuItems"
            );
        }

        private async Task EnsureTableExistsAsync()
        {
            // Create the table if it does not already exist.
            await _tableClient.CreateIfNotExistsAsync();
        }

        public async Task AddMenuItemAsync(MenuItemEntity entity)
        {
            await EnsureTableExistsAsync();

            // Insert a new menu item.
            await _tableClient.AddEntityAsync(entity);
        }

        public async Task<List<MenuItemEntity>> GetAllAsync()
        {
            await EnsureTableExistsAsync();

            List<MenuItemEntity> items = new();

            // Read every entity from MenuItems.
            await foreach (
                MenuItemEntity entity in
                _tableClient.QueryAsync<MenuItemEntity>())
            {
                items.Add(entity);
            }

            return items;
        }

        public async Task<List<MenuItemEntity>> GetByCategoryAsync(
            string category)
        {
            await EnsureTableExistsAsync();

            List<MenuItemEntity> items = new();

            // Category is stored as the PartitionKey.
            await foreach (
                MenuItemEntity entity in
                _tableClient.QueryAsync<MenuItemEntity>(
                    item => item.PartitionKey == category))
            {
                items.Add(entity);
            }

            return items;
        }

        public async Task<MenuItemEntity?> GetAsync(
            string category,
            string id)
        {
            await EnsureTableExistsAsync();

            try
            {
                // Find one item using PartitionKey and RowKey.
                Response<MenuItemEntity> response =
                    await _tableClient.GetEntityAsync<MenuItemEntity>(
                        category,
                        id
                    );

                return response.Value;
            }
            catch (RequestFailedException exception)
                when (exception.Status == 404)
            {
                // Return null when the item does not exist.
                return null;
            }
        }

        public async Task UpdateAsync(MenuItemEntity entity)
        {
            await EnsureTableExistsAsync();

            // Replace the stored entity with the updated version.
            await _tableClient.UpdateEntityAsync(
                entity,
                ETag.All,
                TableUpdateMode.Replace
            );
        }

        public async Task<bool> DeleteAsync(
            string category,
            string id)
        {
            await EnsureTableExistsAsync();

            MenuItemEntity? existingItem =
                await GetAsync(category, id);

            if (existingItem == null)
            {
                return false;
            }

            // Delete the item using its category and SKU.
            await _tableClient.DeleteEntityAsync(
                category,
                id
            );

            return true;
        }
    }
}