using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeCollaboration.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public float UnitOrderItemPrice { get; set; }

        [Required]
        public float TotalPrice { get; set; }


        //one order item belongs to one order

        [ForeignKey("Orders")]

        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        //one order item contains one menu item

        [ForeignKey("MenuItems")]

        public int MenuItemId { get; set; }

        public virtual MenuItem MenuItem { get; set; }

    }

    public class OrderItemDto
    {
        [Key]
        public int OrderItemId { get; set; }
        public int MenuItemId { get; set; }
        public int OrderId { get; set; }

        public string MenuItemName { get; set; }

        public float UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public float TotalPrice { get; set; }


    }


    public class AUOrderItemDto
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public float UnitOrderItemPrice { get; set; }

        [Required]
        public float TotalPrice { get; set; }

        public int MenuItemId { get; set; }

        public int OrderId { get; set; }

    }

    public class CustomerOrderItemDto
    {
        public string CustomerName { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
    }

}
