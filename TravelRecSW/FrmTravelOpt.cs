using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelRecSW
{
    public partial class FrmTravelOpt : Form
    {

        public FrmTravelOpt()
        {
            InitializeComponent();
        }

        private void getTravelFromDBToDGV()
        {
            //ติดต่อ DB 
            SqlConnection conn = new SqlConnection(SharedInfo.connStr);
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();

            //คำสั่ง SQL 
            string strSql = "SELECT travelPlace, travelCostTotal, travelImage, travelId FROM travel_tb  " +
                            "WHERE travellerId = @travellerId";

            //สร้าง SQL Trasaction และ SQL Command เพื่อทำงานกับคำสั่ง SQL
            SqlTransaction sqlTransaction = conn.BeginTransaction();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = conn;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = strSql;
            sqlCommand.Transaction = sqlTransaction;

            //Bind param เพื่อกำหนดข้อมูลให้กับ SQL Parameter
            sqlCommand.Parameters.AddWithValue("@travellerId", SharedInfo.travellerId);


            //สั่งให้ SQL
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            conn.Close();

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            //เอาไปข้อมูลใน DataTable ไปแสดงที่ DGV
            if (dt.Rows.Count > 0)
            {
                //ปรับความสูงของแต่ละแถวของ DGV
                dgvTravel.RowTemplate.Height = 50;

                //กรณีมี จะนำข้อมูลมาแสดง
                dgvTravel.DataSource = dt;
                
                //กำหนดหัวคอลัมน์ของ DGV
                dgvTravel.Columns[0].HeaderText = "สถานที่ที่ไป";
                dgvTravel.Columns[1].HeaderText = "ค่าใช้จ่าย";
                dgvTravel.Columns[2].HeaderText = "รูปสถานที่ที่ไป";
                //กำหนดขนาดความกว้างของ DGV
                dgvTravel.Columns[0].Width = 150;
                dgvTravel.Columns[1].Width = 115;
                dgvTravel.Columns[2].Width = 200;
                dgvTravel.Columns[3].Width = 0;
                
                //ปรับรูปให้พอดีกับความสูง
                DataGridViewImageColumn imgCol = (DataGridViewImageColumn)dgvTravel.Columns[2];
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            else
            {
                //กรณีไม่มี ให้แสดงแค่หัวคอลัมน์ของตาราง ใน DGV

            }
        }

        private void FrmTravelOpt_Load(object sender, EventArgs e)
        {
            //เอารูปมา traveller มาแสดง
            using (MemoryStream ms = new MemoryStream(SharedInfo.travellerImage))
            {
                pbTravellerImage.Image = Image.FromStream(ms);
            }

            //เอาชื่อ traveller มาแสดง
            lbTravellerFullname.Text = SharedInfo.travellerFullname;

            //ดึงข้อมูลการเดินทางของ traveller คนที่ Login เข้ามามาแสดง
            getTravelFromDBToDGV();
        }

        private void tsbtToFrmLogin_Click(object sender, EventArgs e)
        {
            //FrmLogin frmLogin = new FrmLogin();
            //frmLogin.Show();

            new FrmLogin().Show();
            Hide();
        }

        private void tsbtAdd_Click(object sender, EventArgs e)
        {
            FrmTravelAdd frmTravelAdd = new FrmTravelAdd();
            frmTravelAdd.ShowDialog(this);
            getTravelFromDBToDGV();
        }

        private void tsbtEdit_Click(object sender, EventArgs e)
        {
            //ตรวจสอบก่อนว่าได้เลือกรายการหรือแถวที่จะแก้หรือยัง
            if (dgvTravel.SelectedRows.Count <= 0)
            {
                SharedInfo.showWarningMSG("เลือกรายการที่จะแก้ไขด้วย");
            }
            else
            {
                //สร้างตัวแปรเก็บแถวที่เลือก
                int indexRow = dgvTravel.CurrentRow.Index;
                //สร้างตัวแปรเก็บ travelId
                int travelId = int.Parse( dgvTravel.Rows[indexRow].Cells[3].Value.ToString() );

                //ให้เปิดหน้า FrmTravelEdit โดยส่ง travelId ไปด้วย
                FrmTravelEdit frmTravelEdit = new FrmTravelEdit(travelId);
                frmTravelEdit.ShowDialog(this);

                getTravelFromDBToDGV();
            }
        }
    }
}
