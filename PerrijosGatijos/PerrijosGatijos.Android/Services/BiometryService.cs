using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.Core.Hardware.Fingerprint;
using BiometricUtils;
using Biometry.Core.Services;
using PerrijosGatijos.Droid;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Newtonsoft.Json;
using Plugin.CurrentActivity;

namespace Biometry.Droid.Services
{
	public class BiometryService: IBiometryService
    {
        private const String STORE_NAME = "PERRIJOS";
        private FingerPrintDialogFragment fingerPrintDialog;

        public Task<BiometryStorageResult> AddOrUpdateItemAsync<T>(string key, T data)
        {
            var activity = CrossCurrentActivity.Current.Activity as AppCompatActivity;
            var completionSource = new TaskCompletionSource<BiometryStorageResult>();

            string androidID = Android.Provider.Settings.Secure.GetString(activity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            CryptoObjectHelper cryptoObjectHelper = new CryptoObjectHelper();
            fingerPrintDialog = new FingerPrintDialogFragment(activity.Resources.GetString(Resource.String.fingerprintDialogRegister));
            fingerPrintDialog.SetCryptoObject(cryptoObjectHelper.BuildCryptoObject(CipherMode.EncryptMode, androidID));
            fingerPrintDialog.OnAuthenticated += delegate
            {
                var cipher = cryptoObjectHelper.CreateCipher(true, CipherMode.EncryptMode, androidID);
                string jsonData = JsonConvert.SerializeObject(data);
                var encoded = cipher.DoFinal(Encoding.ASCII.GetBytes(jsonData));
                activity.GetSharedPreferences(STORE_NAME, FileCreationMode.Private).Edit()
                        .PutString(key, Base64.EncodeToString(encoded, Base64Flags.Default))
                        .PutString(key + "_encryption_iv", Base64.EncodeToString(cipher.GetIV(), Base64Flags.Default))
                        .Commit();
                completionSource.TrySetResult(BiometryStorageResult.Success);
            };
            fingerPrintDialog.OnDismissed += delegate
            {
                completionSource.TrySetResult(BiometryStorageResult.Cancelled);
            };
            fingerPrintDialog.OnError += delegate
            {
                completionSource.TrySetResult(BiometryStorageResult.UnknownError);
            };

            fingerPrintDialog.Show(activity.SupportFragmentManager, "DIALOG_FRAGMENT_TAG");

            return completionSource.Task;
        }


        public Task<BiometryAuthResult> Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<BiometryAuthResult> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(string key)
        {
            var activity = CrossCurrentActivity.Current.Activity as AppCompatActivity;
            activity.GetSharedPreferences(STORE_NAME, FileCreationMode.Private).Edit()
                                .PutString(key, null)
                                .Commit();
        }

        public BiometryType GetBiometryTypeAvailable()
        {
            var activity = CrossCurrentActivity.Current.Activity as AppCompatActivity;
            KeyguardManager keyguardmanager = (KeyguardManager)activity.GetSystemService(Context.KeyguardService);
            FingerprintManagerCompat fingerprintManager = FingerprintManagerCompat.From(activity);

            if (ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.UseFingerprint) == (int)Android.Content.PM.Permission.Granted)
            {

                if (!fingerprintManager.IsHardwareDetected)
                {
                    return BiometryType.NONE;
                }
                else
                {
                    if (!fingerprintManager.HasEnrolledFingerprints)
                    {
                        return BiometryType.NONE;
                    }
                    else
                    {
                        if (!keyguardmanager.IsKeyguardSecure)
                        {
                            throw new BiometryNotSecuredException();
                        }
                        else
                        {
                            return BiometryType.TOUCH_ID;
                        }

                    }
                }
            }
            return BiometryType.NONE;
        }

        public Task<Tuple<BiometryStorageResult, T>> GetItemAsync<T>(string key)
        {
            var activity = CrossCurrentActivity.Current.Activity as AppCompatActivity;
            var completionSource = new TaskCompletionSource<Tuple<BiometryStorageResult, T>>();

            string androidID = Android.Provider.Settings.Secure.GetString(activity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            CryptoObjectHelper cryptoObjectHelper = new CryptoObjectHelper();
            var mPreferences = activity.GetSharedPreferences(STORE_NAME, FileCreationMode.Private);
            string encodedIv = mPreferences.GetString(key + "_encryption_iv", null);
            byte[] bytes = Base64.Decode(encodedIv, Base64Flags.Default);
            IvParameterSpec ivParams = new IvParameterSpec(bytes);

            fingerPrintDialog = new FingerPrintDialogFragment(activity.Resources.GetString(Resource.String.fingerprintDialogAccess));
            fingerPrintDialog.SetCryptoObject(cryptoObjectHelper.BuildCryptoObject(CipherMode.DecryptMode, androidID, ivParams));
            fingerPrintDialog.OnAuthenticated += delegate
            {
                string base64item = mPreferences.GetString(key, null);
                byte[] encryptedItem = Base64.Decode(base64item, Base64Flags.Default);
                var decoded = cryptoObjectHelper.CreateCipher(true, CipherMode.DecryptMode, androidID, ivParams).DoFinal(encryptedItem);
                string jsonItem = Encoding.UTF8.GetString(decoded);
                var output = JsonConvert.DeserializeObject<T>(jsonItem);
                completionSource.TrySetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.Success, output));
            };
            fingerPrintDialog.OnDismissed += delegate
            {
                completionSource.TrySetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.Cancelled, default(T)));
            };
            fingerPrintDialog.OnError += delegate
            {
                completionSource.TrySetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.UnknownError, default(T)));
            };
            fingerPrintDialog.Show(activity.SupportFragmentManager, "DIALOG_FRAGMENT_TAG");
            return completionSource.Task;
        }

        public bool IsAvailableAsync()
        {
            return GetBiometryTypeAvailable() == BiometryType.TOUCH_ID;
        }
    }
}

