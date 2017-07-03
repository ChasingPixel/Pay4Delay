using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
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
        static int currentSpinner = 0;
        static List<string> allNames;
        List<string> fahrerNames;
        List<string> teilnehmer = new List<string>();
        ArrayAdapter<string> fahrerSpinnerAdapter;
        ArrayAdapter<string> teilnehmerViewAdapter;
        Dictionary<string, bool> Fahrt = new Dictionary<string, bool>(); 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.neueFahrtLayout);

            ListView fahrtView = FindViewById<ListView>(Resource.Id.fahrtView);
            teilnehmerViewAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem2, teilnehmer);
            fahrtView.Adapter = teilnehmerViewAdapter;
            MySqlConnection sqlconn;
            string connsqlString = "Server=sql11.freemysqlhosting.net;Port=3306;database=sql11183190;User Id=sql11183190;Password=X4fs1HWamr";
            sqlconn = new MySqlConnection(connsqlString);
            sqlconn.Open();
            string queryString = "select vorname, name from teilnehmer";
            MySqlCommand sqlCmd = new MySqlCommand(queryString, sqlconn);
            MySqlDataReader reader = sqlCmd.ExecuteReader();
            fahrerNames = new List<string>();
            while(reader.Read())
            {
                fahrerNames.Add(reader.GetString(0) + " " + reader.GetString(1));
            }

            allNames = fahrerNames;

            Spinner fahrerSpinner = FindViewById<Spinner>(Resource.Id.fahrerSpinner);
            
            Button fahrerButton = FindViewById<Button>(Resource.Id.fahrerButton);
            Button mitfahrerButton = FindViewById<Button>(Resource.Id.mitfahrerButton);
            //Button mitfahrerButton = FindViewById<Button>(Resource.Id.fahrerButton);
            //ListView selectedView = FindViewById<ListView>(Resource.Id.);
            

            
            
            fahrerButton.Click += (o, e) =>
            {
                fahrerButton_Click();
            };

            mitfahrerButton.Click += (o, e) => { mitfahrerButton_Click(); };
            //var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.names_array, Android.Resource.Layout.SimpleSpinnerItem);

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, fahrerNames);
            
            fahrerSpinnerAdapter = adapter;
            
            //adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            fahrerSpinner.Adapter = adapter;
            
            // Create your application here
        }

        private void mitfahrerButton_Click()
        {
            Spinner fahrerSpinner = FindViewById<Spinner>(Resource.Id.fahrerSpinner);

            currentName = fahrerSpinnerAdapter.GetItem(fahrerSpinner.SelectedItemPosition);
            //fahrerNames.Remove(currentName);
            fahrerSpinnerAdapter.Remove(currentName);

            fahrerSpinnerAdapter.NotifyDataSetChanged();
            Fahrt.Add(currentName, false);

            ListView fahrtView = FindViewById<ListView>(Resource.Id.fahrtView);

            teilnehmerViewAdapter.Add(currentName);
            teilnehmerViewAdapter.NotifyDataSetChanged();
        }

        private void fahrerButton_Click()
        {
            Spinner fahrerSpinner = FindViewById<Spinner>(Resource.Id.fahrerSpinner);
            Button fahrerButton = FindViewById<Button>(Resource.Id.fahrerButton);
            currentName = fahrerSpinnerAdapter.GetItem(fahrerSpinner.SelectedItemPosition);
                //fahrerNames.Remove(currentName);
                fahrerSpinnerAdapter.Remove(currentName);
                
                fahrerSpinnerAdapter.NotifyDataSetChanged();
                
                Fahrt.Add(currentName, true);

            fahrerButton.Enabled = false;
            teilnehmerViewAdapter.Add(currentName);
            teilnehmerViewAdapter.NotifyDataSetChanged();
        }
    }
}