using aUI.Automation.DbObjects;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aUI.Automation.ScreenCaptures
{
    public class DemoStoreImg
    {
        private static Object locker = new Object();
        public static void UploadImage(string url, byte[] image, bool valid = true)
        {
            string cs = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=dbo.ml_images;Integrated Security=SSPI";
            
            
            cs = @"Data Source=localhost\SQLEXPRESS;User id=automation;password=Test123!;Initial Catalog=machinelearning";

            using var con = new SqlConnection(cs);

            lock (locker)
            {
                con.Open();

                var ins = "Insert INTO ml_images (url, image, success) Values (@url, @img, @suck)";

                var cmd = new SqlCommand(ins, con);

                var b = Convert.ToBase64String(image);

                cmd.Parameters.AddWithValue("@url", url);
                cmd.Parameters.AddWithValue("@img", b);
                cmd.Parameters.AddWithValue("@suck", valid ? 0:1);

                var num = cmd.ExecuteNonQuery();
            }
        }
        /* MySql version (working)
         * 
            string cs = @"server=localhost;userid=auto;password=Test123!;database=dbo.ml_images";

            using var con = new MySqlConnection(cs);
            con.Open();
            
            var ins = "Insert INTO ml_images (url, Image) Values (@url, @img)";
            
            var cmd = new MySqlCommand(ins, con);

            cmd.Parameters.AddWithValue("@url", url);
            cmd.Parameters.AddWithValue("@img", image);

            var num = cmd.ExecuteNonQuery();
            Console.WriteLine($"Rows Altered: {num}");
         */
    }
}
