using Couchbase.Lite;
using Couchbase.Lite.Query;
using Couchbase.Lite.Sync;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection.Metadata;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace windowspersonscrudoperations
{
    public partial class Form1 : Form
    {
        private Replicator _replicator;
        public static string DataFromForm;
        public static string numberFromForm;
        private Database _database;
        DataTable dataTable = new DataTable();
        //Form2 frm = new Form2();
        public Form1()
        {
            InitializeComponent();
            StartSync();
            InitializeDataGridView();
            LoadData();
        }

        private void InitializeDataGridView()
        {
            // Initialize the DataGridView columns
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "Id",
                DataPropertyName = "Id",
                Visible = true
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FirstName",
                HeaderText = "First Name",
                DataPropertyName = "FirstName"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LastName",
                HeaderText = "Last Name",
                DataPropertyName = "LastName"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MobileNumber",
                HeaderText = "Mobile Number",
                DataPropertyName = "MobileNumber"
            });
            dataGridView1.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "ActionButton",
                HeaderText = "Action",
                Text = "Delete",
                UseColumnTextForButtonValue = true
            });
        }
        private void StartSync()
        {
            var database = new Database("Persons");
            var targetUri = new Uri("ws://localhost:4984/db");
            var targetEndpoint = new URLEndpoint(targetUri);

            var replConfig = new ReplicatorConfiguration(database, targetEndpoint)
            {
                ReplicatorType = ReplicatorType.PushAndPull,
                Authenticator = new BasicAuthenticator("bandaruaneesh", "bandarUbjva3#")
            };

            _replicator = new Replicator(replConfig);

            _replicator.Start();
        }

        private void LoadData()
        {
            DataTable dt = Retrivedatafromdatabase();
            populatedatagridview(dt);
        }

        public void RefreshData()
        {
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2(this);
            frm.ShowDialog();
        }

        private void populatedatagridview(DataTable dt)
        {
            dataGridView1.DataSource = dt;
            dataGridView1.Columns["Id"].Visible = true;
            if (!dataGridView1.Columns.Contains("ActionButton"))
            {
                DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                buttonColumn.Name = "ActionButton";
                buttonColumn.HeaderText = "Action";
                buttonColumn.Text = "Delete";
                buttonColumn.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonColumn);
            }
        }
        private DataTable Retrivedatafromdatabase()
        {
            _database = new Database("persons");
            var collection = _database.GetDefaultCollection();

            string id = "-YNNLLUO570aOcKmUTsVbkQ";
            if (dataTable.Columns.Count == 0)
            {
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("FirstName", typeof(string));
                dataTable.Columns.Add("LastName", typeof(string));
                dataTable.Columns.Add("MobileNumber", typeof(string));
            }
            else
            {
                dataTable.Columns.Clear();
                dataTable.Rows.Clear();
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("FirstName", typeof(string));
                dataTable.Columns.Add("LastName", typeof(string));
                dataTable.Columns.Add("MobileNumber", typeof(string));
            }

            using (var query = QueryBuilder
                                .Select(SelectResult.All())
                                .From(DataSource.Collection(_database.GetDefaultCollection())))
            {
                foreach (var result in query.Execute().AllResults())
                {
                    var docId = result.GetString(0);
                    var nresult = JObject.Parse(result.ToJSON())["_default"];
                    string Id = (string)nresult["Id"];
                    string FirstName = (string)nresult["FirstName"];
                    string LastName = (string)nresult["LastName"];
                    string MobileNumber = (string)nresult["MobileNumber"];

                    DataRow dataRow = dataTable.NewRow();
                    dataRow["ID"] = Id;
                    dataRow["FirstName"] = FirstName;
                    dataRow["LastName"] = LastName;
                    dataRow["MobileNumber"] = MobileNumber;
                    dataTable.Rows.Add(dataRow);
                }
            };
            return dataTable;
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex != dataGridView1.Columns["ActionButton"].Index)
            {
                string id = dataTable.Rows[e.RowIndex]["Id"].ToString();
                DataFromForm = id;
                numberFromForm = dataTable.Rows[e.RowIndex]["MobileNumber"].ToString();
                Form2 frm = new Form2(this);
                frm.ShowDialog();
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "FirstName")
            {
                dataTable.DefaultView.Sort = "FirstName ASC, LastName ASC";
                dataGridView1.DataSource = dataTable;
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "LastName")
            {
                dataTable.DefaultView.Sort = "LastName ASC, FirstName ASC";
                dataGridView1.DataSource = dataTable;
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Mobilenumber")
            {
                dataTable.DefaultView.Sort = "MobileNumber ASC";
                dataGridView1.DataSource = dataTable;
            }
            if (e.ColumnIndex == dataGridView1.Columns["ActionButton"].Index && e.RowIndex >= 0)
            {
                string id = dataTable.Rows[e.RowIndex]["Id"].ToString();
                DataFromForm = id;
                DeleteRowData();
            }
        }

        private void DeleteRowData()
        {
            var query = QueryBuilder
                .Select(SelectResult.Expression(Meta.ID))
                .From(DataSource.Collection(_database.GetDefaultCollection()))
                .Where(Expression.Property("Id").EqualTo(Expression.String(Form1.DataFromForm)));

            var results = query.Execute().AllResults();
            foreach (var result in results)
            {
                string docid = result.GetString(0);
                var doc = _database.GetDocument(docid);
                if (doc != null)
                {
                    _database.Delete(doc);
                    MessageBox.Show("Document Deleted");
                }
                LoadData();
            }
        }
    }
}
