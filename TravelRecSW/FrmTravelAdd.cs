using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TravelRecSW
{
    public partial class FrmTravelAdd : Form
    {       

        public FrmTravelAdd()
        {
            InitializeComponent();
        }

        private void tsbtSave_Click(object sender, EventArgs e)
        {
            //Validate data
            if (tbTravelPlace.Text.Trim().Length == 0)
            {
                SharedInfo.showWarningMSG("ป้อนชื่อสถานที่ที่ไปด้วย");
            }
            else if (tbTravelCostTotal.Text.Trim().Length == 0)
            {
                SharedInfo.showWarningMSG("ป้อนค่าใช้จ่ายในการเดินทางไปด้วย");
            }
            else if (dtpTravelEndDate.Value <= dtpTravelStartDate.Value)
            {
                SharedInfo.showWarningMSG("วันที่กลับควรจะหลังหรือวันเดียวกับวันที่ไป");
            }
            else
            {
                //ส่งข้อมูลไปบันทึกใน DB
                //ติดต่อ DB 
                SqlConnection conn = new SqlConnection(SharedInfo.connStr);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();

                //คำสั่ง SQL 
                string strSql = "INSERT INTO travel_tb  " +
                                "(travelPlace, travelStartDate, travelEndDate, travelCostTotal, travelImage, travellerId)  " +
                                "VALUES " +
                                "(@travelPlace, @travelStartDate, @travelEndDate, @travelCostTotal, @travelImage, @travellerId) ";

                //สร้าง SQL Trasaction และ SQL Command เพื่อทำงานกับคำสั่ง SQL
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = conn;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = strSql;
                sqlCommand.Transaction = sqlTransaction;

                //Bind param เพื่อกำหนดข้อมูลให้กับ SQL Parameter
                sqlCommand.Parameters.AddWithValue("@travelPlace", tbTravelPlace.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@travelStartDate", dtpTravelStartDate.Value.Date);
                sqlCommand.Parameters.AddWithValue("@travelEndDate", dtpTravelEndDate.Value.Date);
                sqlCommand.Parameters.AddWithValue("@travelCostTotal", float.Parse( tbTravelCostTotal.Text.Trim()) );
                sqlCommand.Parameters.AddWithValue("@travelImage", travelImage);
                sqlCommand.Parameters.AddWithValue("@travellerId", SharedInfo.travellerId);

                //สั่งให้ SQL ทำงาน
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    sqlTransaction.Commit();
                    conn.Close();

                    //เมื่อบันทึกสำเร็จให้แสดงข้อความแจ้ง และกลับไปหน้า FrmLogin
                    MessageBox.Show("บันทึกการเดินทางสำเร็จสำเร็จ", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Dispose(); //ปิด Dialog เพราะเราเปิดแบบ ShowDialog()
                    //FrmLogin frmLogin = new FrmLogin();
                    //frmLogin.Show();
                    //Hide();
                }
                catch (Exception ex)
                {
                    sqlTransaction.Rollback();
                    conn.Close();

                    SharedInfo.showWarningMSG("มีข้อผิดพลาดเกิดขึ้น กรุณาลองใหม่อีกครั้งหรือติดต่อ IT Error:" + ex.Message);
                }
            }
        }

        //สร้างตัวแปรเก็บรูปที่ผู้ใช้งานเลือกในรูปแบบของ byte[] เพื่อจะเก็บใน DB แบบ image
        byte[] travelImage;

        private void btSelectTravelImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //เอารูปที่เลือกไปแสดงที่ pbTravellerImage
                pbTravelImage.Image = Image.FromFile(ofd.FileName);

                //แปลงรูปที่เลือกเป็น byte[] เก็บในตัวแปร travellerImage ที่สร้างไว้
                //สร้าวตัวแปรเก็บประเภทไฟล์
                string extFile = Path.GetExtension(ofd.FileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    if (extFile == "jpg" || extFile == "jpeg")
                    {
                        pbTravelImage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else
                    {
                        pbTravelImage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    travelImage = ms.ToArray();
                }
            }
        }

        private void tsbtCancel_Click(object sender, EventArgs e)
        {
            tbTravelPlace.Clear();
            tbTravelCostTotal.Clear();
            dtpTravelStartDate.Value = DateTime.Now;
            dtpTravelEndDate.Value = DateTime.Now;
            travelImage = null;
            pbTravelImage.Image = Properties.Resources.logo;
        }

        private void tsbtToFrmLogin_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
