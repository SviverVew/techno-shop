using Microsoft.EntityFrameworkCore;
namespace TechnoShop.Models
{
  public class EFOrderRepository : IOrderRepository
  {
    private readonly TechnoShopDbContext context;
    
    public EFOrderRepository(TechnoShopDbContext ctx)
    {
      context = ctx;
    }
    
    public IQueryable<Order> Orders => context.Orders;
    
    public void SaveOrder(Order order)
    {
      context.AttachRange(order.Lines.Select(l => l.Product));
      if (order.OrderID == 0)
      {
        context.Orders.Add(order);
      }
      context.SaveChanges();
    }
    
    public void DeleteOrder(int orderID)
    {
      var order = context.Orders.Find(orderID);
      if (order != null)
      {
        context.Orders.Remove(order);
        context.SaveChanges();
      }
    }
    
    public Order? GetOrder(int orderID)
    {
      return context.Orders.FirstOrDefault(o => o.OrderID == orderID);
    }
  }
}