//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright � 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------


#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY5
#endif

using Css;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// This scene:// protocol enables a link to point to another scene.
	/// E.g. href="scene://sceneName" will load the scene called 'sceneName' when clicked.
	/// </summary>
	
	public class SceneProtocol:FileProtocol{
		
		public override string[] GetNames(){
			return new string[]{"scene"};
		}
		
		public override void OnFollowLink(HtmlElement linkElement,Location path){

			#if PRE_UNITY5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(path.Directory+path.File);
			#else
			UnityEngine.SceneManagement.SceneManager.LoadScene(path.Directory+path.File);
			#endif

		}
		
	}
	
}