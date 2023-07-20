using Android.Content;
using Android.Graphics;
using Android.Hardware.Fingerprints;
using Android.Widget;
using AndroidX.Core.Hardware.Fingerprint;
using AndroidX.Core.OS;
using PerrijosGatijos.Droid;

namespace BiometricUtils
{
	public class FingerprintUiHelper : FingerprintManagerCompat.AuthenticationCallback
    {
        public interface ICallback
        {
            void OnFingerAuthenticated();
            void OnFingerError();
        }

        static readonly long ERROR_TIMEOUT_MILLIS = 1200;
        static readonly long SUCCESS_DELAY_MILLIS = 800;
        static readonly int MAX_TRIES = 3;

        readonly FingerprintManagerCompat mFingerprintManager;
        readonly ImageView mIcon;
        readonly TextView mErrorTextView;
        readonly ICallback mCallback;
        CancellationSignal mCancellationSignal;

        bool mSelfCancelled;
        int tries;
        readonly Context Context;

        public class FingerprintUiHelperBuilder
        {
            FingerprintManagerCompat mFingerPrintManager;
            Context Context;

            public FingerprintUiHelperBuilder(FingerprintManagerCompat fingerprintManager, Context context)
            {
                mFingerPrintManager = fingerprintManager;
                Context = context;
            }

            public FingerprintUiHelper Build(ImageView icon, TextView errorTextView, ICallback callback)
            {
                return new FingerprintUiHelper(mFingerPrintManager, icon, errorTextView, callback, Context);
            }
        }

        /// <summary>
        /// Constructor for {@link FingerprintUiHelper}. This method is expected to be called from
        /// only the {@link FingerprintUiHelperBuilder} class.
        /// </summary>
        /// <param name="fingerprintManager">Fingerprint manager.</param>
        /// <param name="icon">Icon.</param>
        /// <param name="errorTextView">Error text view.</param>
        /// <param name="callback">Callback.</param>
        FingerprintUiHelper(FingerprintManagerCompat fingerprintManager,
            ImageView icon, TextView errorTextView, ICallback callback, Context context)
        {
            mFingerprintManager = fingerprintManager;
            mIcon = icon;
            mErrorTextView = errorTextView;
            mCallback = callback;
            tries = 0;
            Context = context;
        }

        public bool IsFingerprintAuthAvailable
        {
            get
            {
                return mFingerprintManager.IsHardwareDetected
                    && mFingerprintManager.HasEnrolledFingerprints;
            }
        }

        public void StartListening(FingerprintManagerCompat.CryptoObject cryptoObject)
        {
            if (!IsFingerprintAuthAvailable)
                return;
            tries = 0;
            mCancellationSignal = new CancellationSignal();
            mSelfCancelled = false;
            mFingerprintManager.Authenticate(cryptoObject, 0, mCancellationSignal, this, null);
            mErrorTextView.Text = Context.Resources.GetString(Resource.String.fingerprintDialogStatus);
            mIcon?.SetImageResource(Resource.Drawable.icon_finger);
        }

        public void StopListening()
        {
            try
            {
                if (mCancellationSignal != null)
                {
                    mSelfCancelled = true;
                    mCancellationSignal.Cancel();
                    mCancellationSignal = null;
                }
            }
            catch
            {
                //error cancelando
            }
        }

        public override void OnAuthenticationError(int errMsgId, Java.Lang.ICharSequence errString)
        {
            if (!mSelfCancelled)
            {
                ShowError(errString.ToString());
            }
        }

        public override void OnAuthenticationHelp(int helpMsgId, Java.Lang.ICharSequence helpString)
        {
            ShowError(helpString.ToString());
        }

        public override void OnAuthenticationFailed()
        {
            ShowError(Context.Resources.GetString(Resource.String.fingerprintDialogNotRecognized));
        }

        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            mErrorTextView.RemoveCallbacks(ResetErrorTextRunnable);
            mIcon.SetImageResource(Resource.Drawable.icon_finger_ok);
            mErrorTextView.SetTextColor(Color.Black);
            mErrorTextView.Text = Context.Resources.GetString(Resource.String.fingerprintDialogOK);

            mIcon.PostDelayed(() =>
            {
                mCallback.OnFingerAuthenticated();
            }, SUCCESS_DELAY_MILLIS);
        }


        void ShowError(string error)
        {
            mIcon.SetImageResource(Resource.Drawable.icon_finger_wrong);
            mErrorTextView.Text = error;
            mErrorTextView.SetTextColor(Color.Red);
            mErrorTextView.RemoveCallbacks(ResetErrorTextRunnable);
            mErrorTextView.PostDelayed(ResetErrorTextRunnable, ERROR_TIMEOUT_MILLIS);
        }

        void ResetErrorTextRunnable()
        {
            tries++;
            if (tries == MAX_TRIES)
            {
                tries = 0;
                mErrorTextView.Post(() =>
                {
                    mCallback.OnFingerError();
                });
            }
            else
            {
                mErrorTextView.SetTextColor(Color.Black);
                mErrorTextView.Text = Context.Resources.GetString(Resource.String.fingerprintDialogTryAgain);
                mIcon.SetImageResource(Resource.Drawable.icon_finger);
            }
        }
    }
}

