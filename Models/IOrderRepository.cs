namespace TechnoShop.Models
{
  public interface IOrderRepository
  {
    IQueryable<Order> Orders { get; }
    void SaveOrder(Order order);
    void DeleteOrder(int orderID);
    Order? GetOrder(int orderID);
  }
}