using MassTransit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopCatalogConsumer.Db;
using WebShopContracts;

namespace WebShopCatalogConsumer
{
    public class CatalogConsumer : 
        IConsumer<ICategoryCreated>, 
        IConsumer<ICategoryUpdated>, 
        IConsumer<ICategoryDeleted>,
        IConsumer<IItemAdded>,
        IConsumer<IItemUpdated>,
        IConsumer<IItemDeleted>
    {
        private readonly CategoryService _categoryService;
        private readonly ItemService _itemService;

        public CatalogConsumer(CategoryService categoryService, ItemService itemService)
        {
            _categoryService = categoryService;
            _itemService = itemService;
        }

        public async Task Consume(ConsumeContext<ICategoryCreated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
            var category = new Category
            {
                Id = context.Message.Id,
                Name = context.Message.Name,
                ParentId = context.Message.ParentId
            };

            _categoryService.Create(category);
        }

        public async Task Consume(ConsumeContext<ICategoryUpdated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");

            _categoryService.Update(new Category
            {
                Id = context.Message.Id,
                Name = context.Message.Name,
                ParentId = context.Message.ParentId
            });
        }

        public async Task Consume(ConsumeContext<ICategoryDeleted> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
            _categoryService.Remove(context.Message.Id);
        }

        public async Task Consume(ConsumeContext<IItemAdded> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
            _itemService.Create(context.Message.CategoryId, new Item
            {
                Id = context.Message.Id,
                Price = context.Message.Price,
                Name = context.Message.Name,
                ImageUrl = context.Message.ImageUrl
            });
        }

        public async Task Consume(ConsumeContext<IItemUpdated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
            _itemService.Update(context.Message.CategoryId, new Item
            {
                Id = context.Message.Id,
                Price = context.Message.Price,
                Name = context.Message.Name,
                ImageUrl = context.Message.ImageUrl
            });
        }

        public async Task Consume(ConsumeContext<IItemDeleted> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
            _itemService.Remove(context.Message.CategoryId, context.Message.Id);
        }
    }
}
