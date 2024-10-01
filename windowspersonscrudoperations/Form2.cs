using Couchbase;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using Couchbase.Lite.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace windowspersonscrudoperations
{
    public partial class Form2 : Form
    {
        private Form1 _parentForm;
        private Replicator _replicator;
        public string DataFromForm1 { get; set; }
        public Form2(Form1 parentform)
        {
            InitializeComponent();
            UpdateData();
            StartSync();
            _parentForm = parentform;
        }

        public Form2()
        {
            InitializeComponent();
            StartSync();
            UpdateData();
        }


        private async void  button1_Click(object sender, EventArgs e)
        {
            //var cluster = await Cluster.ConnectAsync("couchbase://localhost", "bandaruaneesh", "bandarUbjva3#");
            //var bucket = await cluster.BucketAsync("Persons");
            //var collections = bucket.DefaultCollection();
            var database = new Database("Persons");
            var collection = database.GetDefaultCollection();
            if (Form1.DataFromForm == null && button1.Text == "SaveDetails")
            {
                var mobileNumberToFind = textBox3.Text;
                var query = QueryBuilder
                .Select(SelectResult.All())
                .From(DataSource.Collection(collection))
                .Where(Expression.Property("MobileNumber").EqualTo(Expression.String(mobileNumberToFind)));

                var result = query.Execute();

                if (result.AllResults().Count > 0)
                {
                    if (textBox1.Text.Length >= 5 && textBox2.Text.Length >= 5)
                    {
                        errorProvider1.SetError(textBox3, "provided mobilenumber alredy exist");
                        MessageBox.Show("provided mobilenumber already exist");
                    }
                    else
                    {
                        label4.ForeColor = Color.Red;
                        label5.ForeColor = Color.Red;
                        errorProvider1.SetError(textBox3, "provided mobilenumber alredy exist");
                        MessageBox.Show("provided mobilenumber already exist");
                    }
                }
                else
                {
                    if (textBox1.Text.Length >= 5 && textBox2.Text.Length >= 5)
                    {
                        var document = new MutableDocument();
                        var id = Guid.NewGuid().ToString();
                        document.SetString("Id", id)
                                .SetString("FirstName", textBox1.Text)
                                .SetString("LastName", textBox2.Text)
                                .SetString("MobileNumber", textBox3.Text);

                        collection.Save(document);
                        //collections.UpsertAsync(document.Id, document);
                        textBox1.Text = document.Id;
                        //StartSync();
                        _parentForm?.RefreshData();
                        this.Close();
                    }

                    else
                    {
                        label4.ForeColor = Color.Red;
                        label5.ForeColor = Color.Red;
                    }
                }
            }
            else
            {
                using (var database1 = new Database("Persons"))
                {
                    var query = QueryBuilder
                                    .Select(SelectResult.Expression(Meta.ID))
                                    .From(DataSource.Collection(database1.GetDefaultCollection()))
                                    .Where(Expression.Property("Id").EqualTo(Expression.String(Form1.DataFromForm)));
                    var query1 = QueryBuilder
                                .Select(SelectResult.All())
                                .From(DataSource.Collection(database1.GetDefaultCollection()))
                                .Where(Expression.Property("MobileNumber").EqualTo(Expression.String(textBox3.Text)));
                    var result2 = query1.Execute().AllResults();
                    if(result2.Count < 1)
                    {
                        foreach (var result in query.Execute().AllResults())
                        {
                                if (result != null && textBox1.Text.Length >= 5 && textBox2.Text.Length >= 5)
                                {
                                    //var data = JObject.Parse(result.ToJSON())["_default"];
                                    var docId = result[0];
                                    var document = database.GetDocument(docId.ToString());
                                    var mutableDoc = document.ToMutable();
                                    mutableDoc.SetString("FirstName", textBox1.Text);
                                    mutableDoc.SetString("LastName", textBox2.Text);
                                    mutableDoc.SetString("MobileNumber", textBox3.Text);
                                    database.Save(mutableDoc);
                                    
                                    _parentForm?.RefreshData();
                                    textBox1.Text = "";
                                    textBox2.Text = "";
                                    textBox3.Text = "";
                                    button1.Text = "SaveDetails";
                                this.Close();
                                }
                                else
                                {
                                    if (textBox1.Text.Length < 5)
                                    {
                                        label4.ForeColor = Color.Red;
                                    }
                                    else
                                    {
                                        if(textBox2.Text.Length < 5)
                                        {
                                            label5.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            errorProvider2.SetError(textBox3, "provided mobilenumber alredy exist");
                                            MessageBox.Show("provided number already exist");
                                        }
                                    }
                                    
                                }
                        }
                    }
                    else
                    {
                        foreach (var result in query.Execute().AllResults())
                        {
                            foreach (var result1 in query1.Execute().AllResults())
                            {
                                var data = JObject.Parse(result.ToJSON())["_default"];
                                var docId1 = result1.GetString(0);
                                var nresult = JObject.Parse(result1.ToJSON())["_default"];
                                string Id = (string)nresult["MobileNumber"];
                                if (result != null && Id == Form1.numberFromForm && textBox1.Text.Length >= 5 && textBox2.Text.Length >= 5)
                                {
                                    var docId = result[0];
                                    //var data = JObject.Parse(result.ToJSON())["_default"];
                                    var document = database.GetDocument(docId.ToString());
                                    var mutableDoc = document.ToMutable();
                                    mutableDoc.SetString("FirstName", textBox1.Text);
                                    mutableDoc.SetString("LastName", textBox2.Text);
                                    mutableDoc.SetString("MobileNumber", textBox3.Text);
                                    database.Save(mutableDoc);
                                    button1.Text = "SaveDetails";
                                    _parentForm?.RefreshData();
                                    textBox1.Text = "";
                                    textBox2.Text = "";
                                    textBox3.Text = "";
                                    button1.Text = "SaveDetails";
                                    this.Close();
                                }
                                else
                                {
                                    if (textBox1.Text.Length  < 5)
                                    {
                                        label4.ForeColor = Color.Red;
                                    }
                                    else
                                    {
                                        if (textBox2.Text.Length < 5)
                                        {
                                            label5.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            errorProvider2.SetError(textBox3, "provided mobilenumber alredy exist");
                                            MessageBox.Show("provided number already exist");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void StartSync()
        {
            var database = new Database("Persons");
            var collection = database.GetDefaultCollection();
            var targetUri = new Uri("ws://localhost:8091/Persons");
            var targetEndpoint = new URLEndpoint(targetUri);
            var replConfig = new ReplicatorConfiguration(database,targetEndpoint)
            {
                Authenticator = new BasicAuthenticator("bandaruaneesh","bandarUbjva3#"),
                ReplicatorType = ReplicatorType.PushAndPull
            };
            replConfig.AddCollection(collection);
            _replicator = new Replicator(replConfig);
            _replicator.AddChangeListener((sender, args) =>
            {
                if (args.Status.Error != null)
                {
                    Console.WriteLine($"Error: {args.Status.Error}");
                }
                else
                {
                    Console.WriteLine($"Status: {args.Status.Activity}");
                }
            });
            _replicator.Start();
        }

        public void UpdateData()
        {
            var database = new Database("Persons");
            var query = QueryBuilder
                            .Select(SelectResult.All())
                            .From(DataSource.Collection(database.GetDefaultCollection()))
                            .Where(Expression.Property("Id").EqualTo(Expression.String(Form1.DataFromForm)));
            foreach (var result in query.Execute().AllResults())
            {
                if (result != null)
                {
                    var data = JObject.Parse(result.ToJSON())["_default"];
                    textBox1.Text = (string)data["FirstName"];
                    textBox2.Text = (string)data["LastName"];
                    textBox3.Text = (string)data["MobileNumber"];
                }
            }
        }

        private void Form2_Load_1(object sender, EventArgs e)
        {
            if (Form1.DataFromForm == null)
            {
                button1.Text = "SaveDetails";
            }
            else
            {
                button1.Text = "Update";
            }
        }
    }
}
