//--------------------------------------//               PowerUI////        For documentation or //    if you have any issues, visit//        powerUI.kulestar.com////    Copyright � 2013 Kulestar Ltd//          www.kulestar.com//--------------------------------------using System;using Dom;using Nitro;namespace PowerUI{		/// <summary>	/// Handles various standard attribute DOM functions.	/// </summary>	public partial class HtmlElement{				/// <summary>Get the value of an attribute by name. Generally element[name] is better.</summary>		public string getAttributeNS(string ns,string name){			return this[ns+":"+name];		}				/// <summary>Does this element have the named attribute? element[name] is generally better.</summary>		public bool hasAttributeNS(string ns,string name){			return (this[ns+":"+name]!=null);		}				/// <summary>Set the named attribute. element[name] is generally better.</summary>		public void setAttributeNS(string ns,string name,string value){			this[ns+":"+name]=value;		}				/// <summary>Remove the named attribute. element[name] is generally better.</summary>		public void removeAttributeNS(string ns,string name){			this[ns+":"+name]=null;		}				/// <summary>Get the value of an attribute by name. Generally element[name] is better.</summary>		public string getAttribute(string name){			return this[name];		}				/// <summary>Does this element have the named attribute? element[name] is generally better.</summary>		public bool hasAttribute(string name){			return (this[name]!=null);		}				/// <summary>Set the named attribute. element[name] is generally better.</summary>		public void setAttribute(string name,string value){			this[name]=value;		}				/// <summary>Remove the named attribute. element[name] is generally better.</summary>		public void removeAttribute(string name){			this[name]=null;		}			}	}