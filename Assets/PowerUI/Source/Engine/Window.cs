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
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents the javascript window object.
	/// </summary>
	
	public partial class Window : EventTarget{
		
		/// <summary>This windows name.</summary>
		private string name_="";
		/// <summary>This windows status.</summary>
		private string status_="";
		/// <summary>The very top window for this UI.</summary>
		public Window top;
		/// <summary>Represents 'this' object. Provided for javascript compatability.</summary>
		public Window self;
		/// <summary>The latest event to have occured.</summary>
		public UIEvent Event;
		/// <summary>The parent window of this one.</summary>
		public Window parent;
		/// <summary>The iframe element that this window is in.</summary>
		public HtmlElement iframe;
		/// <summary>The document for the given window.</summary>
		private HtmlDocument document_;
		/// <summary>The document for the given window.</summary>
		public HtmlDocument document{
			get{
				return document_;
			}
		}
		
		/// <summary>Sets the given document to this window.</summary>
		internal void SetDocument(HtmlDocument doc){
			document_=doc;
		}
		
		/// <summary>The thread that can be blocked in this window. It's the main JS thread.</summary>
		internal System.Threading.Thread BlockableThread;
		/// <summary>The currently open blocking dialogue in this window.</summary>
		internal BlockingDialogue BlockingDialogue;
		
		
		/// <summary>Creates a new window object.</summary>
		public Window(){
			self=this;
			history_=new History(this);
		}
		
		/// <summary>Called when the document is ready. Triggers the various loaded events.</summary>
		internal void Ready(){
			
			history_.Ready();
			
		}
		
		/// <summary>Posts a message to this document.</summary>
		public void postMessage(object message,string targetOrigin){
			
			string target=document.location.origin;
			
			if(targetOrigin=="*" || target==targetOrigin){
				
				// Note that this essentially originates from the target page.
				// It's incorrect, but we can't enforce a same-origin policy within Unity anyway.
				MessageEvent e=new MessageEvent(message,target);
				e.SetTrusted(false);
				dispatchEvent(e);
				
			}
			
		}
		
        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <param name="element">The element to compute the style for.</param>
        /// <returns>The style declaration describing the element.</returns>
        public Css.ComputedStyle getComputedStyle(HtmlElement element){
			if(element==null){
				return null;
			}
			return element.getComputedStyle(null);
        }
		
        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <param name="element">The element to compute the style for.</param>
        /// <param name="pseudo">The optional pseudo selector to use.</param>
        /// <returns>The style declaration describing the element.</returns>
        public Css.ComputedStyle getComputedStyle(HtmlElement element, string pseudo){
			if(element==null){
				return null;
			}
			return element.getComputedStyle(pseudo);
        }
		
		/// <summary>Escapes the given string, essentially making any HTML it contains literal.</summary>
		public string escapeHTML(string html){
			return Dom.Text.Escape(html);
		}
		
		/// <summary>Alerts the given message.</summary>
		public void alert(object value){
			
			// Open the dialogue:
			BlockingDialogues.Open("alert",this,value);
			
		}
		
		/// <summary>Asks the user to confirm something.</summary>
		public bool confirm(object request){
			
			// Open the dialogue:
			BlockingDialogue dialogue=BlockingDialogues.Open("confirm",this,request);
			
			if(dialogue==null){
				// It failed to open.
				// E.g. because 'prevent this page from opening other dialogues' was ticked.
				return false;
			}
			
			// Return the response received from the user:
			return dialogue.OkResponse;
			
		}
		
		/// <summary>Asks the user for some textual information.</summary>
		public string prompt(object request){
			
			// Open the dialogue:
			BlockingDialogue dialogue=BlockingDialogues.Open("prompt",this,request);
			
			if(dialogue==null){
				// It failed to open.
				// E.g. because 'prevent this page from opening other dialogues' was ticked.
				return "";
			}
			
			// Return the response received from the user:
			if(dialogue.Response is string){
				return (string)dialogue.Response;
			}
			
			// Empty string otherwise
			return "";
			
		}
		
		/// <summary>The status text.</summary>
		public string status{
			get{
				return status_;
			}
			set{
				status_=value;
			}
		}
		
		/// <summary>Is this window closed?</summary>
		public bool isClosed{
			get{
				return false;
			}
		}
		
		/// <summary>The location of the contained document.</summary>
		public Location location{
			get{
				return document.location;
			}
			set{
				document.location=value;
			}
		}
		
		/// <summary>This windows name.</summary>
		public string name{
			get{
				return name_;
			}
			set{
				name_=value;
			}
		}
		
		/// <summary>The window that opened this one.</summary>
		public Window opener{
			get{
				return null;
			}
		}
		
		/// <summary>The frame element that this window is in.</summary>
		public HtmlElement frameElement{
			get{
				return iframe;
			}
		}
		
		/// <summary>The device's DPI / 72.</summary>
		public float devicePixelRatio{
			get{
				return (float)ScreenInfo.Dpi / 72;
			}
		}
		
		/// <summary>Screen width.</summary>
		public int innerWidth{
			get{
				return (int)document.Viewport.Width;
			}
		}
		
		/// <summary>Screen height.</summary>
		public int innerHeight{
			get{
				return (int)document.Viewport.Height;
			}
		}
		
		/// <summary>The scroll amount on X.</summary>
		public int pageXOffset{
			get{
				HtmlElement root=document.html;
				
				if(root==null){
					return 0;
				}
				
				return root.scrollLeft;
			}
		}
		/// <summary>The scroll amount on Y.</summary>
		public int pageYOffset{
			get{
				HtmlElement root=document.html;
				
				if(root==null){
					return 0;
				}
				
				return root.scrollTop;
			}
		}
		
		/// <summary>The max scroll amount on X.</summary>
		public int scrollMaxX{
			get{
				HtmlElement root=document.html;
				
				if(root==null){
					return 0;
				}
				
				return root.contentWidth - root.scrollWidth;
			}
		}
		
		/// <summary>The max scroll amount on Y.</summary>
		public int scrollMaxY{
			get{
				HtmlElement root=document.html;
				
				if(root==null){
					return 0;
				}
				
				return root.contentHeight - root.scrollHeight;
			}
		}
		
		/// <summary>The scroll amount on X.</summary>
		public int scrollX{
			get{
				HtmlElement root=document.html;
				
				if(root==null){
					return 0;
				}
				
				return root.scrollLeft;
			}
		}
		
		/// <summary>The scroll amount on Y.</summary>
		public int scrollY{
			get{
				HtmlElement root=document.html;
				
				if(root==null){
					return 0;
				}
				
				return root.scrollTop;
			}
		}
		
		/// <summary>Screen x offset (closest we can get in Unity!)</summary>
		public int screenX{
			get{
				return 0;
			}
		}
		
		/// <summary>Screen y offset (closest we can get in Unity!)</summary>
		public int screenY{
			get{
				return 0;
			}
		}
		
		/// <summary>Screen width (closest we can get in Unity!)</summary>
		public int outerWidth{
			get{
				
				if(document.InWorldUI!=null){
					
					return document.InWorldUI.pixelWidth;
					
				}
				
				return ScreenInfo.ScreenX;
			}
		}
		
		/// <summary>Screen height (closest we can get in Unity!)</summary>
		public int outerHeight{
			get{
				
				if(document.InWorldUI!=null){
					
					return document.InWorldUI.pixelHeight;
					
				}
				
				return ScreenInfo.ScreenY;
			}
		}
		
	}
	
	#region WindowBase64
	
	public partial class Window{
		
		/// <summary>Converts a string to base64.</summary>
		public string atob(string a){
			return System.Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(a));
		}
		
		/// <summary>Converts base64 to a string.</summary>
		public string btoa(string b){
			return System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(b));
		}
		
	}
	
	#endregion
	
	#region WindowTimers
	
	public partial class Window{
		
		/// <summary>Clears intervals.</summary>
		public void clearInterval(UITimer timer){
			if(timer!=null){
				timer.Stop();
			}
		}
		
		/// <summary>Sets an interval.</summary>
		public UITimer setInterval(Nitro.DynamicMethod<Nitro.Void> method,int ms){
			return new UITimer(false,ms,method);
		}
		
		/// <summary>Sets a timeout.</summary>
		public UITimer setTimeout(Nitro.DynamicMethod<Nitro.Void> method,int ms){
			return new UITimer(true,ms,method);
		}
		
	}
	
	#endregion
	
	#region WindowEventHandlers
	
	public partial class Window{
		
		/// <summary>History pop event.</summary>
		public PopStateEventDelegate onpopstate{
			get{
				return GetFirstDelegate<PopStateEventDelegate>("popstate");
			}
			set{
				addEventListener("popstate",new PopStateEventListener(value));
			}
		}
		
		/// <summary>Called when the document is about to be unloaded.</summary>
		public BeforeUnloadEventDelegate onbeforeunload{
			get{
				return GetFirstDelegate<BeforeUnloadEventDelegate>("beforeunload");
			}
			set{
				addEventListener("beforeunload",new BeforeUnloadEventListener(value));
			}
		}
		
		/// <summary>Called when the hash changes.</summary>
		public HashChangeEventDelegate onhashchange{
			get{
				return GetFirstDelegate<HashChangeEventDelegate>("hashchange");
			}
			set{
				addEventListener("hashchange",new HashChangeEventListener(value));
			}
		}
		
	}
	
	#endregion
}