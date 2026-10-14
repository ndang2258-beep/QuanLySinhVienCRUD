using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace QuanLySinhVienCRUD
{
    public partial class Form1 : Form
    {
        private readonly TextBox txtHoTen = new TextBox();
        private readonly TextBox txtDiem = new TextBox();
        private readonly DataGridView dgvSinhVien = new DataGridView();
        private readonly Label lblTrangThai = new Label();

        private readonly Button btnThem = new Button();
        private readonly Button btnSua = new Button();
        private readonly Button btnXoa = new Button();
        private readonly Button btnTaiLai = new Button();
        private readonly Button btnLocDat = new Button();

        public Form1()
        {
            // Không gọi InitializeComponent() vì giao diện được tạo bằng code.
            TaoGiaoDien();

            Load += Form1_Load;
            dgvSinhVien.SelectionChanged += dgvSinhVien_SelectionChanged;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnTaiLai.Click += btnTaiLai_Click;
            btnLocDat.Click += btnLocDat_Click;
        }

        // ===== THIẾT KẾ GIAO DIỆN =====
        private void TaoGiaoDien()
        {
            Text = "Quản Lý Sinh Viên - EF Core CRUD";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(850, 570);
            MinimumSize = new Size(750, 500);
            Font = new Font("Segoe UI", 10);

            Label lblTieuDe = new Label
            {
                Text = "QUẢN LÝ SINH VIÊN - EF CORE (CRUD ĐẦY ĐỦ)",
                Location = new Point(20, 15),
                Size = new Size(790, 35),
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblHoTen = new Label
            {
                Text = "Họ tên:",
                Location = new Point(25, 65),
                Size = new Size(75, 28)
            };

            txtHoTen.Location = new Point(100, 63);
            txtHoTen.Size = new Size(240, 28);

            Label lblDiem = new Label
            {
                Text = "Điểm:",
                Location = new Point(375, 65),
                Size = new Size(60, 28)
            };

            txtDiem.Location = new Point(435, 63);
            txtDiem.Size = new Size(120, 28);

            DatNut(btnThem, "＋ Thêm", 25, 110, 105);
            DatNut(btnSua, "✎ Sửa", 140, 110, 105);
            DatNut(btnXoa, "✖ Xóa", 255, 110, 105);
            DatNut(btnTaiLai, "↻ Tải lại", 370, 110, 115);
            DatNut(btnLocDat, "Lọc SV đạt", 495, 110, 130);

            dgvSinhVien.Location = new Point(25, 165);
            dgvSinhVien.Size = new Size(780, 300);
            dgvSinhVien.Anchor =
                AnchorStyles.Top | AnchorStyles.Bottom |
                AnchorStyles.Left | AnchorStyles.Right;

            dgvSinhVien.ReadOnly = true;
            dgvSinhVien.AllowUserToAddRows = false;
            dgvSinhVien.AllowUserToDeleteRows = false;
            dgvSinhVien.AutoGenerateColumns = true;
            dgvSinhVien.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvSinhVien.MultiSelect = false;
            dgvSinhVien.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvSinhVien.RowHeadersVisible = false;

            lblTrangThai.Location = new Point(25, 480);
            lblTrangThai.Size = new Size(780, 30);
            lblTrangThai.Anchor =
                AnchorStyles.Bottom | AnchorStyles.Left |
                AnchorStyles.Right;
            lblTrangThai.Text = "Sẵn sàng.";

            Controls.AddRange(new Control[]
            {
                lblTieuDe, lblHoTen, txtHoTen,
                lblDiem, txtDiem,
                btnThem, btnSua, btnXoa,
                btnTaiLai, btnLocDat,
                dgvSinhVien, lblTrangThai
            });
        }

        private void DatNut(
            Button nut, string noiDung,
            int x, int y, int rong)
        {
            nut.Text = noiDung;
            nut.Location = new Point(x, y);
            nut.Size = new Size(rong, 35);
            nut.Cursor = Cursors.Hand;
        }

        // ===== KHỞI ĐỘNG =====
        private async void Form1_Load(object sender, EventArgs e)
        {
            lblTrangThai.Text = "Đang kết nối cơ sở dữ liệu...";
            await TaiDanhSach();
        }

        // ===== READ: TẢI DANH SÁCH =====
        private async Task TaiDanhSach()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var danhSach = await context.Students
                        .AsNoTracking()
                        .OrderBy(sv => sv.Id)
                        .ToListAsync();

                    dgvSinhVien.DataSource = null;
                    dgvSinhVien.DataSource = danhSach;

                    lblTrangThai.Text =
                        $"Đã tải {danhSach.Count} sinh viên.";
                    lblTrangThai.ForeColor = Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi tải dữ liệu: " + ex.Message;
                lblTrangThai.ForeColor = Color.Red;
                MessageBox.Show(
                    ex.ToString(),
                    "Lỗi kết nối cơ sở dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ===== CHỌN DÒNG =====
        private void dgvSinhVien_SelectionChanged(
            object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is Student sv)
            {
                txtHoTen.Text = sv.FullName;
                txtDiem.Text = sv.Grade.ToString(
                    CultureInfo.CurrentCulture);
            }
        }

        // Đọc và kiểm tra thông tin nhập
        private bool DocThongTin(out string hoTen, out double diem)
        {
            hoTen = txtHoTen.Text.Trim();
            diem = 0;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                txtHoTen.Focus();
                return false;
            }

            string giaTriDiem = txtDiem.Text.Trim();

            bool hopLe =
                double.TryParse(
                    giaTriDiem,
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out diem)
                ||
                double.TryParse(
                    giaTriDiem,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out diem);

            if (!hopLe || double.IsNaN(diem) ||
                double.IsInfinity(diem) || diem < 0 || diem > 10)
            {
                MessageBox.Show("Điểm phải là số từ 0 đến 10!");
                txtDiem.Focus();
                return false;
            }

            return true;
        }

        // ===== CREATE: THÊM =====
        private async void btnThem_Click(object sender, EventArgs e)
        {
            if (!DocThongTin(out string hoTen, out double diem))
                return;

            try
            {
                using (var context = new AppDbContext())
                {
                    context.Students.Add(new Student
                    {
                        FullName = hoTen,
                        Grade = diem
                    });

                    await context.SaveChangesAsync();
                }

                XoaONhap();
                await TaiDanhSach();
                lblTrangThai.Text = "Đã thêm sinh viên thành công.";
                lblTrangThai.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                BaoLoi("Thêm sinh viên thất bại", ex);
            }
        }

        // ===== UPDATE: SỬA =====
        private async void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is not Student svChon)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa!");
                return;
            }

            if (!DocThongTin(out string hoTen, out double diem))
                return;

            try
            {
                using (var context = new AppDbContext())
                {
                    Student sv = await context.Students.FindAsync(svChon.Id);

                    if (sv == null)
                    {
                        MessageBox.Show("Sinh viên không còn tồn tại.");
                        await TaiDanhSach();
                        return;
                    }

                    sv.FullName = hoTen;
                    sv.Grade = diem;

                    await context.SaveChangesAsync();
                }

                await TaiDanhSach();
                lblTrangThai.Text = "Đã cập nhật sinh viên thành công.";
                lblTrangThai.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                BaoLoi("Sửa sinh viên thất bại", ex);
            }
        }

        // ===== DELETE: XÓA =====
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is not Student svChon)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!");
                return;
            }

            DialogResult ketQua = MessageBox.Show(
                $"Bạn có chắc muốn xóa \"{svChon.FullName}\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (ketQua != DialogResult.Yes)
                return;

            try
            {
                using (var context = new AppDbContext())
                {
                    Student sv = await context.Students.FindAsync(svChon.Id);

                    if (sv != null)
                    {
                        context.Students.Remove(sv);
                        await context.SaveChangesAsync();
                    }
                }

                XoaONhap();
                await TaiDanhSach();
                lblTrangThai.Text = "Đã xóa sinh viên thành công.";
                lblTrangThai.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                BaoLoi("Xóa sinh viên thất bại", ex);
            }
        }

        // ===== TẢI LẠI =====
        private async void btnTaiLai_Click(object sender, EventArgs e)
        {
            await TaiDanhSach();
        }

        // ===== LINQ: LỌC SINH VIÊN ĐẠT =====
        private async void btnLocDat_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var ketQua = await context.Students
                        .AsNoTracking()
                        .Where(sv => sv.Grade >= 5)
                        .OrderByDescending(sv => sv.Grade)
                        .ToListAsync();

                    dgvSinhVien.DataSource = null;
                    dgvSinhVien.DataSource = ketQua;

                    lblTrangThai.Text =
                        $"Tìm thấy {ketQua.Count} sinh viên đạt (điểm >= 5).";
                    lblTrangThai.ForeColor = Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                BaoLoi("Lọc sinh viên thất bại", ex);
            }
        }

        private void XoaONhap()
        {
            txtHoTen.Clear();
            txtDiem.Clear();
            dgvSinhVien.ClearSelection();
        }

        private void BaoLoi(string tieuDe, Exception ex)
        {
            lblTrangThai.Text = tieuDe + ".";
            lblTrangThai.ForeColor = Color.Red;

            MessageBox.Show(
                tieuDe + "!\n\n" + ex.Message,
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}