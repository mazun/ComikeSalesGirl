using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ChinoMessage : MonoBehaviour {
	public int messageChangeInterval = 5;
	[SerializeField] Text message = null;

	private string[] randomMessages = {
		"いらっしゃいませ",
		"ありがとうございます",
		"よろしければ見ていってください"
	};

	private DateTime nextMessageChange;

	// Use this for initialization
	void Start () {
		ChangeMessage (randomMessages [0]);
	}
	
	// Update is called once per frame
	void Update () {
		if (nextMessageChange.CompareTo (DateTime.Now) < 0) {
			ChangeMessage (randomMessages [UnityEngine.Random.Range(0, randomMessages.Length)]);
		}
	}

	public void MessageForBook() {
		ChangeMessage("シャロさんの同人誌です。\n500円になります。");
	}

	public void MessageForBadge() {
		ChangeMessage("私達の缶バッジです。\n200円になります。");
	}

	private void ChangeMessage(String text) {
		message.text = text;
		nextMessageChange = DateTime.Now.AddSeconds (messageChangeInterval);
	}
}
