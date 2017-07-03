using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Widget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MySql.Data.MySqlClient;


namespace Pay4Delay
{
    [Activity(Label = "FahrtAnlegenActivity")]
    public class FahrtAnlegenActivity : Activity
    {
        static string currentName = "";
        static List<string> allNames;
        List<string> fahrerNames;
        List<string> teilnehmer = new List<string>();
        ArrayAdapter<string> fahrerSpinnerAdapter;
        ArrayAdapter<string> teilnehmerViewAdapter;
        Dictionary<string, bool> Fahrt = new Dictionary<string, bool>();
        MySqlConnection sqlconn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.neueFahrtLayout);

            ListView fahrtView = FindViewById<ListView>(Resource.Id.fahrtView);

            fahrtView.ItemLongClick += fahrtView_LongClick;

            teilnehmerViewAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, teilnehmer);
            fahrtView.Adapter = teilnehmerViewAdapter;

            string connsqlString = "Server=sql11.freemysqlhosting.net;Port=3306;database=sql11183190;User Id=sql11183190;Password=X4fs1HWamr";
            sqlconn = new MySqlConnection(connsqlString);
            sqlconn.Open();
            string queryString = "select vorname, name from teilnehmer";
            MySqlCommand sqlCmd = new MySqlCommand(queryString, sqlconn);
            MySqlDataReader reader = sqlCmd.ExecuteReader();
            fahrerNames = new List<string>();
            while (reader.Read())
            {
                fahrerNames.Add(reader.GetString(0) + " " + reader.GetString(1));
            }
            reader.Close();
            sqlconn.Close();
            allNames = fahrerNames;

            Spinner fahrerSpinner = FindViewById<Spinner>(Resource.Id.fahrerSpinner);

            Button fahrerButton = FindViewById<Button>(Resource.Id.fahrerButton);
            Button mitfahrerButton = FindViewById<Button>(Resource.Id.mitfahrerButton);
            //Button mitfahrerButton = FindViewById<Button>(Resource.Id.fahrerButton);
            //ListView selectedView = FindViewById<ListView>(Resource.Id.);

            Button anlegenButton = FindViewById<Button>(Resource.Id.anlegenButton);

            anlegenButton.Click += AnlegenButton_Click;


            fahrerButton.Click += (o, e) =>
            {
                FahrerButton_Click();
            };

            mitfahrerButton.Click += (o, e) => { MitfahrerButton_Click(); };
            //var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.names_array, Android.Resource.Layout.SimpleSpinnerItem);

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, fahrerNames);

            fahrerSpinnerAdapter = adapter;

            //adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            fahrerSpinner.Adapter = adapter;

            // Create your application here
        }

        private void AnlegenButton_Click(object sender, EventArgs e)
        {
            int fahrtID = -1;
            foreach (string teilnehmer in Fahrt.Keys)
            {
                if (Fahrt[teilnehmer])
                {
                    var vornachname = teilnehmer.Split(' ');
                    sqlconn.Open();
                    MySqlCommand sqlcom = new MySqlCommand("SELECT teilnehmerID FROM teilnehmer WHERE vorname='" + vornachname[0] + "' AND name='" + vornachname[1] + "'", sqlconn);

                    int fahrerID = 0;
                    MySqlDataReader reader = sqlcom.ExecuteReader();
                    while (reader.Read())
                    {
                        fahrerID = reader.GetInt32(0);
                    }
                    reader.Close();
                    DateTime curr = DateTime.Now;

                    sqlcom = new MySqlCommand("INSERT INTO fahrt (fahrtDatum,teilnehmerID,fahrgemeinschaftsID) VALUES ('" + DateTime.Now.ToString("yyyy-MM-dd") + "'," + fahrerID + ",1)", sqlconn);

                    sqlcom.ExecuteNonQuery();
                    fahrtID = 1;
                    sqlconn.Close();
                }
            }
            if (fahrtID != -1)
            {
                foreach (string teilnehmer in Fahrt.Keys)
                {
                    if (!Fahrt[teilnehmer])
                    {
                        var vornachname = teilnehmer.Split(' ');
                        sqlconn.Open();
                        MySqlCommand sqlcom = new MySqlCommand("SELECT teilnehmerID FROM teilnehmer WHERE vorname='" + vornachname[0] + "' AND name='" + vornachname[1] + "'", sqlconn);

                        int fahrerID = 0;
                        MySqlDataReader reader = sqlcom.ExecuteReader();
                        while (reader.Read())
                        {
                            fahrerID = reader.GetInt32(0);
                        }
                        reader.Close();

                        sqlcom = new MySqlCommand("INSERT INTO fahrtverteilung (fahrtID,teilnehmerID) VALUES ((SELECT max(fahrtID) FROM fahrt)," + fahrerID + ")", sqlconn);
                        sqlcom.ExecuteNonQuery();
                        sqlconn.Close();
                        
                    }
                }
            }
            Toast.MakeText(this, "Fahrt angelegt!", ToastLength.Long).Show();

            base.OnBackPressed();
        }

        private void fahrtView_LongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            currentName = (e.View as TextView).Text;
            Button fahrerButton = FindViewById<Button>(Resource.Id.fahrerButton);
            string origname = currentName;
            if (currentName.Contains("(Fahrer)"))
            {
                currentName = currentName.Replace("  (Fahrer)", "");
                fahrerButton.Enabled = true;
            }
            else if (currentName.Contains("(Mitfahrer)"))
            {
                currentName = currentName.Replace("  (Mitfahrer)", "");
            }

            teilnehmerViewAdapter.Remove(origname);
            teilnehmerViewAdapter.NotifyDataSetChanged();
            fahrerSpinnerAdapter.Add(currentName);
            fahrerSpinnerAdapter.NotifyDataSetChanged();

            Fahrt.Remove(currentName);

        }

        private void MitfahrerButton_Click()
        {
            Spinner fahrerSpinner = FindViewById<Spinner>(Resource.Id.fahrerSpinner);

            currentName = fahrerSpinnerAdapter.GetItem(fahrerSpinner.SelectedItemPosition);
            //fahrerNames.Remove(currentName);
            fahrerSpinnerAdapter.Remove(currentName);

            fahrerSpinnerAdapter.NotifyDataSetChanged();
            Fahrt.Add(currentName, false);

            ListView fahrtView = FindViewById<ListView>(Resource.Id.fahrtView);

            teilnehmerViewAdapter.Add(currentName + "  (Mitfahrer)");
            teilnehmerViewAdapter.NotifyDataSetChanged();
        }

        private void FahrerButton_Click()
        {
            Spinner fahrerSpinner = FindViewById<Spinner>(Resource.Id.fahrerSpinner);
            Button fahrerButton = FindViewById<Button>(Resource.Id.fahrerButton);
            currentName = fahrerSpinnerAdapter.GetItem(fahrerSpinner.SelectedItemPosition);
            //fahrerNames.Remove(currentName);
            fahrerSpinnerAdapter.Remove(currentName);

            fahrerSpinnerAdapter.NotifyDataSetChanged();

            Fahrt.Add(currentName, true);

            fahrerButton.Enabled = false;
            teilnehmerViewAdapter.Add(currentName + "  (Fahrer)");
            teilnehmerViewAdapter.NotifyDataSetChanged();
        }
    }
}