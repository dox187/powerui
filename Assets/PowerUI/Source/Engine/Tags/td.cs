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

using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles a table cell.
	/// </summary>
	
	[Dom.TagName("td")]
	public class HtmlTdElement:HtmlElement{
		
		/// <summary>The column number of this cell in the table.</summary>
		private int Column;
		/// <summary>The table this cell belongs to.</summary>
		private HtmlTableElement Table;
		
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Handle it (same as th):
			return HtmlThElement.HandleTableCellZone(this,lexer,mode);
			
		}
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			return HandleClose("td",lexer,mode);
		}
		
		public static bool HandleClose(string close,HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				lexer.CloseSelect(false,null,close);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				if(lexer.IsInTableScope(close)){
					
					// Generate implied:
					lexer.GenerateImpliedEndTags();
					
					// Close including:
					lexer.CloseInclusive(close);
					
					// Clear to marker:
					lexer.ClearFormatting();
					
					lexer.CurrentMode=HtmlTreeMode.InRow;
					
				}
				
			}else{
			
				return false;
			
			}
			
			return true;
		}
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override bool ImplicitEndAllowed{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
		public override void OnChildrenLoaded(){
			
			// Go up the tree looking for our table.
			Element parent=parentElement;
			
			while(parent!=null){
				
				if(parent.Tag=="table"){
					// Got it!
					Table=parent as HtmlTableElement;
					break;
				}
				
				parent=parent.parentElement;
			}
			
			if(Table!=null){
				
				// What child number (column) is this element?
				Column=childIndex;
			
			}
			
			// Base:
			base.OnChildrenLoaded();
			
		}
		
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="valign"){
				Style.vAlign=this["valign"];
				return true;
			}
			
			return false;
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return last ? Dom.HtmlTreeMode.InBody : Dom.HtmlTreeMode.InCell;
			
		}
		
		/// <summary>Makes sure the width of this element matches that of the biggest one in the column.</summary>
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			if(Table==null || Table.ColumnWidths==null || !widthUndefined){
				return;
			}
			
			widthUndefined=false;
			
			ComputedStyle computed=Style.Computed;
			ComputedStyle column=Table.ColumnWidths[Column];
			LayoutBox columnBox=column.FirstBox;
			
			// How much style does this cell have?
			float styleSize=box.Width-box.InnerWidth;
			
			if(column!=null){
				
				if(column!=computed){
					box.InnerWidth=(columnBox.InnerWidth-styleSize);
				}else{
					// What if this cell isn't the widest anymore?
				}
				
			}else{
				// No particular element is the max width - use the NoWidthPixels size instead.
				box.InnerWidth=(Table.NoWidthPixels-styleSize);
			}
		}
		
	}
	
}