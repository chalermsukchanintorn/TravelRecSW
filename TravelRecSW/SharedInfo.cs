using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TravelRecSW
{
    internal class SharedInfo
    {
        public static void showWarningMSG(string msg)
        {
            MessageBox.Show(msg, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //สร้างตัวแปรเก็บ Connection String ที่ใช้ติดต่อกับ DB
        //public static string connStr = "Server=NINNIN2020;Database=travel_db;Trusted_connection=True;MultipleActiveResultSets=true";
        public static string connStr = @"Data Source=NINNIN2020\SQLEXPRESS01;Initial Catalog=travel_db;Integrated Security=true";


        //----------------------
        public static int travellerId;
        public static string travellerFullname;
        public static string travellerEmail;
        public static string travellerPassword;
        public static byte[] travellerImage;

    }
}
