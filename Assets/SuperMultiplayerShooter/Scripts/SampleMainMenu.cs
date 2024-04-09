using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Visyde
{
    /// <summary>
    /// Sample Main Menu
    /// - A sample script that handles the main menu UI.
    /// </summary>

    [System.Serializable]
    public class Item
    {
        public int id;
        public string address;
        public string name;
        public int score;
    }

    public class SampleMainMenu : MonoBehaviour
    {
        [Header("Main:")]
        public Text connectionStatusText;
        public Button findMatchBTN;
        public Button customMatchBTN;
        public GameObject findMatchCancelButtonObj;
        public GameObject findingMatchPanel;
        public GameObject customGameRoomPanel;
        public Text matchmakingPlayerCountText;
        public InputField playerNameInput;
        public GameObject messagePopupObj;
        public Text messagePopupText;
        public GameObject characterSelectionPanel;
        public Image characterIconPresenter;
        public GameObject loadingPanel;
        public Toggle frameRateSetting;
        public GameObject payArcadePopUp;
        public GameObject leaderboardObj;
        public LeaderboardItem leaderboardItem;
        public Transform leaderboardContent;
        public Text ArcadeBalance;
        public Text WalletAddress;

        private string apiUrl = "https://game-manager-aececf358025.herokuapp.com/ammo_arcade_sol/";
        // private string apiUrl = "http://localhost:3000/ammo_arcade/";
        private string secretKey = "seaseacretekeybill!@#QWE";
        private int page = 1;

        [DllImport("__Internal")]
        private static extern string payArcadeJS();
        [DllImport("__Internal")]
        private static extern string getArcadeBalanceJS();
        [DllImport("__Internal")]
        private static extern string checkConnectedJS();

        void Awake(){
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        // Use this for initialization
        void Start()
        {
            // Load or create a username:
            if (PlayerPrefs.HasKey("name"))
            {
                playerNameInput.text = PlayerPrefs.GetString("name");
            }
            else
            {
                playerNameInput.text = "Player" + Random.Range(0, 9999);
            }
            SetPlayerName();

            // Others:
            frameRateSetting.isOn = Application.targetFrameRate == 60;
            PlayerPrefs.SetString("wallet_connected", "false");
            checkConnectedJS();
        }

        // Update is called once per frame
        void Update()
        {
            bool connecting = !PhotonNetwork.IsConnectedAndReady || PhotonNetwork.NetworkClientState == ClientState.ConnectedToNameServer || PhotonNetwork.InRoom;

            // Handling texts:
            connectionStatusText.text = connecting ? PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer ? "Connecting..." : "Finding network..."
                : "Connected! (" + PhotonNetwork.CloudRegion + ") | Ping: " + PhotonNetwork.GetPing();
            connectionStatusText.color = PhotonNetwork.IsConnectedAndReady ? Color.green : Color.yellow;
            matchmakingPlayerCountText.text = PhotonNetwork.InRoom ? Connector.instance.totalPlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers : "Matchmaking...";

            // Handling buttons:
            customMatchBTN.interactable = !connecting;
            findMatchBTN.interactable = !connecting;
            findMatchCancelButtonObj.SetActive(PhotonNetwork.InRoom);

            // Handling panels:
            customGameRoomPanel.SetActive(Connector.instance.isInCustomGame);
            loadingPanel.SetActive(PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer || PhotonNetwork.NetworkClientState == ClientState.DisconnectingFromGameServer);

            // Messages popup system (used for checking if we we're kicked or we quit the match ourself from the last game etc):
            if (DataCarrier.message.Length > 0)
            {
                messagePopupObj.SetActive(true);
                messagePopupText.text = DataCarrier.message;
                DataCarrier.message = "";
            }
            if (PlayerPrefs.GetString("wallet_connected") == "true") {
                // getArcadeBalanceJS();
                string data = PlayerPrefs.GetString("wallet_address");
                WalletAddress.text = data.Substring(0, 4) + "..." + data.Substring(data.Length - 4);
            }
        }

        // Profile:
        public void SetPlayerName()
        {
            PlayerPrefs.SetString("name", playerNameInput.text);
            PhotonNetwork.NickName = playerNameInput.text;
        }

        public void PayArcade() {
            payArcadeJS();
        }

        public void SuccessPaid(string data) {
            payArcadePopUp.SetActive(false);
            FindMatch();
        }

        public void successConnected(string data) {
            PlayerPrefs.SetString("wallet_connected", data);
        }

        public void SuccessAracadeBalance(string balance) {
            ArcadeBalance.text = balance.ToString();
        }

        // Main:
        public void FindMatch(){
            // Enable the "finding match" panel:
            findingMatchPanel.SetActive(true);
            // ...then finally, find a match:
            Connector.instance.FindMatch();
        }

        // Others:
        // *called by the toggle itself in the "On Value Changed" event:
        public void ToggleTargetFps(){
            Application.targetFrameRate = frameRateSetting.isOn? 60 : 30;

            // Display a notif message:
            if (frameRateSetting.isOn){
                DataCarrier.message = "Target frame rate has been set to 60.";
            }
        }

        public void showLeaderboard() {
            page = 1;
            StartCoroutine (getLeaderboard(1, ""));
        }

        private IEnumerator getLeaderboard(int pagenum, string direction)
        {
            string post_url = apiUrl + "getLeaderboard?page=" + pagenum;
            UnityWebRequest hs_post = UnityWebRequest.PostWwwForm(post_url, "");
            
            // hs_post.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return hs_post.SendWebRequest();
            if (hs_post.error != null) {
                Debug.Log("Save Error");
            } else {
                Debug.Log("Save Success");
                string dataText = hs_post.downloadHandler.text;
                Item[] items = JsonConvert.DeserializeObject<Item[]>(dataText);
                
                if (dataText != "[]") {
                    if (leaderboardContent.childCount > 0) {
                        for ( int i=leaderboardContent.childCount-1; i>=0; --i )
                        {
                            var child = leaderboardContent.GetChild(i).gameObject;
                            Destroy( child );
                        }
                    }
                    foreach (Item data in items)
                    {
                        LeaderboardItem item = Instantiate(leaderboardItem, leaderboardContent);
                        item.addressText.text = data.address.Substring(0, 6) + "..." + data.address.Substring(data.address.Length - 6);
                        item.nameText.text = data.name;
                        item.scoreText.text = data.score.ToString();
                    }
                    leaderboardObj.SetActive(true);
                } else {
                    if (direction != "") {
                        if (direction == "next") {
                            page--;
                        } else {
                            page++;
                        }
                    }
                }
            }
        }

        public void movePage(bool type) {
            if (type) {
                page++;
                StartCoroutine (getLeaderboard(page, "next"));
            } else {
                if (page > 1) {
                    page--;
                    StartCoroutine (getLeaderboard(page, "prev"));
                }
            }
        }
    }
}