using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;


namespace NorthwindApp
{
    public partial class Form1 : Form
    {
        public string conString { get; set; } = "Server=.;Database=NORTHWND;Trusted_Connection=True;";

        SqlConnection con;

        public int SelectedCategoryId { get; set; }

        public Form1()
        {
            InitializeComponent();
            con = new SqlConnection(conString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetCategories();

            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;

        }

        void SaveCategory()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = con;
            sqlCommand.CommandText = "insert into categories(CategoryName,Description,Picture) values (@CategoryName,@Description,@Picture)";

            MemoryStream ms = new MemoryStream();
            byte[] picture = null;
            if (pbPicture.Image != null)
            {
                pbPicture.Image.Save(ms, ImageFormat.Bmp);
                picture = ms.ToArray();
            }

            sqlCommand.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text);
            sqlCommand.Parameters.AddWithValue("@Description", txtDescription.Text);

            if (picture == null)
            {
                SqlParameter ip = new SqlParameter("@Picture", SqlDbType.Image);
                ip.Value = DBNull.Value;
                sqlCommand.Parameters.Add(ip);
            }
            else
            {
                sqlCommand.Parameters.AddWithValue("@Picture", picture);
            }


            if (con.State != ConnectionState.Open)
                con.Open();

            sqlCommand.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("New category created");
            GetCategories();
            btnNew.PerformClick();
        }

        void UpdateCategory()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = con;
            sqlCommand.CommandText = "Update categories set CategoryName=@CategoryName,Description=@Description, Picture=  @Picture Where CategoryId = @CategoryID";

            MemoryStream ms = new MemoryStream();
            byte[] picture = null;
            if (pbPicture.Image != null)
            {
                pbPicture.Image.Save(ms, ImageFormat.Bmp);
                picture = ms.ToArray();
            }



            sqlCommand.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text);
            sqlCommand.Parameters.AddWithValue("@Description", txtDescription.Text);
            sqlCommand.Parameters.AddWithValue("@CategoryID", SelectedCategoryId);

            if (picture == null)
            {
                SqlParameter ip = new SqlParameter("@Picture", SqlDbType.Image);
                ip.Value = DBNull.Value;
                sqlCommand.Parameters.Add(ip);
            }
            else
            {
                sqlCommand.Parameters.AddWithValue("@Picture", picture);
            }


            if (con.State != ConnectionState.Open)
                con.Open();

            sqlCommand.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Category Updated");
            GetCategories();
        }

        private void DeleteCategory()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = con;
            sqlCommand.CommandText = "delete from categories  Where CategoryId = @CategoryID";

            sqlCommand.Parameters.AddWithValue("@CategoryID", SelectedCategoryId);

            if (con.State != ConnectionState.Open)
                con.Open();

            sqlCommand.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Category Deleted");
            btnNew.PerformClick();
            GetCategories();
        }
        void GetCategories()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = con;
            sqlCommand.CommandText = "select * from Categories";
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            adapter.Fill(dt);

            gridCategoryList.DataSource = dt;

        }

        void GetCategory()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = con;
            sqlCommand.CommandText = "select * from Categories Where CategoryId = @CategoryID";
            sqlCommand.Parameters.AddWithValue(@"CategoryID", SelectedCategoryId);

            if (con.State != ConnectionState.Open)
                con.Open();

            SqlDataReader dr = sqlCommand.ExecuteReader();

            pbPicture.Image = null;

            while (dr.Read())
            {
                txtCategoryName.Text = dr.GetString(1);
                txtDescription.Text = dr.GetString(2);

                if (dr["Picture"] != DBNull.Value)
                {
                    var pictureBytes = (byte[])dr["Picture"];

                    var image = (Image)(new ImageConverter().ConvertFrom(pictureBytes));

                    pbPicture.Image = image;
                }

            }

            con.Close();

        }

        void DeleteImage()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = con;
            sqlCommand.CommandText = "update categories set Picture = @Picture Where CategoryId = @CategoryID ";


            SqlParameter ip = new SqlParameter("@Picture", SqlDbType.Image);
            ip.Value = DBNull.Value;
            sqlCommand.Parameters.Add(ip);

            sqlCommand.Parameters.AddWithValue("@CategoryID", SelectedCategoryId);


            if (con.State != ConnectionState.Open)
                con.Open();

            sqlCommand.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Image Deleted");
            pbPicture.Image = null;
            GetCategories();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCategory();
        }

        private void pbPicture_DoubleClick(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                pbPicture.ImageLocation = fileDialog.FileName;
            }
        }

        private void gridCategoryList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedData = gridCategoryList.SelectedRows[0].Cells[0].Value;
            SelectedCategoryId = (int)selectedData;

            btnSave.Enabled = false;
            btnDelete.Enabled = true;
            btnUpdate.Enabled = true;

            GetCategory();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            SelectedCategoryId = -1;
            txtCategoryName.Clear();
            txtDescription.Clear();
            pbPicture.Image = null;

            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;

        }

        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            if (pbPicture.Image != null)
            {
                if (MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DeleteImage();
                }
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateCategory();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DeleteCategory();
            }
        }


    }
}
