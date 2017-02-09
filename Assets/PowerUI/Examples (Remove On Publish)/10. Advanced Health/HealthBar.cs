using System;using UnityEngine;using PowerUI;public class HealthBar:DynamicTexture{		/// <summary>The active value from 0->1 of this bar.</summary>	public float Health=0f;	/// <summary>The number of lines drawn to display the bar.</summary>	public int PixelLines=0;	/// <summary>The arc of the bar is drawn as a series of lines starting at an inner circle (inner edge of the bar) to the outer circle (outer edge).	/// The following values represent the x and y coordinates of the start (inner) and end (outer) of these lines.	/// They are computed by essentially rotating around the center of the circles.</summary>	public int[] InnerCircleSetX;	public int[] OuterCircleSetX;	public int[] InnerCircleSetY;	public int[] OuterCircleSetY;	/// <summary>The colour of an empty bar.</summary>	public Color UnlitColour=Color.red;	/// <summary>The colour of a full bar.</summary>	public Color LitColour=Color.green;	/// <summary>The y pixel coordinates using the array index as x to draw a quarter circle.	/// The values are essentially mirrored over x to produce a half circle for the ends of the bar.</summary>	public int[] HalfCircleY=new int[]{5,4,4,3,1};			public HealthBar():base(216,81){		// 216 (w) x 81 (h) px.		// The texture will be accessible with dynamic://healthbar						// arcLineCount represents how many lines will be drawn to create the arched part of the bar.		// This value was found by looking for the smallest value 'sweet spot' where all pixels of the curve were filled in.		int arcLineCount=342; 				// Pixel lines is the number of lines drawn to display the bar.		// This is highly important for guaging how many should be red and how many green.		// The division by three makes the spread of health across the bar more even.		// Two ending circles + the 'flat line section' + the arc.		PixelLines=5+5+170+(arcLineCount/3);				// The center/focal point of the arc:		int middleX=175;		int middleY=40;				float theta=0f;		// dTheta is how many rad we must rotate through per line, with the end result of rotating through PI rad (180 degrees).		float dTheta=Mathf.PI/(float)arcLineCount;		InnerCircleSetX=new int[arcLineCount];		OuterCircleSetX=new int[arcLineCount];		InnerCircleSetY=new int[arcLineCount];		OuterCircleSetY=new int[arcLineCount];		for(int i=0;i<arcLineCount;i++){			float cosTheta=Mathf.Cos(theta);			float sinTheta=Mathf.Sin(theta);						InnerCircleSetX[i]=middleX+(int)Math.Round(sinTheta*30f);			InnerCircleSetY[i]=middleY+(int)Math.Round(cosTheta*30f);						OuterCircleSetX[i]=middleX+(int)Math.Round(sinTheta*40f);			OuterCircleSetY[i]=middleY+(int)Math.Round(cosTheta*40f);						theta+=dTheta;		}	}		/// <summary>Increases (or decreases for a negative) the current value of the bar.</summary>	/// <param name="delta">A value from 0->1 representing how much the bar should change by.</param>	public void IncreaseHealth(float delta){		SetHealth(Health+delta);	}		/// <summary>Sets the active value of this bar.</summary>	/// <param name="health">A value from 0->1 indicating how full the bar is.</param>	public void SetHealth(float health){		if(health<0f){			health=0f;		}else if(health>1f){			health=1f;		}				if(health==Health){			return;		}		Health=health;				// The bar changed, so request a refresh:		Refresh();	}		/// <summary>Writes the pixels out to the screen.</summary>	public override void Flush(){				// First, clear:		Clear();				int linesActive=(int)(Health*PixelLines);		if(linesActive<0){			linesActive=0;		}else if(linesActive>PixelLines){			linesActive=PixelLines;		}				// Now draw the ends of the bar (the half circles):		int bottomLeft=linesActive-(PixelLines-5);		if(bottomLeft<0){			bottomLeft=0;		}		// Bottom left:		DrawHalfCircle(5,5,bottomLeft,true);				// Top right:		DrawHalfCircle(175,75,linesActive,false);				// And the rectangular section of the bar along the bottom:		int redLineCount=PixelLines-5-linesActive;		if(redLineCount<0){			redLineCount=0;		}				Color colour=UnlitColour;		int lineCount=0;		for(int x=5;x<=175;x++){			if(lineCount==redLineCount){				colour=LitColour;			}			lineCount++;			DrawLine(x,10,x,0,colour);		}		int arcLines=linesActive-5;		if(arcLines<0){			arcLines=0;		}		// Finally, the arc connecting the flat line to the top right half circle:		DrawArc(arcLines*3);			}		/// <summary>Draws the arched/curved section of the bar.</summary>	/// <param name="linesGreen">The number of lines of the arc that should be coloured green.</param>	public void DrawArc(int linesGreen){		int lineCount=0;		Color colour=LitColour;		int arcLines=InnerCircleSetX.Length;		for(int i=0;i<arcLines;i++){			if(lineCount==linesGreen){				colour=UnlitColour;			}			lineCount++;			DrawLine(InnerCircleSetX[i],InnerCircleSetY[i],OuterCircleSetX[i],OuterCircleSetY[i],colour);		}	}		/// <summary>Draws a half of a circle to the image.</summary>	/// <param name="x0">The x location in pixels of the middle of the circle.</param>	/// <param name="y0">The y location in pixels of the middle of the circle.</param>	/// <param name="linesGreen">The number of lines out of 5 that should be coloured green.</param>	/// <param name="leftwards">True leftwards means that the greenness fills from right to left - it travels left.</param>	public void DrawHalfCircle(int x0,int y0,int linesGreen,bool leftwards){ 		if(leftwards){			linesGreen=5-linesGreen;		}				// y^2   = 25 - x^2.		// Radius is always 5. => x^2 + y^2 = 25.		// Must use this as we need to scan from the left to right for linesGreen.				int lineCount=0;		Color colour=leftwards?UnlitColour:LitColour;				for(int i=4;i>=0;i--){			if(lineCount==linesGreen){				colour=leftwards?LitColour:UnlitColour;			}			lineCount++;			int y=HalfCircleY[i];			DrawLine(x0-i,y0-y,x0-i,y0+y,colour);		}	}	}