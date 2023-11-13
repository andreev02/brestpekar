using Brest_Pekar;
using Brest_Pekar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class Product : INotifyPropertyChanged
    {
        [Key]
        public int id { get; set; }
        public int product_typeid { get; set; }
        public ProductType? product_type { get; set; }
        public DateTime date_of_production { get; set; }
        public ICollection<Order> orders { get; } = new List<Order>();
        public ICollection<Supply> supplies { get; set; } = new List<Supply>();

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
        public ProductType Product_Type
        {
            get
            {
                return product_type;
            }
            set
            {
                product_type = value; 
                OnPropertyChanged("Product_Type");
            }
        }

        [NotMapped]
        public DateTime Date_Of_Production
        { 
            get { return date_of_production; }
            set
            {
                date_of_production = value;
                OnPropertyChanged("Date_Of_Production");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Update(this);
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
