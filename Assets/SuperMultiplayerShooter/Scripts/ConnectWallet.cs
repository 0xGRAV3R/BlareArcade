using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace Visyde
{
    public class ConnectWallet : MonoBehaviour
    {
        public Text Address;

        [DllImport("__Internal")]
        private static extern string connectWalletJS();

        void Start () {
			// if (PlayerPrefs.GetString("wallet_connected") != null && PlayerPrefs.GetString("wallet_connected") == "true") {
            //     string data = PlayerPrefs.GetString("wallet_address");
            //     Address.text = data.Substring(0, 4) + "..." + data.Substring(data.Length - 4);
            // }
		}

        public void connectWallet() {
            connectWalletJS();
        }

        public void ReceiveAddress(string data) {
            Address.text = data.Substring(0, 4) + "..." + data.Substring(data.Length - 4);
            PlayerPrefs.SetString("wallet_address", data);
            PlayerPrefs.SetString("wallet_connected", "true");
        }
    }
}