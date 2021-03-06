﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BGC.Audio.Visualization
{

    public static class PlotUtility
    {
        public static Color JetMap(float value, float min, float max)
        {
            float z = 4f * (value - min) / (max - min);

            return new Color(
                r: Mathf.Clamp(1.5f - Mathf.Abs(z - 3f), 0f, 1f),
                g: Mathf.Clamp(1.5f - Mathf.Abs(z - 2f), 0f, 1f),
                b: Mathf.Clamp(1.5f - Mathf.Abs(z - 1f), 0f, 1f));
        }

        public static PlotData GetLinePlot(
            float[] dataPoints,
            Vector2? xRangeOpt = null,
            Vector2? yRangeOpt = null,
            int texH = 300,
            int texW = 400,
            Color? lineColorOpt = null)
        {
            Color lineColor;
            Vector2 xRange;
            Vector2 yRange;

            if (lineColorOpt.HasValue)
            {
                lineColor = lineColorOpt.Value;
            }
            else
            {
                lineColor = Color.blue;
            }

            if (xRangeOpt.HasValue)
            {
                xRange = xRangeOpt.Value;
            }
            else
            {
                xRange = new Vector2(0, dataPoints.Length + 1);
            }

            if (yRangeOpt.HasValue)
            {
                yRange = yRangeOpt.Value;
            }
            else
            {
                float yMin = float.PositiveInfinity;
                float yMax = float.NegativeInfinity;

                //Find min and max
                for (int i = 0; i < dataPoints.Length; i++)
                {
                    if (dataPoints[i] > yMax)
                    {
                        yMax = dataPoints[i];
                    }

                    if (dataPoints[i] < yMin)
                    {
                        yMin = dataPoints[i];
                    }
                }

                //Find the magnitude of the difference
                float diff = yMax - yMin;
                float orderOfMag = Mathf.Log10(diff);

                float newYMin = Mathf.Floor(yMin * Mathf.Pow(10f, orderOfMag)) / Mathf.Pow(10f, orderOfMag);
                float newYMax = Mathf.Ceil(yMax * Mathf.Pow(10f, orderOfMag)) / Mathf.Pow(10f, orderOfMag);

                yRange = new Vector2(newYMin, newYMax);
            }

            Texture2D tex = new Texture2D(texW, texH);

            float xScaleFactor = texW / (xRange.y - xRange.x);
            float yScaleFactor = texH / (yRange.y - yRange.x);

            int xPixel;
            int yPixel;

            //First white out texture
            for (xPixel = 0; xPixel < texW; xPixel++)
            {
                for (yPixel = 0; yPixel < texH; yPixel++)
                {
                    tex.SetPixel(xPixel, yPixel, Color.white);
                }
            }

            if (yRange.x < 0f && 0f < yRange.y)
            {
                //Draw XAxis
                yPixel = Mathf.FloorToInt(texH * -yRange.x / (yRange.y - yRange.x));
                for (xPixel = 0; xPixel < texW; xPixel++)
                {
                    tex.SetPixel(xPixel, yPixel, Color.black);
                }
            }

            if (xRange.x < 0f && 0f < xRange.y)
            {
                //Draw YAxis
                xPixel = Mathf.FloorToInt(texW * -xRange.x / (xRange.y - xRange.x));
                for (yPixel = 0; yPixel < texH; xPixel++)
                {
                    tex.SetPixel(xPixel, yPixel, Color.black);
                }
            }

            int x0;
            int x1 = Mathf.FloorToInt(xScaleFactor * (1 - xRange.x));
            int y0;
            int y1 = Mathf.FloorToInt(yScaleFactor * (dataPoints[0] - yRange.x));

            //Draw Lines
            for (int sample = 1; sample < dataPoints.Length; sample++)
            {
                x0 = x1;
                x1 = Mathf.FloorToInt(xScaleFactor * (1 + sample - xRange.x));

                y0 = y1;
                y1 = Mathf.FloorToInt(yScaleFactor * (dataPoints[sample] - yRange.x));

                DrawLine(x0, y0, x1, y1, tex, lineColor);
            }

            tex.Apply();

            SimplePlotData data = new SimplePlotData()
            {
                plot = tex,
                xBounds = xRange,
                yBounds = yRange,
            };

            return data;
        }



        public static PlotData GetPointAndLinePlot(
            List<PlotPoint> dataPoints,
            Vector2? xRangeOpt = null,
            Vector2? yRangeOpt = null,
            int texH = 600,
            int texW = 800,
            Color? lineColorOpt = null,
            Color? goodPointColorOpt = null,
            Color? badPointColorOpt = null,
            int pointRadius = 3)
        {
            Color lineColor = Color.blue;
            Color goodPointColor = Color.green;
            Color badPointColor = Color.red;
            Vector2 xRange;
            Vector2 yRange;

            if (lineColorOpt.HasValue)
            {
                lineColor = lineColorOpt.Value;
            }

            if (goodPointColorOpt.HasValue)
            {
                goodPointColor = goodPointColorOpt.Value;
            }

            if (badPointColorOpt.HasValue)
            {
                badPointColor = badPointColorOpt.Value;
            }

            if (xRangeOpt.HasValue)
            {
                xRange = xRangeOpt.Value;
            }
            else
            {
                xRange = new Vector2(0, dataPoints.Count + 2);
            }

            if (yRangeOpt.HasValue)
            {
                yRange = yRangeOpt.Value;
            }
            else
            {
                float yMin = float.PositiveInfinity;
                float yMax = float.NegativeInfinity;

                //Find min and max
                for (int i = 0; i < dataPoints.Count; i++)
                {
                    if (dataPoints[i].parameterValue > yMax)
                    {
                        yMax = dataPoints[i].parameterValue;
                    }

                    if (dataPoints[i].parameterValue < yMin)
                    {
                        yMin = dataPoints[i].parameterValue;
                    }
                }

                //Find the magnitude of the difference
                float diff = yMax - yMin;
                float orderOfMag = Mathf.Floor(Mathf.Log10(diff));

                float newYMin = Mathf.Floor(yMin / Mathf.Pow(10f, orderOfMag) - 1) * Mathf.Pow(10f, orderOfMag);
                float newYMax = Mathf.Ceil(yMax / Mathf.Pow(10f, orderOfMag) + 1) * Mathf.Pow(10f, orderOfMag);

                yRange = new Vector2(newYMin, newYMax);
            }

            Texture2D tex = new Texture2D(texW, texH);

            float xScaleFactor = texW / (xRange.y - xRange.x);
            float yScaleFactor = texH / (yRange.y - yRange.x);

            int xPixel;
            int yPixel;

            //First white out texture
            for (xPixel = 0; xPixel < texW; xPixel++)
            {
                for (yPixel = 0; yPixel < texH; yPixel++)
                {
                    tex.SetPixel(xPixel, yPixel, Color.white);
                }
            }

            //Draw XAxis
            if (yRange.x < 0f && 0f < yRange.y)
            {
                yPixel = Mathf.FloorToInt(yScaleFactor - yRange.x);
                for (xPixel = 0; xPixel < texW; xPixel++)
                {
                    tex.SetPixel(xPixel, yPixel, Color.black);
                }
            }

            //Draw YAxis
            if (xRange.x < 0f && 0f < xRange.y)
            {
                xPixel = Mathf.FloorToInt(xScaleFactor * -xRange.x);
                for (yPixel = 0; yPixel < texH; xPixel++)
                {
                    tex.SetPixel(xPixel, yPixel, Color.black);
                }
            }

            //Draw Grid
            {
                float diff;
                float gridDelta;
                int gridLineCount;

                //Draw X Gridlines
                diff = yRange.y - yRange.x;
                gridDelta = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(diff / 5)));
                gridLineCount = Mathf.RoundToInt(diff / gridDelta) + 1;
                for (int i = 0; i < gridLineCount; i++)
                {
                    yPixel = Mathf.Clamp(Mathf.FloorToInt(yScaleFactor * i * gridDelta), 0, texH - 1);
                    for (xPixel = 0; xPixel < texW; xPixel++)
                    {
                        tex.SetPixel(xPixel, yPixel, Color.gray);
                    }
                }

                //Draw Y Gridlines
                diff = xRange.y - xRange.x;
                gridDelta = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(diff / 5)));
                gridLineCount = Mathf.RoundToInt(diff / gridDelta) + 1;
                for (int i = 0; i < gridLineCount; i++)
                {
                    xPixel = Mathf.Clamp(Mathf.FloorToInt(xScaleFactor * i * gridDelta), 0, texW - 1);
                    for (yPixel = 0; yPixel < texH; yPixel++)
                    {
                        tex.SetPixel(xPixel, yPixel, Color.gray);
                    }
                }

            }

            int x0;
            int y0;
            int x1 = Mathf.FloorToInt(xScaleFactor * (1 - xRange.x));
            int y1 = Mathf.FloorToInt(yScaleFactor * (dataPoints[0].parameterValue - yRange.x));

            //Draw Lines
            for (int sample = 1; sample < dataPoints.Count; sample++)
            {
                x0 = x1;
                x1 = Mathf.FloorToInt(xScaleFactor * (1 + sample - xRange.x));

                y0 = y1;
                y1 = Mathf.FloorToInt(yScaleFactor * (dataPoints[sample].parameterValue - yRange.x));

                DrawLine(x0, y0, x1, y1, tex, lineColor);
            }

            //Draw Points
            for (int sample = 0; sample < dataPoints.Count; sample++)
            {
                x0 = Mathf.FloorToInt(xScaleFactor * (1 + sample - xRange.x));
                y0 = Mathf.FloorToInt(yScaleFactor * (dataPoints[sample].parameterValue - yRange.x));

                if (dataPoints[sample].responseCorrect)
                {
                    DrawCircle(x0, y0, pointRadius, tex, goodPointColor);
                }
                else
                {
                    DrawCircle(x0, y0, pointRadius, tex, badPointColor);
                }
            }

            tex.Apply();

            SimplePlotData data = new SimplePlotData()
            {
                plot = tex,
                xBounds = xRange,
                yBounds = yRange,
            };

            return data;
        }

        public static void DrawLine(int x0, int y0, int x1, int y1, Texture2D tex, Color color)
        {
            int x;
            int y;
            //Handle edge case
            if (x0 == x1)
            {
                if (y0 == y1)
                {
                    tex.SetPixel(x0, y0, color);
                    return;
                }

                if (y0 > y1)
                {
                    int temp = y1;
                    y1 = y0;
                    y0 = temp;
                }

                for (y = y0; y < y1; y++)
                {
                    tex.SetPixel(x0, y, color);
                }
                return;
            }

            //Order points
            if (x0 > x1)
            {
                int temp = x1;
                x1 = x0;
                x0 = temp;

                temp = y1;
                y1 = y0;
                y0 = temp;
            }

            float deltaErr = Mathf.Abs(((float)y1 - y0) / (x1 - x0));

            float Err;
            int adj = 1;
            if (deltaErr <= 1.0f)
            {
                Err = deltaErr - 0.5f;

                y = y0;

                if (y0 > y1)
                {
                    adj = -1;
                }


                for (x = x0; x < x1; x++)
                {
                    tex.SetPixel(x, y, color);
                    Err += deltaErr;
                    if (Err >= 0.5f)
                    {
                        y += adj;
                        Err -= 1.0f;
                    }
                }
            }
            else
            {
                if (y0 > y1)
                {
                    int temp = x1;
                    x1 = x0;
                    x0 = temp;

                    temp = y1;
                    y1 = y0;
                    y0 = temp;
                }

                deltaErr = Mathf.Abs(((float)x1 - x0) / (y1 - y0));
                Err = deltaErr - 0.5f;


                if (x0 > x1)
                {
                    adj = -1;
                }

                x = x0;
                for (y = y0; y < y1; y++)
                {
                    tex.SetPixel(x, y, color);
                    Err += deltaErr;
                    if (Err >= 0.5f)
                    {
                        x += adj;
                        Err -= 1.0f;
                    }
                }
            }
        }


        public static void DrawCircle(int x0, int y0, int radius, Texture2D tex, Color color)
        {
            int x;
            int y;

            int x1;
            int y1;

            for (y = -radius; y <= radius; y++)
            {
                for (x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        x1 = x0 + x;
                        y1 = y0 + y;

                        if (x1 >= 0 && x1 < tex.width && y1 >= 0 && y1 < tex.height)
                        {
                            tex.SetPixel(x1, y1, color);
                        }
                    }
                }
            }
        }
    }

}
