using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreTest : MonoBehaviour {
	string log = "";
	Vector2 logPosition;
	
	void Log(string _msg) {
		log += _msg + "\n";
	}
	
	void OnGUI(){
		if(GUILayout.Button("Init Shop", GUILayout.Width(200))){
			StoreManager.Initialize(
				() => {
					Log("Shop Initialized");
				},
				(_msg) => {
					Log ("Error:" + _msg);
				}
			);
		}
		
		if(GUILayout.Button("Restore purchases", GUILayout.Width(200))){
			StoreManager.RestorePurchases(
				() => {
					Log ("Purchases restored");
				},
				(_id) => {
					Log("Restored " + _id);
				},
				(_error) => {
					Log ("Error:" + _error);
				}
			);
		}
		
		foreach(string product in StoreManager.GetProducts()) {
			GUILayout.BeginHorizontal();
				if(GUILayout.Button("Purchase")) {
					StoreManager.PurchaseProduct(
						product,
						(_id) => {
							Log ("Purchased:" + _id);
						},
						(_error) => {
							Log ("Error:" + _error);
							Debug.Log("Error:" + _error);
						}
					);
				}
				GUILayout.Label(product + (StoreManager.IsProductPurchased(product) ? ":purchased" : ""));
			GUILayout.EndHorizontal();	
		}
		
		if(GUILayout.Button("Retrieve prices", GUILayout.Width(200))){
			foreach(string product in StoreManager.GetProducts()) {
				StoreManager.GetProductInfo(
					product,
					(_id, _info) => {
						Log ("Price for " + _id + ": " +_info.price);
					},
					(_id, _msg) => {
						Log (_msg);
					}
				);
			}
		}
		
		
		logPosition = GUILayout.BeginScrollView(logPosition, GUILayout.Width(500), GUILayout.Height(500));
			GUILayout.TextArea(log);
		GUILayout.EndScrollView();
	}
}
