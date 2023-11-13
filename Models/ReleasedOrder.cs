using Brest_Pekar.Models;
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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Brest_Pekar.Models
{
    class RealesedOrder : INotifyPropertyChanged
    {
        [Key]
        public int id { get; set; }

        [AllowNull]
        public int? storeid { get; set; }
        public Store? store { get; set; }

        [AllowNull]
        public int? product_typeid { get; set; }
        public ProductType? product_type { get; set; }

        public DateTime date { get; set; }
        public int count { get; set; }
        public int money { get; set;}


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
        public Store Store
        {
            get 
            {
                return store;
            }
            set
            {
                store = value;
                OnPropertyChanged("Store");
            }
        }

        [NotMapped]
        public ProductType Product_Type
        {
            get { return product_type; }
            set
            {
                product_type = value;
                OnPropertyChanged("Product_Type");
            }
        }

        [NotMapped]
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        [NotMapped]
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }

        [NotMapped]
        public int Money
        {
            get { return money; }
            set
            {
                money = value;
                OnPropertyChanged("Money");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Realesed_Orders.Update(this);
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
