using System;
using System.Text;
using System.Threading.Tasks;
using Biometry.Core.Services;
using CoreFoundation;
using Foundation;
using LocalAuthentication;
using Newtonsoft.Json;
using Security;
using UIKit;
using ObjCRuntime;
using System.Diagnostics;

namespace PerrijosGatijos.iOS.Services
{
	public class BiometryService : IBiometryService
    {
        private LAContext context;

        public BiometryService()
		{
            context = new LAContext();
        }

        #region Biometry

        public BiometryType GetBiometryTypeAvailable()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var error);
                if (error != null && error.Code != (long)LAStatus.Success)
                {
                    switch (error.Code)
                    {
                        case (long)LAStatus.BiometryNotEnrolled:
                            return BiometryType.NONE;
                        default:
                            return BiometryType.NONE;
                    }
                }
                switch (context.BiometryType)
                {
                    case LABiometryType.TouchId:
                        return BiometryType.TOUCH_ID;
                    case LABiometryType.FaceId:
                        return BiometryType.FACE_ID;
                    case LABiometryType.None:
                        return BiometryType.NONE;
                    default:
                        return BiometryType.NONE;
                }
            }
            else
            {
                var result = context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var error);
                if (result)
                {
                    return BiometryType.TOUCH_ID;
                }

                return BiometryType.NONE;
            }
        }

        public async Task<BiometryAuthResult> AuthenticateAsync()
        {
            var result = await context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, NSBundle.MainBundle.GetLocalizedString("biometry_auth_prompt"));
            var success = result.Item1;
            if (success)
            {
                return BiometryAuthResult.BIOMETRY_SUCCESS;
            }

            var error = result.Item2;
            switch (error.Code)
            {
                case (long)LAStatus.AuthenticationFailed:
                    return BiometryAuthResult.BIOMETRY_FAILED;
                case (long)LAStatus.BiometryNotAvailable:
                    return BiometryAuthResult.BIOMETRY_NOT_AVAILABLE;
                case (long)LAStatus.BiometryNotEnrolled:
                    return BiometryAuthResult.BIOMETRY_NOT_ENROLLED;
                case (long)LAStatus.UserFallback:
                    return BiometryAuthResult.BIOMETRY_USER_FALLBACK;
                default:
                    return BiometryAuthResult.BIOMETRY_NOT_AVAILABLE;
            }
        }

        #endregion

        #region Keychain

        private Task<BiometryStorageResult> UpdateItemAsync(string key, byte[] data)
        {
            var task = new TaskCompletionSource<BiometryStorageResult>();

            DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() => {
                var securityRecord = new SecRecord(SecKind.GenericPassword)
                {
                    Service = key,
                    UseOperationPrompt = NSBundle.MainBundle.GetLocalizedString("settings_biometry_activated_succesfully")
                };

                var recordUpdate = new SecRecord
                {
                    ValueData = NSData.FromArray(data)
                };

                var status = SecKeyChain.Update(securityRecord, recordUpdate);
                switch (status)
                {
                    case SecStatusCode.Success:
                        task.SetResult(BiometryStorageResult.Success);
                        break;
                    case SecStatusCode.AuthFailed:
                        task.SetResult(BiometryStorageResult.AuthFailed);
                        break;
                    case SecStatusCode.ItemNotFound:
                        task.SetResult(BiometryStorageResult.ItemNotFound);
                        break;
                    case SecStatusCode.UserCanceled:
                        task.SetResult(BiometryStorageResult.Cancelled);
                        break;
                    default:
                        task.SetResult(BiometryStorageResult.UnknownError);
                        break;
                }
            });

            return task.Task;
        }

        public Task<BiometryStorageResult> AddOrUpdateItemAsync<T>(string key, T data)
        {
            var task = new TaskCompletionSource<BiometryStorageResult>();
            DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() =>
            {
                try
                {
                    SecAccessControl secObject;
                    SecRecord secRecord;
                    var json = JsonConvert.SerializeObject(data);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);

                    if (Runtime.Arch == Arch.SIMULATOR)
                        secRecord = new SecRecord(SecKind.GenericPassword) { Service = key, ValueData = NSData.FromArray(jsonBytes) };
                    else
                    {
                        secObject = new SecAccessControl(SecAccessible.WhenUnlockedThisDeviceOnly, SecAccessControlCreateFlags.BiometryCurrentSet);
                        secRecord = new SecRecord(SecKind.GenericPassword) { Service = key, ValueData = NSData.FromArray(jsonBytes), AccessControl = secObject };
                    }

                    SecStatusCode status = SecKeyChain.Add(secRecord);
                    switch (status)
                    {
                        case SecStatusCode.Success:
                            task.SetResult(BiometryStorageResult.Success);
                            break;
                        case SecStatusCode.DuplicateItem:
                            task.SetResult(UpdateItemAsync(key, jsonBytes).Result);
                            break;
                        case SecStatusCode.UserCanceled:
                            task.SetResult(BiometryStorageResult.Cancelled);
                            break;
                        default:
                            task.SetResult(BiometryStorageResult.UnknownError);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    task.SetResult(BiometryStorageResult.UnknownError);
                    Debug.WriteLine($"Class Name: {nameof(AddOrUpdateItemAsync)}");
                    Debug.WriteLine(ex);
                }
            });

            return task.Task;
        }

        public Task<Tuple<BiometryStorageResult, T>> GetItemAsync<T>(string key)
        {
            var task = new TaskCompletionSource<Tuple<BiometryStorageResult, T>>();

            DispatchQueue.MainQueue.DispatchAsync(() => {
                var securityRecord = new SecRecord(SecKind.GenericPassword)
                {
                    Service = key,
                    UseOperationPrompt = NSBundle.MainBundle.GetLocalizedString("fingerprintDialogAccess")
                };

                NSData resultData = SecKeyChain.QueryAsData(securityRecord, false, out SecStatusCode status);
                switch (status)
                {
                    case SecStatusCode.Success:
                        var output = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(resultData.ToArray()));
                        task.SetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.Success, output));
                        break;
                    case SecStatusCode.ItemNotFound:
                        task.SetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.ItemNotFound, default(T)));
                        break;
                    case SecStatusCode.AuthFailed:
                        task.SetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.AuthFailed, default(T)));
                        break;
                    case SecStatusCode.UserCanceled:
                        task.SetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.Cancelled, default(T)));
                        break;
                    default:
                        task.SetResult(new Tuple<BiometryStorageResult, T>(BiometryStorageResult.UnknownError, default(T)));
                        break;
                }
            });

            return task.Task;
        }

        public void DeleteItem(string key)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var securityRecord = new SecRecord(SecKind.GenericPassword)
                {
                    Service = key
                };

                SecKeyChain.Remove(securityRecord);
            });
        }


        public bool IsAvailableAsync()
        {
            var type = GetBiometryTypeAvailable();
            return (type == BiometryType.TOUCH_ID) || (type == BiometryType.FACE_ID);
        }

        #endregion
    }
}

