using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Brest_Pekar.Models
{
    class BankAccount : INotifyPropertyChanged
    {
        [Key]
        public int id { get; set; }
        [AllowNull]
        public string? iban { get; set; }

        public int bankid { get; set; }
        public Bank bank { get; set; }

        public ICollection<Store> Stores { get; } = new List<Store>();

        [NotMapped]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        [NotMapped]
        public string IBAN
        {
            get
            {
                return iban;
            }
            set
            {
                
                iban = value;
                OnPropertyChanged("IBAN");
            }
        }

        [NotMapped]
        public Bank Bank
        {
            get
            {
                return bank;
            }
            set 
            {
                bank = value;
                OnPropertyChanged("Bank");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

            using (ApplicationContext db = new ApplicationContext())
            {
                db.BankAccounts.Update(this);
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
