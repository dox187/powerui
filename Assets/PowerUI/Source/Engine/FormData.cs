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

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;


namespace PowerUI{
	
	/// <summary>
	/// Represents information entered into a html form.
	/// It maps field names to their current selected value.
	/// </summary>
	
	public class FormData : UIEvent{
		
		/// <summary>The source form element.</summary>
		public HtmlElement form;
		/// <summary>The internal dictionary which holds the field/value pairs.</summary>
		private Dictionary<string,string> RawFields;
		
		
		/// <summary>Creates a new form with the given field/value pairs.</summary>
		/// <param name="fields">A dictionary holding the field/value pairs from the form.</param>
		public FormData(Dictionary<string,string> fields){
			RawFields=fields;
		}
		
		/// <summary>Gets the value of the named input element.</summary>
		/// <param name="name">The field name.</param>
		/// <returns>The field value.</returns>
		public string this[string name]{
			get{
				if(RawFields==null){
					return null;
				}
				string result;
				RawFields.TryGetValue(name,out result);
				return result;
			}
		}
		
		/// <summary>Provides a way of easily checking if a named checkbox is checked.</summary>
		/// <param name="name">The field name of the checkbox.</param>
		/// <returns>True if the box is checked.</returns>
		public bool Checked(string name){
			return !string.IsNullOrEmpty(this[name]);
		}
		
		/// <summary>Converts this form data to a unity form.</summary>
		/// <returns>A Unity WWWForm suitable for web posting.</returns>
		public WWWForm ToUnityForm(){
			WWWForm result=new WWWForm();
			if(RawFields!=null){
				foreach(KeyValuePair<string,string> kvp in RawFields){
					result.AddField(kvp.Key,kvp.Value);
				}
			}
			return result;
		}
		
		/// <summary>Converts this form data into a string suitable for use in post or gets.</summary>
		/// <returns>A url friendly string, e.g. field1=value1&field2=value2...</returns>
		public string ToUrlString(){
			if(RawFields==null){
				return "";
			}
			
			string postString="";
			
			foreach(KeyValuePair<string,string> kvp in RawFields){
				if(postString!=""){
					postString+="&";
				}
				postString+=Web.UrlEncode(kvp.Key)+"="+kvp.Value;
			}
			
			return postString;
		}
		
	}
	
}