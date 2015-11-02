﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class parallaxPlan : MonoBehaviour {

	public List<GameObject> visibleGameObjectTab;

	public float distance;

	public GameObject popLimitation;
	public GameObject depopLimitation;
	public float hightSpaceBetweenAsset = 0;
	public float lowSpaceBetweenAsset = 0;

	public float space;

	public assetGenerator generator;

	private float initSpeed = 0.1f;
	private bool isInit = false;

	private float actualSpeed = 0.0f;

	private float spaceBetweenAsset = 0.0f;
	private float speedMultiplicator;

	private int speedSign = 1;

	// Use this for initialization
	void Start () {
		actualSpeed = 1;
		if (distance < 0) {
			speedMultiplicator = 1/ -distance;//1 - (1 / (1 -distance));
		} else {
			speedMultiplicator = 1 +  distance/10;//1 - (1 / (1 + distance));
		}
		generator.clear ();
		while (!isInit) {
			moveAsset (initSpeed);
//			Debug.Log();
			generateAssetIfNeeded ();
		}
	}
	
	// Update is called once per frame
	void Update () {
			moveAsset (actualSpeed * speedMultiplicator);
			generateAssetIfNeeded ();
	}

	void moveAsset(float speed){
		for (int i=0; i<visibleGameObjectTab.Count; i++) {
			GameObject parrallaxAsset = visibleGameObjectTab[i];
			Vector3 positionAsset = parrallaxAsset.transform.position;
			if (!isStillVisible(parrallaxAsset)){
				parrallaxAsset.SetActive(false);
				visibleGameObjectTab.Remove(parrallaxAsset);
				isInit =true;
			} else {
				positionAsset.x -= speed;
				parrallaxAsset.transform.position = positionAsset;
			}
		}
	}


	void generateAssetIfNeeded(){
		if(((spaceBetweenLastAndPopLimitation() < (-spaceBetweenAsset + actualSpeed * speedMultiplicator)) && (speedSign > 0)) ||
		   ((spaceBetweenLastAndPopLimitation() > (spaceBetweenAsset + actualSpeed * speedMultiplicator)) && (speedSign < 0))){
			GameObject asset = generator.generateGameObjectAtPosition();
			asset.transform.parent = this.transform;
			asset.transform.position = new Vector3(popLimitation.transform.position.x + (speedSign * asset.GetComponent<SpriteRenderer> ().sprite.bounds.max.x),popLimitation.transform.position.y,this.transform.position.z);
			visibleGameObjectTab.Add(asset);
			generateNewSpaceBetweenAssetValue();
		}
	}


	void generateNewSpaceBetweenAssetValue(){
		spaceBetweenAsset = Random.Range (lowSpaceBetweenAsset,hightSpaceBetweenAsset);
	}


	public void setSpeedOfPlan(float newSpeed){
			if (actualSpeed * newSpeed < 0) {
				swapPopAndDepop ();

				print ("Swap");
		}
		actualSpeed = newSpeed;
	}

	void swapPopAndDepop(){
		GameObject temp = popLimitation;
		popLimitation = depopLimitation;
		depopLimitation = temp;
		speedSign = speedSign * -1;
	}


	bool isStillVisible (GameObject parallaxObject) {
		if (speedSign < 0) {
			return (parallaxObject.transform.position.x - (parallaxObject.GetComponent<SpriteRenderer> ().sprite.bounds.max.x ) < depopLimitation.transform.position.x);// probl"me ici
		} else {
			return (parallaxObject.transform.position.x + (parallaxObject.GetComponent<SpriteRenderer> ().sprite.bounds.max.x ) > depopLimitation.transform.position.x);// probl"me ici
		}
	}


	float spaceBetweenLastAndPopLimitation() {
		if (visibleGameObjectTab.Count != 0) {
			if (speedSign > 0){
				space = (visibleGameObjectTab[visibleGameObjectTab.Count - 1].transform.position.x +(visibleGameObjectTab[visibleGameObjectTab.Count - 1].GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) - popLimitation.transform.position.x;
			}else {
				space = (visibleGameObjectTab[visibleGameObjectTab.Count - 1].transform.position.x -(visibleGameObjectTab[visibleGameObjectTab.Count - 1].GetComponent<SpriteRenderer> ().sprite.bounds.max.x)) - popLimitation.transform.position.x;
			}
			return space;

		} else {
			return - float.MaxValue;
		}
	}
}