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
	/// Handles the scroll right button on scrollbars.
	/// </summary>
	
	[Dom.TagName("scrollright")]
	public class HtmlScrollRightElement:HtmlElement{
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Get the scroll bar:
			HtmlInputElement scroll=parentElement as HtmlInputElement;
			// And scroll it:
			scroll.ScrollBy(1);
			
		}
		
	}
	
}