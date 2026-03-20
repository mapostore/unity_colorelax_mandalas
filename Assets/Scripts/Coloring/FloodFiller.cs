using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class FloodFiller  {
	public static Color tarGetCol;
    public static readonly List<int> lastFillIndices = new List<int>();
	public struct Point
	{
		public short x;
		public short y;
		public Point(short aX, short aY) { x = aX; y = aY; }
		public Point(int aX, int aY) : this((short)aX, (short)aY) { }
	}
	// Use this for initialization
	public static void RevisedQueueFloodFill(Texture2D targetTex,int hitX,int hitY, Color replaceColor,bool dontPush)
	{
        RevisedQueueFloodFill(targetTex, null, hitX, hitY, replaceColor, dontPush);
	}


    public static void RevisedQueueFloodFill(Texture2D targetTex, Texture2D referenceTex, int hitX, int hitY, Color replaceColor, bool dontPush)
	{
        lastFillIndices.Clear();

		Color32[] pixels = targetTex.GetPixels32 ();// store image colors
        Color32[] referencePixels = referenceTex != null ? referenceTex.GetPixels32() : pixels;
		int w = targetTex.width;
		int h = targetTex.height;
		Color targetColor = referencePixels[hitX+hitY*w];
		tarGetCol = pixels[hitX+hitY*w];
		if (referenceTex == null && tarGetCol == replaceColor) return;	
		Queue<Point> q = new Queue<Point>();
		q.Enqueue(new Point(hitX,hitY));// add hit point to queue
		Point n, t, u;
		while (q.Count > 0)
		{
			n = q.Dequeue();
			if (referencePixels[n.x+n.y*w] == targetColor&&referencePixels[n.x+ n.y*w] != Color.black)
			{
				
				t = n;
				while ((t.x > 0) && (referencePixels[t.x+ t.y*w] == targetColor)&&(referencePixels[t.x+ t.y*w] != Color.black))//check whether point in context is within bounds and does not match new fill color or border color
				{
                    int idx = t.x + t.y * w;
					pixels[idx] = replaceColor; // change color of reference point to replaced color
                    lastFillIndices.Add(idx);
					t.x--;
				}
				int XMin = t.x + 1;
								
				t = n;
				t.x++;
				while ((t.x < w - 1) &&
				       (referencePixels[t.x+ t.y*w] == targetColor)&&(referencePixels[t.x+ t.y*w] != Color.black))//check whether point in context is within bounds and does not match new fill color or border color
				{
                    int idx = t.x + t.y * w;
					pixels[idx] = replaceColor;// change color of reference point to replaced color
                    lastFillIndices.Add(idx);
					t.x++;
				}

				int XMax = t.x - 1;
				t = n;
				t.y++;
				
				u = n;
				u.y--;
				
				for ( int i = XMin; i <= XMax; i++)
				{
					t.x =(short) i;
					u.x = (short)i;
					//DFS to check if point does not match replace color
					if ((t.y <h- 1) &&
					    (referencePixels[t.x+ t.y*w] == targetColor)) q.Enqueue(t);
					
					if ((u.y >= 0) &&
					    (referencePixels[u.x+ u.y*w] == targetColor)) q.Enqueue(u);
				}
			}
		}
		targetTex.SetPixels32 (pixels);

	}
}
