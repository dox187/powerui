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

namespace PowerUI{
	
	/// <summary>
	/// Handles the scroll left button on scrollbars.
	/// </summary>
	
	[Dom.TagName("scrollleft")]
	public class HtmlScrollLeftElement:HtmlElement{
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Get the scroll bar:
			HtmlInputElement scroll=parentElement as HtmlInputElement;
			// And scroll it:
			scroll.ScrollBy(-1);
			
		}
		
	}
	
}