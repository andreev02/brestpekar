using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Brest_Pekar.Models
{
    class Bank : INotifyPropertyChanged
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; } = null!;
        public ICollection<BankAccount> BankAccounts { get; } = new List<BankAccount>();

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
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Banks.Update(this);
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
