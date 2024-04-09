using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Visyde{

    /// <summary>
    /// Scoreboard Item
    /// - The script for the item that is populated in the scoreboard.
    /// </summary>

    public class LeaderboardItem : MonoBehaviour {

		public Text addressText;
		public Text nameText;
		public Text scoreText;

		// Use this for initialization
		void Start () {
			// Stats:
		}
	}
}