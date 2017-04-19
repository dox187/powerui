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

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using PowerUI;
using Dom;
using Nitro;
using Css;


namespace PowerUI{

	/// <summary>
	/// Looks for and compiles Nitro script ahead of time for iOS.
	/// The Nitro can be located in any .html file.
	/// </summary>

	public class NitroAOT:AssetPostprocessor{

		public static void OnPostprocessAllAssets(string[] importedAssets,string[] deletedAssets,string[] movedAssets,string[] movedFromAssetPaths){
			if(!NitroSettings.CompileAOT){
				return;
			}
			
			// A list is created if any changed assets are .html files - they are then added to the list:
			List<string> HtmlAssets=null;
			
			FindHtmlAssets(importedAssets,ref HtmlAssets);
			FindHtmlAssets(movedAssets,ref HtmlAssets);
			FindHtmlAssets(movedFromAssetPaths,ref HtmlAssets);
			
			if(HtmlAssets!=null){
				// Some html assets changed somewhere.
				Compile(HtmlAssets[0]);
			}
		}
		
		/// <summary>AOT compiles the given HTML file. It gets searched for any Nitro, and a DLL may be generated from it.</summary>
		/// <param name="htmlFile">The path to a html file.</param>
		public static void Compile(string htmlFile){
			// Grab the HTML:
			string htmlText=File.ReadAllText(htmlFile);
			
			// Make sure the parser and compiler is ready to go:
			UI.Start(true);
			
			// Hook up the AOT file:
			NitroCode.OnAotFileExists=OnAotFileExists;
			
			// Drop the HTML into a document:
			HtmlDocument uiDocument=new HtmlDocument(new Renderman(true),null,true);
			uiDocument.SetRawLocation( new Location("resources://",null) );
			// Set the location so it knows where it came from for error reporting:
			uiDocument.ScriptLocation=htmlFile;
			// Write the HTML:
			uiDocument.innerHTML=htmlText;
		}
		
		public static void FindHtmlAssets(string[] fileSet,ref List<string> allFoundFiles){
			if(fileSet==null || fileSet.Length==0){
				return;
			}
			
			for(int i=0;i<fileSet.Length;i++){
				string filePath=fileSet[i].ToLower();
				if(filePath.EndsWith(".html") || filePath.EndsWith(".htm")){
					
					// Got one! Add it to the set which may not have been initialised yet:
					if(allFoundFiles==null){
						allFoundFiles=new List<string>();
					}
					
					// Add it in (bearing in mind that filePath is lowercase):
					allFoundFiles.Add(fileSet[i]);
				}
			}
		}
		
		/// <summary>Deletes all precompiled files.</summary>
		public static void DeleteAll(){
			DeleteAll("Assets");
			
			// Save the changes:
			AssetDatabase.SaveAssets();
			// Refresh:
			AssetDatabase.Refresh();
		}
		
		/// <summary>Deletes all Nitro precompiled files in the given directory.</summary>
		private static void DeleteAll(string inDirectory){
		
			// Any -nitro-aot.dll files?
			string[] files=Directory.GetFiles(inDirectory,"*-nitro-aot.dll");
			
			// For each -nitro-aot.dll file..
			for(int i=0;i<files.Length;i++){
				string filePath=files[i];
				// Safety check, if for whatever reason the underlying API returns incorrect files:
				if(!filePath.EndsWith("-nitro-aot.dll")){
					continue;
				}
				// We have a nitro precompile file. Delete it now:
				AssetDatabase.DeleteAsset(filePath);
			}
			
			// Any subdirectories?
			string slash=Path.DirectorySeparatorChar+"";
			string[] subDirectories=Directory.GetDirectories(inDirectory);
			
			for(int i=0;i<subDirectories.Length;i++){
				// Skip if it's a folder called 'Languages' or 'NoAOT'.
				string fullPath=subDirectories[i];
				
				if(fullPath.EndsWith(slash+"Languages") || fullPath.EndsWith(slash+"NoAOT") || fullPath.EndsWith(slash+".svn")){
					continue;
				}
				
				DeleteAll(fullPath);
			}
			
		}
		
		/// <summary>AOT compiles all .html files found in Assets. The result goes into a DLL named after the file.
		/// The class name inside the DLL is based on the script string, which is available to the runtime.</summary>
		public static void CompileAll(){
			CompileAll("Assets");
		}
		
		/// <summary>AOT compiles all .html files found in Assets. The result goes into a DLL named after the file.
		/// The class name inside the DLL is based on the script string, which is available to the runtime.</summary>
		/// <param name="inDirectory">The directory to search in.</param>
		public static void CompileAll(string inDirectory){
			// Any html files?
			string[] files=Directory.GetFiles(inDirectory);
			
			for(int i=0;i<files.Length;i++){
				string filePath=files[i].ToLower();
				if(filePath.EndsWith(".html") || filePath.EndsWith(".htm")){
					Compile(files[i]);
				}
			}
			
			// Any subdirectories?
				string slash=Path.DirectorySeparatorChar+"";
			string[] subDirectories=Directory.GetDirectories(inDirectory);
			
			for(int i=0;i<subDirectories.Length;i++){
				// Skip if it's a folder called 'Languages' or 'NoAOT'.
				string fullPath=subDirectories[i];
				
				if(fullPath.EndsWith(slash+"Languages") || fullPath.EndsWith(slash+"NoAOT") || fullPath.EndsWith(slash+".svn")){
					continue;
				}
				
				CompileAll(fullPath);
			}
			
			// Make sure the asset database is up to date:
			AssetDatabase.Refresh();
		}
		
		/// <summary>Called by wrench when an AOT DLL already exists.
		/// Unity requires AssetDatabase to be used to handle this.</summary>
		private static void OnAotFileExists(string path){
			AssetDatabase.DeleteAsset(path);
			AssetDatabase.SaveAssets();
		}
		
	}
	
}