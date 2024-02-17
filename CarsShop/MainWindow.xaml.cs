using CarsShop.Models;
using System.ComponentModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace CarsShop
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _dbContext;
        private User _selectedUser;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _userCountText;
        public string UserCountText
        {
            get { return _userCountText; }
            set { _userCountText = value; OnPropertyChanged(nameof(UserCountText)); }
        }

        private string _carCountText;
        public string CarCountText
        {
            get { return _carCountText; }
            set { _carCountText = value; OnPropertyChanged(nameof(CarCountText)); }
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
            await LoadCarsAsync();
            UpdateCounts();
        }

        private async Task LoadUsersAsync()
        {
            var users = await _dbContext.Users.ToListAsync();
            userDataGrid.ItemsSource = users;
            UpdateUserCount(users.Count);
        }

        private async Task LoadCarsAsync()
        {
            var cars = await _dbContext.Cars.Where(x=>x.UserId==_selectedUser.UserId).ToListAsync();
            carsDataGrid.ItemsSource = cars;
            UpdateCarCount(cars.Count);
        }

        private async void UpdateCounts()
        {
            UserCountText = await _dbContext.Users.CountAsync() > 0 ? $"Total Users: {await _dbContext.CountUsersAsync()}" : "No users in the system at the moment.";
            lblNoUsers.Text = UserCountText;
            CarCountText = _selectedUser != null ? $"Total Cars: {await _dbContext.CountCarsAsync(_selectedUser.UserId)}" : "Please select a user in order to display his cars.";
            lblNoCars.Text = CarCountText;
        }

        private void UpdateUserCount(int count)
        {
            UserCountText = $"Total Users: {count}";
        }

        private void UpdateCarCount(int count)
        {
            CarCountText = $"Total Cars: {count}";
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                if (string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("Please enter a username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = new User { Username = username, Email = "" };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding a user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedUser = userDataGrid.SelectedItem as User;
                if (selectedUser != null)
                {
                    string newUsername = txtUsername.Text;
                    if (string.IsNullOrEmpty(newUsername))
                    {
                        MessageBox.Show("Please enter a new username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    selectedUser.Username = newUsername;
                    await _dbContext.SaveChangesAsync();
                    await LoadUsersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating a user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = userDataGrid.SelectedItem as User;
            if (selectedUser == null)
            {
                MessageBox.Show("Please select a user to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _dbContext.Users.Remove(selectedUser);
            await _dbContext.SaveChangesAsync();
            await LoadUsersAsync();
        }

        private async void AddCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedUser == null)
                {
                    MessageBox.Show("Please select a user before adding a car.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string carName = txtCarName.Text;
                if (string.IsNullOrEmpty(carName))
                {
                    MessageBox.Show("Please enter a car name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var car = new Car { Name = carName, User = _selectedUser };
                _dbContext.Cars.Add(car);
                await _dbContext.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding a car: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedCar = carsDataGrid.SelectedItem as Car;
                if (selectedCar == null)
                {
                    MessageBox.Show("Please select a car to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string newCarName = txtCarName.Text;
                if (string.IsNullOrEmpty(newCarName))
                {
                    MessageBox.Show("Please update the car name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selectedCar.Name = newCarName;
                await _dbContext.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating a car: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedCar = carsDataGrid.SelectedItem as Car;
                if (selectedCar == null)
                {
                    MessageBox.Show("Please select a car to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _dbContext.Cars.Remove(selectedCar);
                await _dbContext.SaveChangesAsync();
                await LoadCarsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while removing a car: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = userDataGrid.SelectedItem as User;
            UpdateCounts();
            if (_selectedUser != null)
            {
                _dbContext.Entry(_selectedUser).Collection(u => u.Cars).Load();
                carsDataGrid.ItemsSource = _selectedUser.Cars;
            }
            else
            {
                carsDataGrid.ItemsSource = null;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}