using Brest_Pekar;
using Brest_Pekar.Models;
using GalaSoft.MvvmLight.Command;
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
    class ProductTypeViewModel : INotifyPropertyChanged
    {
        private ProductType? _selectedProductType { get; set; }
        public ObservableCollection<ProductType> ProductTypes { get; set; }

        public ProductTypeViewModel()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Product_Types.Load();
                ProductTypes = new ObservableCollection<ProductType>(db.Product_Types);
            }
        }

        public ProductType SelectedProductType
        {
            get 
            {
                return _selectedProductType; 
            }
            set
            {
                _selectedProductType = value;
                OnPropertyChanged("SelectedProductType");
            }
        }

        private RelayCommand<ProductType> _addProductTypeCommand = null!;
        public RelayCommand<ProductType> AddProductTypeCommand => _addProductTypeCommand
            ?? (_addProductTypeCommand = new RelayCommand<ProductType>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Product_Types.Load();

                    ProductType productType = new ProductType();
                    productType.name = "Новый";
                    productType.price = 5;

                    db.Product_Types.Add(productType);
                    db.SaveChanges();

                    ProductTypes.Add(productType);
                }
            }
        ));

        private RelayCommand<ProductType> _removeProductTypeCommand = null!;
        public RelayCommand<ProductType> RemoveProductTypeCommand => _removeProductTypeCommand
            ?? (_removeProductTypeCommand = new RelayCommand<ProductType>(obj =>
            {
                using (var db = new ApplicationContext())
                {
                    db.Product_Types.Load();

                    if (db.Product_Types.Contains(obj))
                    {
                        db.Product_Types.Remove(obj);
                        db.SaveChanges();
                    }

                    ProductTypes.Remove(obj);
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
