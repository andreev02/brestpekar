using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Brest_Pekar.Models
{
    class ProductType : INotifyPropertyChanged
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; } = null!;
        public int price { get; set; }
        public ICollection<Product> Products { get; } = new List<Product>();
        public ICollection<Order> Orders { get; } = new List<Order>();
        public ICollection<RealesedOrder> RealesedOrders { get; } = new List<RealesedOrder>();

        [NotMapped]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        [NotMapped]
        public string Name
        {
            get { return name; }
            set
            {
                if (value.Length == 0)
                {
                    value = name;
                }

                name = value;
                OnPropertyChanged("Name");
            }
        }

        [NotMapped]
        public int Price
        {
            get { return price; }
            set
            {
                if (value < 0)
                {
                    value = value * -1;
                }

                price = value;
                OnPropertyChanged("Price");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Product_Types.Attach(this);
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
