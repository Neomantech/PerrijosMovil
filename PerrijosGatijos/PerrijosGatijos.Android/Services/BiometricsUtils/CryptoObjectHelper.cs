using System;
using Android.Security.Keystore;
using AndroidX.Core.Hardware.Fingerprint;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;

namespace BiometricUtils
{
	public class CryptoObjectHelper
	{
        public static readonly string KEY_NAME = "perrijos";
        static readonly string KEYSTORE_NAME = "AndroidKeyStore";

        // Should be no need to change these values.
        static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;
        static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" +
                                                BLOCK_MODE + "/" +
                                                ENCRYPTION_PADDING;
        readonly KeyStore _keystore;

        public CryptoObjectHelper()
        {
            _keystore = KeyStore.GetInstance(KEYSTORE_NAME);
            _keystore.Load(null);
        }

        public KeyStore KeyStore
        {
            get
            {
                return _keystore;
            }
        }

        public FingerprintManagerCompat.CryptoObject BuildCryptoObject(CipherMode mode, string user, IvParameterSpec ivParams = null)
        {
            Cipher cipher = CreateCipher(true, mode, user, ivParams);
            return new FingerprintManagerCompat.CryptoObject(cipher);
        }

        public Cipher CreateCipher(bool retry = true,
                                   CipherMode mode = CipherMode.DecryptMode,
                                   string key_name = null,
                                  IvParameterSpec ivParams = null)
        {
            string theKeyName = key_name ?? KEY_NAME;

            IKey key = GetKey(theKeyName, mode);
            Cipher cipher = Cipher.GetInstance(KeyProperties.KeyAlgorithmAes + "/"
                    + KeyProperties.BlockModeCbc + "/"
                    + KeyProperties.EncryptionPaddingPkcs7);
            try
            {
                if (ivParams != null)
                {
                    cipher.Init(mode, key, ivParams);
                }
                else
                {
                    cipher.Init(mode, key);
                }
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                _keystore.DeleteEntry(theKeyName);
                if (retry)
                {
                    CreateCipher(false, mode, theKeyName, ivParams);
                }
                else
                {
                    throw new Exception("Could not create the cipher for fingerprint authentication.", e);
                }
            }
            return cipher;
        }

        public IKey GetKey(string key_name, CipherMode mode)
        {
            IKey secretKey;

            if (!_keystore.IsKeyEntry(key_name))
            {
                CreateKey(key_name);
            }

            secretKey = _keystore.GetKey(key_name, null);
            return secretKey;
        }

        public void CreateKey(string key_name)
        {
            KeyGenerator keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KEYSTORE_NAME);
            _keystore.Load(null);
            // Set the alias of the entry in Android KeyStore where the key will appear
            // and the constrains (purposes) in the constructor of the Builder
            keyGen.Init(new KeyGenParameterSpec.Builder(key_name,
                KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                .SetBlockModes(KeyProperties.BlockModeCbc)
                        // Require the user to authenticate with a fingerprint to authorize every use
                        // of the key
                        .SetUserAuthenticationRequired(key_name.Equals(KEY_NAME))
                .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
                .Build());
            keyGen.GenerateKey();
        }
    }
}

