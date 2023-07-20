using System;
using System.Threading.Tasks;

namespace Biometry.Core.Services
{
    public enum BiometryType
    {
        TOUCH_ID,
        FACE_ID,
        NONE
    }

    public enum BiometryAuthResult
    {
        BIOMETRY_NOT_AVAILABLE,
        BIOMETRY_NOT_ENROLLED,
        BIOMETRY_USER_FALLBACK,
        BIOMETRY_FAILED,
        BIOMETRY_SUCCESS
    }

    public enum BiometryStorageResult
    {
        ItemNotFound,
        AuthFailed,
        UnknownError,
        Success,
        Cancelled
    }

    public interface IBiometryService
	{
        BiometryType GetBiometryTypeAvailable();
        Task<BiometryAuthResult> AuthenticateAsync();
        Task<BiometryStorageResult> AddOrUpdateItemAsync<T>(string key, T data);
        Task<Tuple<BiometryStorageResult, T>> GetItemAsync<T>(string key);
        void DeleteItem(string key);
        bool IsAvailableAsync();
    }
}

