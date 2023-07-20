using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Hardware.Fingerprint;
using AndroidX.Fragment.App;
using PerrijosGatijos.Droid;

namespace BiometricUtils
{
	public class FingerPrintDialogFragment : DialogFragment, FingerprintUiHelper.ICallback
    {
        public const string TAG = "FINGER_PRINT_DIALOG";

        public event EventHandler OnAuthenticated;
        public event EventHandler OnError;
        public event EventHandler OnDismissed;
        private string title;

        FingerprintManagerCompat mFingerprintManager;
        ImageView ivFingerprintIcon;
        TextView tvFingerprintDescription, tvFingerprintStatus;

        Button btCancel;

        FingerprintManagerCompat.CryptoObject mCryptoObject;
        FingerprintUiHelper mFingerprintUIHelper;

        public void UpdateStatusBarColor(Color statusColor, Color navigationColor, bool light = true)
        {

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                Window window = Dialog.Window;
                window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                window.ClearFlags(WindowManagerFlags.TranslucentNavigation);
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                if (light)
                    window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                else
                    window.DecorView.SystemUiVisibility = 0;
                window.SetStatusBarColor(statusColor);
                window.SetNavigationBarColor(navigationColor);
            }
        }

        public FingerPrintDialogFragment(string title)
        {
            this.title = title;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Do not create a new Fragment when the Activity is re-created such as orientation changes.
            RetainInstance = true;

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
                Bundle savedInstanceState)
        {
            UpdateStatusBarColor(Color.White, Color.White);
            if (Context is AppCompatActivity)
            {
                var appcompatActivity = Context as AppCompatActivity;
                mFingerprintManager = FingerprintManagerCompat.From(appcompatActivity);
            }

            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            View view = inflater.Inflate(Resource.Layout.fingerprint_dialog_fragment, container, false);
            btCancel = view.FindViewById<Button>(Resource.Id.btCancel);
            btCancel.Text = Context.Resources.GetString(Resource.String.msg_cancel);
            ivFingerprintIcon = view.FindViewById<ImageView>(Resource.Id.ivFingerprintIcon);
            tvFingerprintDescription = view.FindViewById<TextView>(Resource.Id.tvFingerprintDescription);
            tvFingerprintStatus = view.FindViewById<TextView>(Resource.Id.tvFingerprintStatus);
            tvFingerprintDescription.Text = title;
            var builder = new FingerprintUiHelper.FingerprintUiHelperBuilder(mFingerprintManager, Context);
            mFingerprintUIHelper = builder.Build(ivFingerprintIcon, tvFingerprintStatus, this);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            // Make sure there is no background behind our view
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            // Disable standard dialog styling/frame/theme: our custom view should create full UI
            SetStyle(DialogFragment.StyleNoFrame, Android.Resource.Style.Theme);

            Dialog.SetCancelable(false);
            mFingerprintUIHelper?.StartListening(mCryptoObject);

            btCancel.Click += BtCancel_Click;
        }

        public override void OnPause()
        {
            base.OnPause();
            mFingerprintUIHelper.StopListening();
            btCancel.Click -= BtCancel_Click;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.bottom_up_dialog_animation;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
        }

        /**
		 * Sets the crypto object to be passed in when authenticating with fingerprint.
		 */
        public void SetCryptoObject(FingerprintManagerCompat.CryptoObject cryptoObject)
        {
            mCryptoObject = cryptoObject;
        }

        void BtCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
            if (OnDismissed != null)
                OnDismissed(this, null);
        }

        #region MyFingerprintUiHelper.ICallbackRegion

        public void OnFingerAuthenticated()
        {
            if (OnAuthenticated != null)
                OnAuthenticated(this, null);
            if (Activity != null && !Activity.IsDestroyed)
                DismissAllowingStateLoss();
        }

        public void OnFingerError()
        {
            if (OnError != null)
                OnError(this, null);
            if (Activity != null && !Activity.IsDestroyed)
                DismissAllowingStateLoss();
        }
        #endregion
    }
}

