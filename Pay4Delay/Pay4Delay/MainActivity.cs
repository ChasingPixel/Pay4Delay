using Android.App;
using Android.Widget;
using Android.OS;

namespace Pay4Delay
{
    [Activity(Label = "Pay4Delay", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Button neueFahrtButton = FindViewById<Button>(Resource.Id.neueFahrtButton);

            neueFahrtButton.Click += (o, e) =>
            {
                StartActivity(typeof(FahrtAnlegenActivity));
            };

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

