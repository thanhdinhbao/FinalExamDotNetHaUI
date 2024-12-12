using FinalExam.Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace FinalExam
{
    public partial class MainWindow : Window
    {
        private QlbanHangContext _context;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            QlbanHangContext _context = new QlbanHangContext();
            var sanPhamList = _context.SanPhams
            .OrderBy(sp => sp.DonGia)
            .Select(sp => new
            {
                sp.MaSp,
                sp.TenSp,
                sp.DonGia,
                sp.SoLuong,
                sp.MaLoai,
                TenLoai = sp.MaLoaiNavigation.TenLoai,
                ThanhTien = sp.SoLuong * sp.DonGia
            })
            .ToList();

            dgSanPham.ItemsSource = sanPhamList;

        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTenSanPham.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm !", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTenSanPham.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMaSanPham.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sản phẩm !", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMaSanPham.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDonGia.Text))
            {
                MessageBox.Show("Vui lòng nhập đơn giá !", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDonGia.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSoLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập số lượng !", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSoLuong.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMaLoai.Text))
            {
                MessageBox.Show("Vui lòng nhập mã loại !", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMaLoai.Focus();
                return false;
            }


            if (!decimal.TryParse(txtDonGia.Text, out _) || !int.TryParse(txtSoLuong.Text, out _))
            {
                MessageBox.Show("Đơn giá và số lượng phải là số hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            using (var context = new QlbanHangContext())
            {
                var sanPham = new SanPham
                {
                    MaSp = txtMaSanPham.Text,
                    TenSp = txtTenSanPham.Text,
                    DonGia = int.Parse(txtDonGia.Text),
                    SoLuong = int.Parse(txtSoLuong.Text),
                    MaLoai = txtMaLoai.Text
                };
                if (context.SanPhams.Any(sp => sp.MaSp == txtMaSanPham.Text))
                {
                    MessageBox.Show("Sản phẩm với mã này đã tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    context.SanPhams.Add(sanPham);
                    context.SaveChanges();

                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadData();
                }

            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new QlbanHangContext())
            {
                string maSanPham = txtMaSanPham.Text;

                var sanPham = context.SanPhams.FirstOrDefault(sp => sp.MaSp == maSanPham);
                if (sanPham == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm với mã này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTenSanPham.Text) ||
                    !int.TryParse(txtDonGia.Text, out int donGia) || donGia <= 0 ||
                    !int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin và đảm bảo Đơn giá, Số lượng là số nguyên > 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                sanPham.TenSp = txtTenSanPham.Text;
                sanPham.DonGia = donGia;
                sanPham.SoLuong = soLuong;
                sanPham.MaLoai = txtMaLoai.Text;

                context.SaveChanges();

                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadData();
            }

        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new QlbanHangContext())
            {
                string maSanPham = txtMaSanPham.Text;

                var sanPham = context.SanPhams.FirstOrDefault(sp => sp.MaSp == maSanPham);
                if (sanPham == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm với mã này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm với mã: {maSanPham}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    context.SanPhams.Remove(sanPham);
                    context.SaveChanges();

                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadData();
                }
            }

        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            string maSanPham = txtMaSanPham.Text.Trim();
            QlbanHangContext _context = new QlbanHangContext();

            var product = _context.SanPhams
                .Where(sp => sp.MaSp == maSanPham)
                .Select(sp => new
                {
                    sp.MaSp,
                    sp.TenSp,
                    sp.DonGia,
                    sp.SoLuong,
                    sp.MaLoai,
                    TenLoai = sp.MaLoaiNavigation.TenLoai,
                    ThanhTien = sp.SoLuong * sp.DonGia
                })
                .ToList();

            if (product.Any())
            {
                var window2 = new FindWindow();
                window2.LoadData(product);
                window2.ShowDialog();
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để hiển thị!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void dgSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSanPham.SelectedItem == null)
            {
                return;
            }
            var selectedProduct = dgSanPham.SelectedItem as dynamic;
            txtMaSanPham.Text = selectedProduct.MaSp;
            txtTenSanPham.Text = selectedProduct.TenSp;
            txtDonGia.Text = selectedProduct.DonGia.ToString();
            txtSoLuong.Text = selectedProduct.SoLuong.ToString();
            txtMaLoai.Text = selectedProduct.MaLoai;

        }

        
    }
}