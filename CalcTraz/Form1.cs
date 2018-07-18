using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace CalcTraz
{
    public partial class Form1 : Form
    {
        #region Fields
        private readonly SqlConnection _conn = new SqlConnection();
        private int _count;
        private DataTable _dt;
        #endregion

        #region Constructor
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Functions
        public double CalcStdev(double avg, double val)
        {
            double s = 0;
            for (int i = 0; i < _count; i++)
            {
                s += Math.Pow(Convert.ToDouble(dataGridViewX1.Rows[i].Cells[0].Value.ToString()) - avg, 2);
            }
            s = s / _count;//واریانس
            s = Math.Pow(s, val);//انحراف معیار
            return s;
        }
        #endregion

        #region Events
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.Rows.Count == 0)
                return;

            _count = dataGridViewX1.Rows.Count - 1;

            List<double> listLessThanTenThousand = new List<double>();
            List<double> listMoreTenThousand = new List<double>();
            double sum = 0;
            for (int i = 0; i < _count; i++)
            {
                sum += Convert.ToDouble(dataGridViewX1.Rows[i].Cells[0].Value.ToString());
            }

            sum = sum / _count;//average

            double sqrtIndex = .50;
            double zaribEtminan = 1;
            var stdev = CalcStdev(sum, sqrtIndex);

            for (int i = 0; i < _count; i++)
            {
                var z = (Convert.ToDouble(dataGridViewX1.Rows[i].Cells[0].Value.ToString()) - sum) / stdev;
                var resultTraz = Math.Floor(Convert.ToInt32(1000 * z + 5000) * zaribEtminan);

                listLessThanTenThousand.Add(resultTraz);
            }

            for (int j = 0; j < _count; j++)
            {
                var z = (Convert.ToDouble(dataGridViewX1.Rows[j].Cells[0].Value.ToString()) - sum) / stdev;
                var resultTraz = Math.Floor(Convert.ToInt32(1000 * z + 5000) * zaribEtminan); // OR --> T=2000z+5000

                if (resultTraz > 10000)//سقف کنکور تا 15000 است
                {
                    zaribEtminan = .99;
                    sqrtIndex = sqrtIndex + .01;
                    stdev = CalcStdev(sum, sqrtIndex);
                    j = -1;
                    listMoreTenThousand.Clear();
                }

                else
                    listMoreTenThousand.Add(resultTraz);

            }
         

            listBox1.DataSource = listLessThanTenThousand;
            countList1.Text = listLessThanTenThousand.Count.ToString();

            listBox2.DataSource = listMoreTenThousand;
            countList2.Text = listMoreTenThousand.Count.ToString();

            //max min value ListBox2
            double maxLst1 = Convert.ToDouble(listBox1.Items[0]);
            double minLst1 = maxLst1;

            foreach (var item in listBox1.Items)
            {
                var cng = Convert.ToDouble(item);
                if (cng > maxLst1)
                    maxLst1 = cng;
                else if (cng < minLst1)
                    minLst1 = cng;
            }

            //max min value ListBox2
            double maxLst2 = Convert.ToDouble(listBox2.Items[0]);
            double minLst2 = maxLst2;

            foreach (var item in listBox2.Items)
            {
                var cng = Convert.ToDouble(item);
                if (cng > maxLst2)
                    maxLst2 = cng;
                else if (cng < minLst2)
                    minLst2 = cng;
            }
            maxList1.Text = maxLst1.ToString(CultureInfo.InvariantCulture);
            minList1.Text = minLst1.ToString(CultureInfo.InvariantCulture);

            maxList2.Text = maxLst2.ToString(CultureInfo.InvariantCulture);
            minList2.Text = minLst2.ToString(CultureInfo.InvariantCulture);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _dt = new DataTable();
                _conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                _conn.Open();
                string query = "Select F_nmrh From NmrhFard WHERE F_Drs = 6 AND F_ID = 139701331";
                SqlDataAdapter da = new SqlDataAdapter(query, _conn);
                da.Fill(_dt);
                dataGridViewX1.DataSource = _dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                _conn.Close();
            }
        }
        #endregion
    }
}