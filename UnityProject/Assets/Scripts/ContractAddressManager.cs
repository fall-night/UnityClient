using Loom.Client;
using UnityEngine;

namespace Assets.Scripts
{
    public class ContractAddressManager : MonoBehaviour
    {

        [SerializeField] private UILabel uiLabel;
        
        private static string ByteArrayToString(byte[] arr)
        {
            return System.Convert.ToBase64String(arr);
        }
        
        private static byte[] StringToByteArray(string str)
        {
            return System.Convert.FromBase64String(str);
        }
        
        public void Start ()
        {
            var privateKey = ReadPrivateKeyFromPlayerPrefs("privateKey");
            var privateKeyString = ByteArrayToString(privateKey);
            this.uiLabel.text = privateKeyString.Substring(0, 8) + "....." + privateKeyString.Substring(80, 8);
            var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);
        }
        
        private static void WritePrivateKeyToPlayerPrefs (string tag, byte[] key)
        {
            // convert byte array to base64 string
            string base64String = ByteArrayToString(key);
            // write string to playerpref
            PlayerPrefs.SetString (tag, base64String);
            PlayerPrefs.Save ();
            
        }

        private static byte[] ReadPrivateKeyFromPlayerPrefs (string tag)
        {
            string base64String = PlayerPrefs.GetString (tag, null);
            byte[] privateKey = null;
            if (string.IsNullOrEmpty (base64String)) {
                privateKey = CryptoUtils.GeneratePrivateKey();
                WritePrivateKeyToPlayerPrefs(tag, privateKey);
            }
            else
            {
                privateKey = StringToByteArray(base64String);
            }

            return privateKey;
        }    
    }
}