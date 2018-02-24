﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;

	private Animator animator;
	private int food;


	protected override void Start() {
		animator = GetComponent<Animator>();
		food = GameManager.instance.playerFoodPoints;

		base.Start();
	}

	void OnDisable() {
		GameManager.instance.playerFoodPoints = food;
	}

	void Update () {
		if (!GameManager.instance.playersTurn) return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

		// prevent the player from moving diagonally
		if (horizontal != 0) {
			vertical = 0;
		}

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall>(horizontal, vertical);
		}
	}

	protected override void AttemptMove <T> (int xDir, int yDir) {
		food--;

		// call the function of the base class
		base.AttemptMove <T> (xDir, yDir);
		RaycastHit2D hit;
		CheckIfGameOver();
		GameManager.instance.playersTurn = false;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Exit") {
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerFood;
			other.gameObject.SetActive(false);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			other.gameObject.SetActive(false);
		}
	}

	protected override void OnCantMove <T> (T component) {
		Wall hitWall = component as Wall;
		hitWall.DamageWall(wallDamage);
		animator.SetTrigger("playerChop");
	}

	void Restart() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public void LoseFood(int loss) {
		animator.SetTrigger("playerHit");
		food -= loss;
		CheckIfGameOver();
	}

	void CheckIfGameOver() {
		if (food <= 0) {
			GameManager.instance.GameOver();
		}
	}
}