using Brest_Pekar;
using Brest_Pekar.Models;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Brest_Pekar.ViewModels
{
    class StoreViewModel : INotifyPropertyChanged
    {
        public static StoreViewModel Instance = null!;
        private Store? _selectedStore { get; set; }
        public ObservableCollection<Store> Stores { get; set; }
        public ObservableCollection<Bank> Banks { get; set; }
        public ObservableCollection<BankAccount> BankAccounts { get; set; }

        public StoreViewModel()
        {
            Instance = this;

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Stores.Load();
                Stores = new ObservableCollection<Store>(db.Stores.Include(x => x.iban));

                db.Banks.Load();
                Banks = new ObservableCollection<Bank>(db.Banks);

                db.BankAccounts.Load();
                BankAccounts = new ObservableCollection<BankAccount>(db.BankAccounts.Include(x => x.bank));
            }
        }

        public Store? SelectedStore
        {
            get
            {        
                return _selectedStore;
            }
            set
            {
                _selectedStore = value;
                OnPropertyChanged("SelectedStore");
            }
        }

        private Bank _selectedBank;
        public Bank SelectedBank
        {
            get
            {
                return _selectedBank;
            }
            set
            {
                _selectedBank = value;
                OnPropertyChanged("SelectedBank");
            }
        }

        private BankAccount _selectedBankAccount;
        public BankAccount SelectedBankAccount
        {
            get
            {
                return _selectedBankAccount;
            }
            set
            {
                _selectedBankAccount = value;
                OnPropertyChanged("SelectedBankAccount");
            }
        }

        private RelayCommand<BankAccount> _addBankAccountCommand = null!;
        public RelayCommand<BankAccount> AddBankAccountCommand => _addBankAccountCommand
            ?? (_addBankAccountCommand = new RelayCommand<BankAccount>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.BankAccounts.Load();
                    db.Banks.Load();

                    if (db.Banks.Count() == 0)
                    {
                        MainWindow.Instance.SendSnackMessage("Добавьте банк!", PackIconKind.Error);
                        return;
                    }

                    BankAccount bankAccount = new BankAccount { bank = db.Banks.First() };

                    db.BankAccounts.Attach(bankAccount);
                    db.BankAccounts.Add(bankAccount);
                    db.SaveChanges();

                    BankAccounts.Add(bankAccount);
                }
            }
        ));

        private RelayCommand<BankAccount> _removeBankAccountCommand = null!;
        public RelayCommand<BankAccount> RemoveBankAccountCommand => _removeBankAccountCommand
            ?? (_removeBankAccountCommand = new RelayCommand<BankAccount>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.BankAccounts.Load();

                    if (db.BankAccounts.Contains(obj))
                    {
                        db.BankAccounts.Remove(obj);
                        db.SaveChanges();
                    }

                    BankAccounts.Remove(obj);
                }
            }
        ));

        private RelayCommand<Bank> _addBankCommand = null!;
        public RelayCommand<Bank> AddBankCommand => _addBankCommand
            ?? (_addBankCommand = new RelayCommand<Bank>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Banks.Load();

                    Bank bank = new Bank { name = "Новый" };

                    db.Banks.Attach(bank);
                    db.Banks.Add(bank);
                    db.SaveChanges();

                    Banks.Add(bank);
                }
            }
        ));

        private RelayCommand<Bank> _removeBankCommand = null!;
        public RelayCommand<Bank> RemoveBankCommand => _removeBankCommand
            ?? (_removeBankCommand = new RelayCommand<Bank>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Banks.Load();

                    if (db.Banks.Contains(obj))
                    {
                        db.Banks.Remove(obj);
                        db.SaveChanges();
                    }

                    Banks.Remove(obj);
                }
            }
        ));

        private RelayCommand<Store> _addStoreCommand = null!;
        public RelayCommand<Store> AddStoreCommand => _addStoreCommand
            ?? (_addStoreCommand = new RelayCommand<Store>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Stores.Load();
                    db.BankAccounts.Load();

                    if (db.BankAccounts.Count() == 0)
                    {
                        MainWindow.Instance.SendSnackMessage("Добавьте новый счёт!", PackIconKind.Error);
                        return;
                    }

                    var store = new Store();
                    store.name = "Новый";
                    store.address = "-";
                    store.phone = "-";
                    store.iban = db.BankAccounts.First();

                    db.Stores.Attach(store);
                    db.Stores.Add(store);
                    db.SaveChanges();

                    Stores.Add(store);
                }
            }
        ));

        private RelayCommand<Store> _removeStoreCommand = null!;
        public RelayCommand<Store> RemoveStoreCommand => _removeStoreCommand
            ?? (_removeStoreCommand = new RelayCommand<Store>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Stores.Load();

                    if (db.Stores.Contains(obj))
                    {
                        db.Stores.Remove(obj);
                        db.SaveChanges();
                    }

                    Stores.Remove(obj);
                }
            }
        ));


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
