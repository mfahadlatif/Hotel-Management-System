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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HotelMGT
{
    public partial class Booking : Form
    {
        public Booking()
        {
            InitializeComponent();
            GetRooms();
            GetCustomers();
            populate();
            
        }
        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""F:\C# Projects\My Projects\HotelMGT\HotelMGT\HotelDBMS.mdf"";Integrated Security=True");
        private void populate()
        {
            Con.Open();
            string Query = "select * from BookingTbl";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            BookingDGV.DataSource = ds.Tables[0];
            Con.Close();
        }
        private void GetRooms()
        {
            Con.Open();
            SqlCommand cmd = new SqlCommand("select * from RoomTbl where RStatus='Available'", Con);
            SqlDataReader rdr;
            rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Columns.Add("RNum", typeof(int));
            dt.Load(rdr);
            RoomCb.ValueMember = "RNum";
            //RoomCb.DisplayMember = "Rname";
            RoomCb.DataSource = dt;
            Con.Close();
           
        }
        private void GetCustomers()
        {
            Con.Open();
            SqlCommand cmd = new SqlCommand("select * from CustomerTbl ",
                                            Con);
            SqlDataReader rdr;
            rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Columns.Add("CustNum", typeof(int));
            dt.Load(rdr);
            CustomerCb.ValueMember = "CustNum";
            CustomerCb.DisplayMember = "CustName";

            CustomerCb.DataSource = dt;
            Con.Close();
        }
        int PRICE=1;
        private void fetchCost()
        {
            Con.Open();
            string query = "select TypeCost from RoomTbl join TypeTbl on RType=TypeNum where RNum=" + RoomCb.SelectedValue.ToString() + "";
            SqlCommand cmd=new SqlCommand(query, Con);
            DataTable dt = new DataTable();
            SqlDataAdapter sda=new SqlDataAdapter(cmd);
            sda.Fill(dt);
            foreach(DataRow dr in dt.Rows)
            {
                PRICE = Convert.ToInt32(dr["TypeCost"].ToString());
            }
            Con.Close();
        }
        private void BookRoom()
        {

            if (CustomerCb.SelectedIndex == -1 || RoomCb.SelectedIndex == -1 || AmountTb.Text == "" || DurationTb.Text == "")
            {
                MessageBox.Show("Missing Information !!!");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("insert into BookingTbl(Room,Customer,BookDate ,Duration,Cost) values(@R,@C,@BD,@Dur,@Cost)", Con);
                    cmd.Parameters.AddWithValue("@R", RoomCb.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@C", CustomerCb.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@BD", BDate.Value.Date);
                    cmd.Parameters.AddWithValue("@Dur", DurationTb.Text);
                    cmd.Parameters.AddWithValue("@Cost", AmountTb.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Room Booked !!!");
                    Con.Close();
                    populate();
                    SetBooked();
                    GetRooms();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally { Con.Close(); }
            }
        }
        private void SetBooked()
        {
           
            
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("update RoomTbl set RStatus=@RS where RNum=@BKEY", Con);
                    cmd.Parameters.AddWithValue("@RS", "Booked");
                    cmd.Parameters.AddWithValue("@BKey",RoomCb.SelectedValue.ToString());
                    cmd.ExecuteNonQuery();
                    Con.Close();
                    populate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally { Con.Close(); }
            
    }
        private void SetAvailable()
        {


            try
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("update RoomTbl set RStatus=@RS where RNum=@RKEY", Con);
                cmd.Parameters.AddWithValue("@RS", "Available");
                cmd.Parameters.AddWithValue("@RKey", RoomCb.Text);
                cmd.ExecuteNonQuery();
                Con.Close();
                populate();
                GetRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { Con.Close(); }

        }
        int KEY = 0;
        private void CancelBooking()
        {
            if (KEY == 0)
            {
                MessageBox.Show("Select a Booking !!!");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("delete from BookingTbl where BookNum = @BKey ", Con);
                    cmd.Parameters.AddWithValue("@BKey", KEY);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("  Booking Cancelled !!! ");
                    Con.Close();
                    populate();
                    SetAvailable();
                   
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally { Con.Close(); }
            }

        }
       
          
        private void BookingDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void BookBtn_Click_1(object sender, EventArgs e)
        {
            BookRoom();
        }

        private void CancelBtn_Click_1(object sender, EventArgs e)
        {
            CancelBooking();
        }

        private void RoomCb_SelectionChangeCommitted_1(object sender, EventArgs e)
        {
            fetchCost();
        }

        private void DurationTb_TextChanged_1(object sender, EventArgs e)
        {
            if (DurationTb.Text == "")
            {
                AmountTb.Text = "Rs 0";
            }
            else
            {
                try
                {
                    int Total = PRICE * Convert.ToInt32(DurationTb.Text);
                    AmountTb.Text = "" + Total;
                }
                catch (Exception Ex)
                {

                }

            }
        }

        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            Login obj = new Login();
            obj.Show();
            this.Hide();
        }

        private void panel5_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void panel6_MouseClick(object sender, MouseEventArgs e)
        {
            Rooms obj = new Rooms();
            obj.Show();
            this.Hide();
        }

        private void panel7_MouseClick(object sender, MouseEventArgs e)
        {
            Customers obj = new Customers();
            obj.Show();
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void label4_Click(object sender, EventArgs e)
        {
            
        }

        

        

        private void BookingDGV_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            RoomCb.Text = BookingDGV.SelectedRows[0].Cells[1].Value.ToString();
            CustomerCb.Text = BookingDGV.SelectedRows[0].Cells[2].Value.ToString();
            BDate.Text = BookingDGV.SelectedRows[0].Cells[3].Value.ToString();
            DurationTb.Text = BookingDGV.SelectedRows[0].Cells[4].Value.ToString();
            AmountTb.Text = BookingDGV.SelectedRows[0].Cells[5].Value.ToString();

            if (AmountTb.Text == "")
            {
                KEY = 0;
            }
            else
            {
                KEY = Convert.ToInt32(BookingDGV.SelectedRows[0].Cells[0].Value.ToString());
            }
        }

        private void label4_MouseClick(object sender, MouseEventArgs e)
        {
            Types obj = new Types();
            obj.Show();
            this.Hide();
        }

        private void label6_MouseClick(object sender, MouseEventArgs e)
        {
            Customers obj = new Customers();
            obj.Show();
            this.Hide();
        }

        private void label5_MouseClick(object sender, MouseEventArgs e)
        {
            Rooms obj = new Rooms();
            obj.Show();
            this.Hide();
        }

        private void label2_MouseClick(object sender, MouseEventArgs e)
        {
            Dashboard obj = new Dashboard();
            obj.Show();
            this.Hide();

        }
    }                  
}

