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
using System.Collections;
using System.Collections.Generic;
using Css;
using InfiniText;


namespace PowerUI{
	
	/// <summary>
	/// Stores the information used whilst laying out boxes during a reflow.
	/// <summary>

	public class LineBoxMeta{
		
		/// <summary>The "host" block box.</summary>
		public BlockBoxMeta HostBlock;
		/// <summary>The height of the current line being processed.</summary>
		public float LineHeight;
		/// <summary>The current font size.</summary>
		public float FontSize_=float.MinValue;
		/// <summary>A linked list of elements on a line are kept. This is the last element on the current line.</summary>
		internal LayoutBox LastOnLine;
		/// <summary>A linked list of elements on a line are kept. This is the first element on the current line.</summary>
		internal LayoutBox FirstOnLine;
		/// <summary>A linked list of elements on a line are kept. This is the last element on the current out of flow line.</summary>
		internal LayoutBox LastOutOfFlow;
		/// <summary>A linked list of elements on a line are kept. This is the first element on the current out of flow line.</summary>
		internal LayoutBox FirstOutOfFlow;
		/// <summary>The last line start. Tracked for alignment.</summary>
		internal LayoutBox LastLineStart;
		/// <summary>The first line start. Tracked for alignment.</summary>
		internal LayoutBox FirstLineStart;
		/// <summary>The value of the CSS line-height property.</summary>
		public float CssLineHeight_=float.MinValue;
		/// <summary>The set of active floated elements for the current line being rendered.</summary>
		internal List<LayoutBox> ActiveFloats;
		/// <summary>The current x location of the renderer in screen pixels from the left.</summary>
		internal float PenX;
		/// <summary>The point at which lines begin at.</summary>
		public float LineStart;
		/// <summary>The position of the text baseline.</summary>
		public float Baseline;
		/// <summary>The current box being worked on.</summary>
		internal LayoutBox CurrentBox;
		/// <summary>The next box in the hierarchy.</summary>
		public LineBoxMeta Parent;
		/// <summary>The inline element.</summary>
		public RenderableData RenderData;
		
		
		public LineBoxMeta(LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData){
			
			Parent=parent;
			CurrentBox=firstBox;
			RenderData=renderData;
			
		}
		
		/// <summary>The value of the CSS line-height property.</summary>
		public float CssLineHeight{
			get{
				
				if(CssLineHeight_==float.MinValue){
					// Parent:
					return HostBlock.CssLineHeight_;
				}
				
				return CssLineHeight_;
			}
			set{
				CssLineHeight_=value;
			}
		}
		
		/// <summary>The value of the CSS font-size property.</summary>
		public float FontSize{
			get{
				
				if(FontSize_==float.MinValue){
					// Parent:
					return HostBlock.FontSize_;
				}
				
				return FontSize_;
			}
			set{
				FontSize_=value;
			}
		}
		
		/// <summary>The length of the longest line so far.</summary>
		public virtual float LargestLineWidth{
			get{
				return HostBlock.LargestLineWidth_;
			}
			set{
				HostBlock.LargestLineWidth_=value;
			}
		}
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public virtual float PenY{
			get{
				return HostBlock.PenY_;
			}
			set{
				HostBlock.PenY_=value;
			}
		}
		
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public virtual bool GoingLeftwards{
			get{
				return HostBlock.GoingLeftwards_;
			}
			set{
				HostBlock.GoingLeftwards_=value;
			}
		}
		
		/// <summary>Is this box a flow root?</summary>
		public virtual bool IsFlowRoot{
			get{
				return false;
			}
		}
		
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public virtual float MaxX{
			get{
				return HostBlock.MaxX_;
			}
			set{
				HostBlock.MaxX_=value;
			}
		}
		
		/// <summary>The current font face in use.</summary>
		internal FontFace FontFace_;
		
		/// <summary>The current font family in use.</summary>
		internal virtual FontFace FontFace{
			get{
				
				if(FontFace_==null){
					// Use the block:
					return HostBlock.FontFace;
				}
				
				return FontFace_;
				
			}
			set{
				FontFace_=value;
			}
		}
		
		
		/// <summary>Adds the given style to the current line.</summary>
		/// <param name="style">The style to add.</param>
		internal void AddToLine(LayoutBox styleBox){
			
			// Make sure it's safe:
			styleBox.NextLineStart=null;
			styleBox.NextOnLine=null;
			
			if((styleBox.PositionMode & PositionMode.InFlow)==0){
				
				// Out of flow - add it to a special line:
				if(FirstOutOfFlow==null){
					FirstOutOfFlow=LastOutOfFlow=styleBox;
				}else{
					LastOutOfFlow=LastOutOfFlow.NextOnLine=styleBox;
				}
				
				styleBox.ParentOffsetLeft=PenX+styleBox.Margin.Left;
				styleBox.ParentOffsetTop=PenY+styleBox.Margin.Top;
				
				return;
			}
			
			if(styleBox.FloatMode==FloatMode.None){
				
				// In flow - add to line
				
				if(FirstOnLine==null){
					FirstOnLine=LastOnLine=styleBox;
					
					if(FirstLineStart==null){
						
						// First child element. Update parent if we've got one:
						if(Parent!=null && Parent.CurrentBox!=null){
							
							Parent.CurrentBox.FirstChild=styleBox;
							
						}
						
						FirstLineStart=LastLineStart=styleBox;
					}else{
						LastLineStart=LastLineStart.NextLineStart=styleBox;
					}
					
				}else{
					LastOnLine=LastOnLine.NextOnLine=styleBox;
				}
				
			}else{
				
				// Using float - add to active floaters:
				
				if(ActiveFloats==null){
					ActiveFloats=new List<LayoutBox>(1);
				}
				
				ActiveFloats.Add(styleBox);
				
			}
			
		}
		
		/// <summary>Completes a line, optionally breaking it.</summary>
		public void CompleteLine(bool breakLine,bool topOfStack){
			
			float lineHeight=LineHeight;
			
			if(breakLine || topOfStack){
				
				// Vertically align all elements on the current line and reset it:
				LayoutBox currentBox=FirstOnLine;
				FirstOnLine=null;
				LastOnLine=null;
				
				while(currentBox!=null){
					// Calculate the offset to where the top left corner is (of the complete box, margin included):
					
					// Must be positioned such that the boxes sit on this lines baseline.
					// the baseline is by default at half the line-height but moves up whenever 
					// an inline-block element with padding/border/margin is added.
					
					float delta=(currentBox.Height+currentBox.Margin.Bottom);
					
					if((currentBox.DisplayMode & DisplayMode.OutsideInline)!=0){
						
						// Must also move it down by padding and border:
						delta-=currentBox.Border.Bottom + currentBox.Padding.Bottom;
						
					}
					
					delta=lineHeight-delta;
					
					currentBox.ParentOffsetTop=PenY+delta;
					
					// Hop to the next one:
					currentBox=currentBox.NextOnLine;
				}
				
				currentBox=FirstOutOfFlow;
				FirstOutOfFlow=null;
				LastOutOfFlow=null;
				
				while(currentBox!=null){
					// Calculate the offset to where the top left corner is (of the complete box, margin included):
					
					// Just margin for these ones:
					float delta=(currentBox.Margin.Bottom);
					
					if((currentBox.DisplayMode & DisplayMode.OutsideInline)!=0){
						
						// Must also move it down by padding and border:
						delta-=currentBox.Border.Bottom + currentBox.Padding.Bottom;
						
					}else if((currentBox.DisplayMode & DisplayMode.OutsideBlock)!=0){
						
						// Clear x:
						currentBox.ParentOffsetLeft=LineStart;
						
					}
					
					delta=lineHeight-delta;
					
					currentBox.ParentOffsetTop=PenY+lineHeight;
					
					// Hop to the next one:
					currentBox=currentBox.NextOnLine;
				}
				
			}
			
			// Recurse down to the nearest flow root element.
			
			if(IsFlowRoot){
				
				// Done recursing downwards - we're at the block!
				
				if(breakLine || topOfStack){
					
					// Move the pen down to the following line:
					PenY+=lineHeight;
					
					if(ActiveFloats!=null){
						
						// Are any now cleared?
						
						for(int i=ActiveFloats.Count-1;i>=0;i--){
							
							// Grab the style:
							LayoutBox activeFloat=ActiveFloats[i];
							
							// Is the current render point now higher than this floating object?
							// If so, we must reduce LineStart/ increase MaxX depending on which type of float it is.
							
							if(PenY>=(activeFloat.ParentOffsetTop + activeFloat.Height)){
								
								// Yep! Cleared. Reduce our size:
								if(activeFloat.FloatMode==FloatMode.Right){
									
									if(GoingLeftwards){
										
										// Decrease LineStart:
										LineStart-=activeFloat.Width;
										
									}else{
										
										// Increase max x:
										MaxX+=activeFloat.Width;
										
									}
									
								}else{
									
									if(GoingLeftwards){
										
										// Increase max x:
										MaxX+=activeFloat.Width;
										
									}else{
										
										// Decrease LineStart:
										LineStart-=activeFloat.Width;
										
									}
									
								}
								
								// Remove it as an active float:
								ActiveFloats.RemoveAt(i);
								
							}
							
						}
						
					}
					
				}
				
			}else{
				
				// Apply valid width/height:
				LayoutBox box=CurrentBox;
				
				bool inFlow=((box.PositionMode & PositionMode.InFlow)!=0);
				
				// Update line height:
				if(inFlow && lineHeight>Parent.LineHeight){
					Parent.LineHeight=lineHeight;
				}
				
				box.InnerHeight=lineHeight;
				box.InnerWidth=PenX-LineStart;
				box.SetDimensions(false,false);
				
				// Update content w/h:
				box.ContentHeight=box.InnerHeight;
				box.ContentWidth=box.InnerWidth;
				
				if(inFlow){
					// Update dim's:
					Parent.AdvancePen(box);
				}
				
				if(inFlow && breakLine){
					
					if((CurrentBox.DisplayMode & DisplayMode.FlowRoot)==0){
						
						// Linebreak the parent:
						Parent.CompleteLine(breakLine,false);
						
						// Create a new box!
						// (And add it to the parent)
						LayoutBox styleBox=new LayoutBox();
						styleBox.Border=box.Border;
						styleBox.Padding=box.Padding;
						styleBox.Margin=box.Margin;
						
						// No left margin:
						styleBox.Margin.Left=0f;
						
						styleBox.DisplayMode=box.DisplayMode;
						styleBox.PositionMode=box.PositionMode;
						
						CurrentBox=styleBox;
						
						styleBox.NextInElement=null;
						
						// Add to the inline element's render data:
						RenderData.LastBox.NextInElement=styleBox;
						RenderData.LastBox=styleBox;
						
						// Add to line next:
						Parent.AddToLine(styleBox);
						
					}else{
						
						// It's a flow root inside. Inline-block here.
						
						// Check if it's the only one on the parents line:
						if(Parent.FirstOnLine!=CurrentBox){
							
							//
							
						}
						
					}
					
				}
				
			}
			
			if(breakLine){
				
				// Finally, reset the pen (this is after the recursion call, so we've cleared floats etc):
				if(this is InlineBoxMeta){
					LineStart=HostBlock.LineStart;
				}
				
				PenX=LineStart;
				
				LineHeight=0;
				Baseline=0;
				
			}
			
		}
		
		/// <summary>Advances the pen now.</summary>
		public void AdvancePen(LayoutBox styleBox){
			
			if(styleBox.FloatMode==FloatMode.Right){
				
				// Float right
				if(GoingLeftwards){
					styleBox.ParentOffsetLeft=LineStart;
					PenX+=styleBox.TotalWidth;
				}else{
					styleBox.ParentOffsetLeft=MaxX-styleBox.TotalWidth;
				}
				
				if(GoingLeftwards){
					
					// Push over where lines start at:
					LineStart+=styleBox.Width;
					
				}else{
					
					// Reduce max:
					MaxX-=styleBox.Width;
					
				}
				
			}else if(styleBox.FloatMode==FloatMode.Left){
				
				// Float left
				if(GoingLeftwards){
					styleBox.ParentOffsetLeft=MaxX-styleBox.TotalWidth;
				}else{
					styleBox.ParentOffsetLeft=LineStart;
					PenX+=styleBox.TotalWidth;
				}
				
				if(GoingLeftwards){
				
					// Reduce max:
					MaxX-=styleBox.Width;
					
				}else{
					
					// Push over where lines start at:
					LineStart+=styleBox.Width;
					
				}
				
				// If it's not the first on the line then..
				if(styleBox!=FirstOnLine){
					
					// Push over all the elements before this on the line.
					LayoutBox currentLine=FirstOnLine;
					
					while(currentLine!=styleBox && currentLine!=null){
						
						if(currentLine.FloatMode==FloatMode.None){
							// Move it:
							currentLine.ParentOffsetLeft+=styleBox.Width;
						}
						
						// Next one:
						currentLine=currentLine.NextOnLine;
						
					}
					
				}
				
			}else if(GoingLeftwards){
				PenX+=styleBox.Width+styleBox.Margin.Right;
				styleBox.ParentOffsetLeft=LineStart*2-PenX;
				PenX+=styleBox.Margin.Left;
				
				float effectiveHeight=styleBox.TotalHeight;
				
				if(effectiveHeight>LineHeight){
					LineHeight=effectiveHeight;
				}
				
			}else{
				PenX+=styleBox.Margin.Left;
				styleBox.ParentOffsetLeft=PenX;
				PenX+=styleBox.Width+styleBox.Margin.Right;
				
				// If it's inline then don't use total height.
				// If it's a word then we don't check it at all.
				float effectiveHeight;
				
				if((styleBox.DisplayMode & DisplayMode.OutsideInline)!=0){
					effectiveHeight=styleBox.InnerHeight;
				}else{
					effectiveHeight=styleBox.TotalHeight;
				}
				
				if(effectiveHeight>LineHeight){
					LineHeight=effectiveHeight;
				}
				
			}
			
		}
		
	}
	
	public class BlockBoxMeta : LineBoxMeta{
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		internal float PenY_;
		/// <summary>The point at which lines begin at.</summary>
		internal float LineStart_;
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		internal bool GoingLeftwards_;
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		internal float MaxX_;
		/// <summary>The length of the longest line so far.</summary>
		public float LargestLineWidth_;
		
		
		public BlockBoxMeta(LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData):base(parent,firstBox,renderData){
			
		}
		
		/// <summary>Is this box a flow root?</summary>
		public override bool IsFlowRoot{
			get{
				return true;
			}
		}
		
		/// <summary>The length of the longest line so far.</summary>
		public override float LargestLineWidth{
			get{
				return LargestLineWidth_;
			}
			set{
				LargestLineWidth_=value;
			}
		}
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public override float PenY{
			get{
				return PenY_;
			}
			set{
				PenY_=value;
			}
		}
		
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public override bool GoingLeftwards{
			get{
				return GoingLeftwards_;
			}
			set{
				GoingLeftwards_=value;
			}
		}
		
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public override float MaxX{
			get{
				return MaxX_;
			}
			set{
				MaxX_=value;
			}
		}
		
		/// <summary>The current font family in use.</summary>
		internal override FontFace FontFace{
			get{
				return FontFace_;
			}
			set{
				FontFace_=value;
			}
		}
		
	}
	
	public class InlineBoxMeta : LineBoxMeta{
		
		public InlineBoxMeta(BlockBoxMeta block,LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData):base(parent,firstBox,renderData){
			
			HostBlock=block;
			PenX=parent.PenX + firstBox.InlineStyleOffsetLeft;
			LineStart=PenX;
			
		}
		
	}
	
	public class InlineBlockBoxMeta : BlockBoxMeta{
		
		/// <summary>True when this is acting like a BlockBoxMeta.</summary>
		public bool BlockMode;
		
		
		public InlineBlockBoxMeta(BlockBoxMeta block,LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData):base(parent,firstBox,renderData){
			
			HostBlock=block;
			LineStart=PenX;
			
		}
		
		/// <summary>The length of the longest line so far.</summary>
		public override float LargestLineWidth{
			get{
				if(BlockMode){
					return base.LargestLineWidth;
				}
				return HostBlock.LargestLineWidth;
			}
			set{
				if(BlockMode){
					base.LargestLineWidth=value;
				}else{
					HostBlock.LargestLineWidth=value;
				}
			}
		}
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public override float PenY{
			get{
				if(BlockMode){
					return base.PenY;
				}
				return HostBlock.PenY;
			}
			set{
				if(BlockMode){
					base.PenY=value;
				}else{
					HostBlock.PenY=value;
				}
			}
		}
		
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public override bool GoingLeftwards{
			get{
				if(BlockMode){
					return base.GoingLeftwards;
				}
				return HostBlock.GoingLeftwards;
			}
			set{
				if(BlockMode){
					base.GoingLeftwards=value;
				}else{
					HostBlock.GoingLeftwards=value;
				}
			}
		}
		
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public override float MaxX{
			get{
				if(BlockMode){
					return base.MaxX;
				}
				return HostBlock.MaxX;
			}
			set{
				if(BlockMode){
					base.MaxX=value;
				}else{
					HostBlock.MaxX=value;
				}
			}
		}
		
		/// <summary>It's a flow root when we're in block mode.</summary>
		public override bool IsFlowRoot{
			get{
				return BlockMode;
			}
		}
		
		/// <summary>The current font family in use.</summary>
		internal override FontFace FontFace{
			get{
				if(BlockMode){
					return base.FontFace;
				}
				
				return HostBlock.FontFace;
			}
			set{
				if(BlockMode){
					base.FontFace=value;
				}else{
					HostBlock.FontFace=value;
				}
			}
		}
		
	}
	
	
}