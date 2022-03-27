using MassTransit;
using WebShopBasketConsumer.Db;
using WebShopContracts;

namespace WebShopBasketConsumer
{
    public class BasketConsumer :
        IConsumer<IBasketItemUpdated>,
        IConsumer<IBasketItemDeleted>
    {
        private readonly BasketService _itemService;

        public BasketConsumer(BasketService itemService)
        {
            _itemService = itemService;
        }

        public async Task Consume(ConsumeContext<IBasketItemUpdated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
            await _itemService.UpdateItems(new Item
            {
                Id = context.Message.Id,
                Price = context.Message.Price,
                Name = context.Message.Name
            });
        }

        public async Task Consume(ConsumeContext<IBasketItemDeleted> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");
            await _itemService.DeleteItems(context.Message.Id);
        }
    }
}
