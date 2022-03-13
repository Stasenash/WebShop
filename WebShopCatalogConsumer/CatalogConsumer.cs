using MassTransit;
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

        public CatalogConsumer(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task Consume(ConsumeContext<ICategoryCreated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
        }

        public async Task Consume(ConsumeContext<ICategoryUpdated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
        }

        public async Task Consume(ConsumeContext<ICategoryDeleted> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
        }

        public async Task Consume(ConsumeContext<IItemAdded> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
        }

        public async Task Consume(ConsumeContext<IItemUpdated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
        }

        public async Task Consume(ConsumeContext<IItemDeleted> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
        }
    }
}
