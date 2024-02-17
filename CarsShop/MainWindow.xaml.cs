using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CarsShop.Models;

namespace CarsShop
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _userCountText;
        private string _carCountText;
        private readonly ApplicationDbContext _dbContext;
        private User _selectedUser;

        public event PropertyChangedEventHandler PropertyChanged;

        public string UserCountText
        {
            get { return _userCountText; }
            set
            {
                _userCountText = value;
                OnPropertyChanged(nameof(UserCountText));
            }
        }

        public string CarCountText
        {
            get { return _carCountText; }
            set
            {
                _carCountText = value;
                OnPropertyChanged(nameof(CarCountText));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _dbContext = new ApplicationDbContext();
            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await LoadUsersAsync();
            UpdateCounts();
        }

        private async Task LoadUsersAsync()
        {
            var users = await _dbContext.Users.ToListAsync();
            userDataGrid.ItemsSource = users;
            UserCountText = $"Total Users: {users.Count}";
        }
        private async Task LoadCarsAsync()
        {
            var cars = await _dbContext.Cars.ToListAsync();
            userDataGrid.ItemsSource = cars;
            UserCountText = $"Total Users: {cars.Count}";
        }


        private async void UpdateCounts()
        {
            UserCountText= await _dbContext.Cars.ToListAsync() !=null ? $"Total Users : { _dbContext.Cars.Count()}" : "No user in the system at the moment";
            lblNoUsers.Text = UserCountText;
            CarCountText = _selectedUser != null ? $"Total Cars: {_selectedUser.Cars.Count}" : "No cars for this user";
            lblNoCars.Text = CarCountText;

        }


        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            var user = new User { Username = username };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            await LoadUsersAsync();
        }

        private async void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = userDataGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                // Update only if the new username is not empty
                if (!string.IsNullOrEmpty(txtUsername.Text))
                {
                    selectedUser.Username = txtUsername.Text;
                    await _dbContext.SaveChangesAsync();
                }
                await LoadUsersAsync();
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = userDataGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                _dbContext.Users.Remove(selectedUser);
                await _dbContext.SaveChangesAsync();
                await LoadUsersAsync();
            }
        }

        private async void AddCar_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser != null)
            {
                string carName = txtCarName.Text;
                var car = new Car { Name = carName, User = _selectedUser };
                _dbContext.Cars.Add(car);
                await _dbContext.SaveChangesAsync();
                await LoadDataAsync();
            }
        }

        private async void UpdateCar_Click(object sender, RoutedEventArgs e)
        {
            var selectedCar = carsDataGrid.SelectedItem as Car;
            if (selectedCar != null)
            {
                selectedCar.Name = txtCarName.Text;
                await _dbContext.SaveChangesAsync();
                await LoadDataAsync();
            }
        }

        private async void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            var selectedCar = carsDataGrid.SelectedItem as Car;
            if (selectedCar != null)
            {
                _dbContext.Cars.Remove(selectedCar);
                await _dbContext.SaveChangesAsync();
                await LoadDataAsync();
            }
        }
        private void UserDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = userDataGrid.SelectedItem as User;
            if (_selectedUser != null)
            {
                _dbContext.Entry(_selectedUser).Collection(u => u.Cars).Load();
                carsDataGrid.ItemsSource = _selectedUser.Cars;
                UpdateCounts();
            }
            else
            {
                carsDataGrid.ItemsSource = null;
                UpdateCounts();
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
