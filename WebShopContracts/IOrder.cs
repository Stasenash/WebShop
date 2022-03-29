using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopContracts
{
    public interface IOrderCreated
    {
        public User User { get; set; }
        public List<int> Items { get; set; }
    }

    public interface IOrderStatusChanged
    {
        public User User { get; set; }
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        AdminApproved,
        AdminCancelled,
        UserCancelled,
        Completed
    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
