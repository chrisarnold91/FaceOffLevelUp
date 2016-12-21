using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AI : MonoBehaviour {

	public float timer = 10.0f;
	public Text score;
	public Text EnemyScore;
	public Text prompt;
	public bool takeScore = true;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		GameObject EnemyTomato = GameObject.Find("Enemy Tomato");
		timer -= Time.deltaTime;

		if(timer <= 0){
			//throw tomato
			EnemyTomato.GetComponent<Rigidbody>().AddForce(transform.forward * 5000);

			//update score
			if(takeScore){
				float floatedTimer = float.Parse(score.text)-5;
				score.text = floatedTimer.ToString();
				takeScore = false;
			} else {
				takeScore = true;
				timer = 5.0f;
				EnemyTomato.transform.position = new Vector3(30, -40, 408);
			}

			float floatedScore = float.Parse(score.text);
			float enemy = float.Parse(EnemyScore.text);

			if(enemy > 0 && floatedScore <= 0){
				prompt.text = "You lose!";
			}
			if(enemy <= 0 && floatedScore > 0){
				prompt.text = "You win!";
			}

		}

	}
}
