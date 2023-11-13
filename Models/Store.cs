using Brest_Pekar;
using Brest_Pekar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Brest_Pekar.Models
{
    class Store : INotifyPropertyChanged
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string address { get; set; } = null!;
        public string phone { get; set; } = null!;
        [AllowNull]
        public int? bankaccountid { get; set; }
        public BankAccount? iban { get; set; }
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
        public string Address
        {
            get { return address; }
            set
            {
                if (value.Length == 0)
                {
                    value = address;
                }

                address = value;
                OnPropertyChanged("Address");
            }
        }

        [NotMapped]
        public string Phone
        {
            get { return phone; }
            set
            {
                if (value.Length == 0)
                {
                    value = phone;
                }

                phone = value;
                OnPropertyChanged("Phone");
            }
        }

        [NotMapped]
        public BankAccount BankAccount
        {
            get { return iban; }
            set
            {
                iban = value;
                OnPropertyChanged("BankAccount");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Stores.Attach(this);
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
