using System;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TravelRecSW
{
    public partial class FrmRegister : Form
    {
        //สร้างตัวแปรเก็บรูปที่ผู้ใช้งานเลือกในรูปแบบของ byte[] เพื่อจะเก็บใน DB แบบ image
        byte[] travellerImage;

        public FrmRegister()
        {
            InitializeComponent();
        }

        private void tbTravellerPassword_Enter(object sender, EventArgs e)
        {
            //แสดง ToolTip เมื่อผู้ใช้งานคลิกลงไปในช่อง TextBox
            TextBox tb = (TextBox)sender;
            int showToolTipTime = 3000; //หน่วยเป็น millisecond

            ToolTip tt = new ToolTip();
            tt.Show("รหัสผ่านต้อง 6 ตัวขึ้นไป", tb, 20, 20, showToolTipTime);
        }

        private void tbTravellerPasswordConfirm_Enter(object sender, EventArgs e)
        {
            //แสดง ToolTip เมื่อผู้ใช้งานคลิกลงไปในช่อง TextBox
            TextBox tb = (TextBox)sender;
            int showToolTipTime = 3000; //หน่วยเป็น millisecond

            ToolTip tt = new ToolTip();
            tt.Show("รหัสผ่านต้อง 6 ตัวขึ้นไป", tb, 20, 20, showToolTipTime);
        }

        private void btSelectTravellerImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //เอารูปที่เลือกไปแสดงที่ pbTravellerImage
                pbTravellerImage.Image = Image.FromFile(ofd.FileName);

                //แปลงรูปที่เลือกเป็น byte[] เก็บในตัวแปร travellerImage ที่สร้างไว้
                //สร้าวตัวแปรเก็บประเภทไฟล์
                string extFile = Path.GetExtension(ofd.FileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    if (extFile == "jpg" || extFile == "jpeg")
                    {
                        pbTravellerImage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else
                    {
                        pbTravellerImage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    travellerImage = ms.ToArray();
                }
            }
        }

        private void tsbtSave_Click(object sender, EventArgs e)
        {
            //Validate UI
            if (tbTravellerFullname.Text.Trim().Length == 0)
            {
                SharedInfo.showWarningMSG("ป้อนชื่อด้วย");
            }
            else if (tbTravellerEmail.Text.Trim().Length == 0)
            {
                SharedInfo.showWarningMSG("ป้อนอีเมล์ด้วย");
            }
            else if (!tbTravellerEmail.Text.Trim().Contains("@"))
            {
                SharedInfo.showWarningMSG("ป้อนอีเมล์ให้ถูกรูปแบบด้วย");
            }
            else if (tbTravellerPassword.Text.Trim().Length == 0)
            {
                SharedInfo.showWarningMSG("ป้อนรหัสผ่านด้วย");
            }
            else if (tbTravellerPassword.Text.Trim().Length < 6)
            {
                SharedInfo.showWarningMSG("ป้อนรหัสผ่านต้อง 6 ตัวอักษรขึ้นไป");
            }
            else if (tbTravellerPassword.Text.Trim() != tbTravellerPasswordConfirm.Text.Trim())
            {
                SharedInfo.showWarningMSG("รหัสและยืนยันรหัสผ่านต้องตรงกัน");
            }
            else if (travellerImage == null)
            {
                SharedInfo.showWarningMSG("เลือกรูปด้วย");
            }
            else if (cbConfirm.Checked == false)
            {
                SharedInfo.showWarningMSG("กรุณายืนยันการลงทะเบียนด้วย");
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
                string strSql = "INSERT INTO traveller_tb  " +
                                "(travellerFullname, travellerEmail, travellerPassword, travellerImage)  " +
                                "VALUES " +
                                "(@travellerFullname, @travellerEmail, @travellerPassword, @travellerImage) ";

                //สร้าง SQL Trasaction และ SQL Command เพื่อทำงานกับคำสั่ง SQL
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = conn;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = strSql;
                sqlCommand.Transaction = sqlTransaction;

                //Bind param เพื่อกำหนดข้อมูลให้กับ SQL Parameter
                sqlCommand.Parameters.AddWithValue("@travellerFullname", tbTravellerFullname.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@travellerEmail", tbTravellerEmail.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@travellerPassword", tbTravellerPassword.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@travellerImage", travellerImage);

                //สั่งให้ SQL ทำงาน
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    sqlTransaction.Commit();
                    conn.Close();

                    //เมื่อบันทึกสำเร็จให้แสดงข้อความแจ้ง และกลับไปหน้า FrmLogin
                    MessageBox.Show("ลงทะเบียนสำเร็จ","ผลการทำงาน",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FrmLogin frmLogin = new FrmLogin();
                    frmLogin.Show();
                    Hide();
                }
                catch (Exception ex) {
                    sqlTransaction.Rollback();
                    conn.Close();

                    SharedInfo.showWarningMSG("มีข้อผิดพลาดเกิดขึ้น กรุณาลองใหม่อีกครั้งหรือติดต่อ IT Error:" + ex.Message);
                }
            }
        }

        private void tsbtCancel_Click(object sender, EventArgs e)
        {
            //ทุกอย่างกลับเป็นเหมือนเดิม
            tbTravellerFullname.Clear();
            tbTravellerEmail.Clear();
            tbTravellerPassword.Clear();
            tbTravellerPasswordConfirm.Clear();
            pbTravellerImage.Image = Properties.Resources.profile; 
            travellerImage = null;
            cbConfirm.Checked = false;
        }

        private void tsbtToFrmLogin_Click(object sender, EventArgs e)
        {
            //เปิดกลับไปหน้า FrmLogin
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.Show();
            Hide();
        }
    }
}
