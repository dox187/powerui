//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using PowerUI;
using ContextMenus;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Windows{
	
	/// <summary>
	/// A specialised window type for displaying OptionLists.
	/// Context menus derive from this.
	/// </summary>
	
	public class ContextMenuWindow : Window{
		
		/// <summary>The source option list.</summary>
		public OptionList List;
		
		/// <summary>The depth that this type of window lives at.</summary>
		public override int Depth{
			get{
				return 10000;
			}
		}
		
		/// <summary>Adds an option to the builder.</summary>
		public virtual void BuildOption(StringBuilder builder,Option option){}
		
		/// <summary>Builds up the options now.</summary>
		public virtual void BuildOptions(StringBuilder builder){
			
			if(List==null){
				return;
			}
			
			// The root node (must only be one node at the root):
			builder.Append("<div>");
			
			// Generate the menu now!
			foreach(Option option in List.options){
				
				// Build it:
				BuildOption(builder,option);
				
			}
			
			builder.Append("</div>");
			
		}
		
		/// <summary>The submenu window type. It's just "the same as this" by default.
		/// You can detect if an OptionList is a submenu from the parent property.</summary>
		public virtual string SubMenuType{
			get{
				// Same by default.
				return Type;
			}
		}
		
		public override void Load(string url,Dictionary<string,object> globals){
			
			// Get the list of options to show:
			List=Get<OptionList>("options",globals);
			
			// Build now:
			StringBuilder builder=new System.Text.StringBuilder();
			BuildOptions(builder);
			
			// Get the HTML:
			string html=builder.ToString();
			
			// Write it out into the template:
			SetHtml(html);
			
		}
		
	}
	
}