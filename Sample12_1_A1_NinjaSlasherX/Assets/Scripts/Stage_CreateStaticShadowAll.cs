using UnityEngine;
using System.Collections;

public class Stage_CreateStaticShadowAll : MonoBehaviour {
	
	void Start () {
		// シーン内のSpriteRendererを検索
		SpriteRenderer[] spriteList = GameObject.FindObjectsOfType<SpriteRenderer> ();
		foreach (SpriteRenderer sprite in spriteList) {
			bool shadowOn = true;

			// SpriteRendererをチェック
			if (sprite.transform.parent) {
				if (sprite.transform.name 		 == "Shadow" 	||
					sprite.transform.parent.tag  == "Player" 	||
				    sprite.transform.parent.tag  == "Enemy" 	||
				    sprite.transform.parent.name == "VRPad") {
					shadowOn = false;
				}
			}
			if (sprite.name == "Filter_Paper" 				||
			    sprite.name == "Stage_Block_A3" 			||
			    sprite.name == "Stage_Block_B3" 			||
			    sprite.name == "Stage_Block_C3" 			||
			    sprite.name == "Stage_Car_Wheel" 			||
			    sprite.name.StartsWith("Chain") 			||
			    sprite.name == "Pin" 						||
			    sprite.name == "StageA_Road_A" 				||
			    sprite.name == "StageA_Road_B" 				||
			    sprite.name == "StageA_Road_R" 				||
			    sprite.name == "StageA_RoadUnder_A" 		||
			    sprite.name == "StageA_RoadUnder_B" 		||
			    sprite.name == "StageA_RoadUnder_LT" 		||
			    sprite.name == "StageA_RoadUnder_RT" 		||
			    sprite.name == "StageB_RoadUnder_L2" 		||
			    sprite.name == "StageB_BackWall_A" 			||
			    sprite.name == "StageB_BackWall_A3x3"	 	||
			    sprite.name == "StageB_BackWall_LT" 		||
			    sprite.name == "StageB_BackWall_RT" 		||
			    sprite.name == "StageB_Road_A" 				||
			    sprite.name == "StageB_RoadUnder_A" 		||
			    sprite.name == "StageB_RoadUnder_B" 		||
			    sprite.name == "StageB_Floor_R" 			||
			    sprite.name == "StageB_DoorA" 				||
			    sprite.name == "StageB_DoorB" 				||
			    sprite.name == "StageB_DoorA_Key" 			||
			    sprite.name == "StageB_DoorB_Key" 			||
			    sprite.name == "StageB_ExitA" 				||
			    sprite.name == "StageB_ExitB" 				||
			    sprite.name == "Stage_Item_Key_A" 			||
			    sprite.name == "Stage_Item_Key_B" 			||
			    sprite.name == "Stage_Item_Key_C" 			||
			    sprite.name == "Effect_Bas_Circle" 			||
			    sprite.name == "Effect_Bas_Semicircle" 		||
			    sprite.name == "Filter_Paper" 				||
			    sprite.name == "SlidePad" 					||
			    sprite.name == "Menu_Button") { 
				shadowOn = false;
			}
			if (shadowOn) {
				// 影のゲームオブジェクト作成
				GameObject 		goEmpty 		= new GameObject ("Shadow");
				SpriteRenderer 	spriteCopy 		= goEmpty.AddComponent<SpriteRenderer> ();

				spriteCopy.tag					= "Shadow";
				spriteCopy.sprite 				= sprite.sprite;
				spriteCopy.transform.parent 	= sprite.transform;
				spriteCopy.transform.position 	= sprite.transform.position;
				spriteCopy.transform.Translate(-0.4f,0.0f,0.5f,Space.Self);			
				spriteCopy.transform.localScale = Vector3.one;
				spriteCopy.transform.rotation 	= sprite.transform.rotation;
				spriteCopy.color 				= new Color(0.0f,0.0f,0.0f,0.5f);

				// ソーティングレイヤー＆オーダーを調整
				spriteCopy.sortingLayerName		= sprite.sortingLayerName;
				spriteCopy.sortingOrder 		= sprite.sortingOrder;
				//spriteCopy.sortingOrder 		= 0;
				if (sprite.sortingLayerName == "ObjectFront" ||
				    sprite.sortingLayerName == "Object") {
					spriteCopy.sortingLayerName = "ObjectBack";
				}
				if (sprite.sortingLayerName == "ObjectBack") {
					spriteCopy.sortingOrder += -10;
				}
				if (sprite.name == "StageB_Floor_L" ||
				    sprite.name == "StageB_FloorUnder_B") {
					spriteCopy.sortingOrder += -10;
				}
			}
		}
	}
}
