using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelRecSW
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void lbToFrmRegister_Click(object sender, EventArgs e)
        {
            FrmRegister frmRegister = new FrmRegister();
            frmRegister.Show();
            Hide();
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            //Validate UI
            if (tbTravellerEmail.Text.Trim().Length == 0)
            {
                SharedInfo.showWarningMSG("ป้อนชื่อผู้ใช้ (อีเมล์) ด้วย");
            }
            else if (tbTravellerPassword.Text.Trim().Length == 0)
            {
                SharedInfo.showWarningMSG("ป้อนรหัสผ่านด้วย");
            }
            else
            {
                //เอาชื่อผู้ใช้ (อีเมล์) และรหัสผ่านไปตรวจสอบใน DB แล้วเปิดไปหน้า FrmTravelOpt
                //ติดต่อ DB 
                SqlConnection conn = new SqlConnection(SharedInfo.connStr);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();

                //คำสั่ง SQL 
                string strSql = "SELECT * FROM traveller_tb WHERE  " +
                                "travellerEmail = @travellerEmail and  " +
                                "travellerPassword = @travellerPassword";

                //สร้าง SQL Trasaction และ SQL Command เพื่อทำงานกับคำสั่ง SQL
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = conn;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = strSql;
                sqlCommand.Transaction = sqlTransaction;

                //Bind param เพื่อกำหนดข้อมูลให้กับ SQL Parameter
                sqlCommand.Parameters.AddWithValue("@travellerEmail", tbTravellerEmail.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@travellerPassword", tbTravellerPassword.Text.Trim());

                //สั่งให้ SQL
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                conn.Close();

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //ถูกต้อง เปิดหน้าจอ FrmTravelOpt
                    //ก่อนเปิดไป เอาข้อมูลผู้ใช้เก็บในตัวแปรที่ ShareInfo
                    SharedInfo.travellerId = (int) dt.Rows[0]["travellerId"];
                    SharedInfo.travellerFullname = dt.Rows[0]["travellerFullname"].ToString();
                    SharedInfo.travellerEmail = dt.Rows[0]["travellerEmail"].ToString();
                    SharedInfo.travellerPassword = dt.Rows[0]["travellerPassword"].ToString();
                    SharedInfo.travellerImage = (byte[]) dt.Rows[0]["travellerImage"];

                    FrmTravelOpt frmTravelOpt = new FrmTravelOpt();
                    frmTravelOpt.Show();
                    Hide();
                }
                else
                {
                    //ไม่ถูกต้อง แสดงข้อความเตือน
                    SharedInfo.showWarningMSG("ชื่อผู้ใช้ (อีเมล์) หรือรหัสผ่านไม่ถูกต้อง");
                }
            }

        }
    }
}
