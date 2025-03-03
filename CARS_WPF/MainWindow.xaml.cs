using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CARS_WPF
{
    public partial class MainWindow : Window
    {
        private string connectionString = "server=localhost;database=classicmodels;user=root;password=;";

        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
            LoadCountries();
            LoadOrders();
        }

        private void LoadProducts()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT productCode, productName FROM products", conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    productList.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a termékek betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCountries()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT country FROM customers ORDER BY country", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        countryDropdown.Items.Add(reader["country"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az országok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT orderNumber, orderDate, status FROM orders", conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ordersGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a rendelések betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productList.SelectedItem is DataRowView row)
            {
                string productCode = row["productCode"].ToString();
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM orderdetails WHERE productCode = @code", conn);
                        cmd.Parameters.AddWithValue("@code", productCode);
                        int orderCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (orderCount > 0)
                        {
                            orderCountLabel.Content = $"Rendelések száma: {orderCount}";
                        }
                        else
                        {
                            MessageBox.Show("Ehhez a termékhez nem található rendelés.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a rendelések lekérdezésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CountryDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (countryDropdown.SelectedItem is string country)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("SELECT customerName, phone, city FROM customers WHERE country = @country", conn);
                        cmd.Parameters.AddWithValue("@country", country);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        customersGrid.ItemsSource = dt.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba az ügyfelek betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ordersGrid.SelectedItem is DataRowView row)
            {
                string orderNumber = row["orderNumber"].ToString();
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("SELECT p.productName, p.buyPrice FROM orderdetails od JOIN products p ON od.productCode = p.productCode WHERE od.orderNumber = @orderNum ORDER BY p.productName", conn);
                        cmd.Parameters.AddWithValue("@orderNum", orderNumber);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        productsListBox.ItemsSource = dt.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a rendelés termékeinek betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}