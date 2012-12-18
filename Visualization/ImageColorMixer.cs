using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perceptual.Foundation;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper;
using System.Drawing;
using System.Drawing.Imaging;
using Cloo;
using OpenTKWrapper;

using OpenTKWrapper.CLGLInterop;
namespace Perceptual.Visualization
{

    public class ImageColorMixer : ImageButtonRender, ImageButtonObserver
    {
        public const int NUM_CONTROL_PTS = 5;
        protected ImageScrollButton hueScroll;
        protected ImageScrollButton saturationScroll;
        protected ImageScrollButton valueScroll;
        protected ImageButton mixedColor, chosenColor;
        protected ImageButton[] controlPoints;

        protected ImageButton[] controlPointsBorder;
        protected ImageButton[] stems;
        protected int xpos, ypos, width, height;
        protected CLCalc.Program.Kernel kernelUpdateImageGradients;
        protected CLCalc.Program.Image2D hueImage, saturationImage, valueImage;
        public CLCalc.Program.Image2D mixedImage;
        protected float[] hueBuffer, saturationBuffer, valueBuffer, mixedBuffer, controlColorsBuffer, controlAmpsBuffer;
        protected int scrollWidth = 0;
        protected int controlXpos;
        protected int colorIndex = 0;
        protected CLCalc.Program.Variable controlColors;
        public float[] GetColors()
        {
            return controlColorsBuffer;
        }
        public float[] GetAmplitudes()
        {
            return controlAmpsBuffer;
        }
        protected CLCalc.Program.Variable controlAmps;
        public override void Draw(GLAdvancedRender glw)
        {
            Update();
            base.Draw(glw);
        }
        public override void Initialize(BaseCameraApplication app, GLAdvancedRender glw)
        {
            base.Initialize(app, glw);
            Initialize();
            AppendColorMixer(this);

        }

        public ImageColorMixer(int xpos, int ypos, int width, int height)
        {

            this.xpos = xpos;
            this.ypos = ypos;
            this.width = width;
            this.height = height;
            this.scrollWidth = width / 2 - 40;

            this.controlXpos = xpos + scrollWidth + 40;
            hueScroll = new ImageScrollButton("Hue", xpos, ypos, scrollWidth, 20, 0, 1.0f);

            saturationScroll = new ImageScrollButton("Saturation", xpos, ypos + 30, scrollWidth, 20, 0, 1.0f);
            valueScroll = new ImageScrollButton("Brightness", xpos, ypos + 60, scrollWidth, 20, 0, 1.0f);
            hueImage = new CLCalc.Program.Image2D(hueBuffer = new float[scrollWidth * 4], scrollWidth, 1);
            saturationImage = new CLCalc.Program.Image2D(saturationBuffer = new float[scrollWidth * 4], scrollWidth, 1);
            valueImage = new CLCalc.Program.Image2D(valueBuffer = new float[scrollWidth * 4], scrollWidth, 1);
            mixedImage = new CLCalc.Program.Image2D(mixedBuffer = new float[scrollWidth * 4], scrollWidth, 1);
            controlColors = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite, controlColorsBuffer = new float[4 * NUM_CONTROL_PTS]));
            controlAmps = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite, controlAmpsBuffer = new float[NUM_CONTROL_PTS]));
            hueScroll.sliderTrack.SetDynamicImage(hueImage);
            saturationScroll.sliderTrack.SetDynamicImage(saturationImage);
            valueScroll.sliderTrack.SetDynamicImage(valueImage);
            mixedColor = new ImageButton("Mixture", false, controlXpos, ypos + 90, scrollWidth, 20);
            chosenColor = new ImageButton("Chosen", false, xpos, ypos + 90, scrollWidth, 20);
            mixedColor.SetDynamicImage(mixedImage);

            controlPoints = new ImageButton[NUM_CONTROL_PTS];
            stems = new ImageButton[NUM_CONTROL_PTS];
            controlPointsBorder = new ImageButton[NUM_CONTROL_PTS];
            Bitmap bitmap = new Bitmap(20, 20);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.Clear(Color.FromArgb(0, 0, 0, 0));
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.FillEllipse(new SolidBrush(Color.White), 1, 1, 17, 17);
            }

            hueScroll.SetValue(0.1f);
            valueScroll.SetValue(0.5f);
            saturationScroll.SetValue(0.5f);
            float4 currentColor = HSVtoRGB(new float4(hueScroll.GetValue(), saturationScroll.GetValue(), valueScroll.GetValue(), 1.0f));
            colorIndex = NUM_CONTROL_PTS / 2;
            for (int i = 0; i < NUM_CONTROL_PTS; i++)
            {
                controlPoints[i] = new ImageButton("control_" + i, bitmap, true, (int)(controlXpos + (i + 0.5f) * (scrollWidth) / (float)NUM_CONTROL_PTS), ypos + ((i == colorIndex) ? 50 : 70), 20, 20);
                controlPointsBorder[i] = new ImageButton("control_" + i, new Bitmap(20, 20), false, (int)(controlXpos + i * (scrollWidth) / (float)NUM_CONTROL_PTS), ypos + 70, 20, 20);
                controlPoints[i].SetDragAndDrop(true);
                controlPoints[i].SetDragBoundingBox(controlXpos - 10, ypos, controlXpos + scrollWidth - 10, ypos + 70);
                stems[i] = new ImageButton("stem_" + i, false, (int)(controlXpos + i * (scrollWidth) / (float)NUM_CONTROL_PTS), 90, 2, 20);
                stems[i].SetBackgroundColor(Color4.White);
                controlPoints[i].SetForegroundColor(HSVtoRGB(new float4(i / (float)(NUM_CONTROL_PTS), saturationScroll.GetValue(), valueScroll.GetValue(), 1.0f)));
                UpdateControlPoint(i);
                controlPoints[i].AddObserver(this);
            }

        }
        protected Color4 negativeColor(Color4 c)
        {
            float4 hsv = RGBtoHSV(c);
            hsv.x = 0;
            hsv.y = 0;
            hsv.z = 1.0f - hsv.z;
            hsv.w = 1.0f;
            return HSVtoRGB(hsv);
        }
        protected void UpdateControlPoint(int index)
        {
            Bitmap bitmap = controlPointsBorder[index].bitmap;
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {

                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(Color.FromArgb(0, 0, 0, 0));
                Color4 c = negativeColor(controlPoints[index].GetForegroundColor());
                c.A = 0.75f;
                if (index == colorIndex)
                {
                    c = Color4.Yellow;
                }
                gfx.DrawEllipse(new Pen(Color.FromArgb(c.ToArgb()), 2.0f), 1, 1, 17, 17);
                stems[index].SetBackgroundColor(c);
            }
            controlPointsBorder[index].UpdateBitmap();
            controlPointsBorder[index].xpos = controlPoints[index].xpos;
            controlPointsBorder[index].ypos = controlPoints[index].ypos;
            stems[index].xpos = controlPoints[index].xpos + 9;
            stems[index].ypos = controlPoints[index].ypos + 19;
            stems[index].height = controlPoints[index].maxY - controlPoints[index].ypos + 21;
        }

        public void Initialize()
        {
            try
            {
                CLCalc.Program.Compile(src);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            kernelUpdateImageGradients = new CLCalc.Program.Kernel("UpdateGradients");

        }
        public void AppendColorMixer(ImageButtonRender imageRender)
        {
            hueScroll.AppendScrollBar(imageRender);
            saturationScroll.AppendScrollBar(imageRender);
            valueScroll.AppendScrollBar(imageRender);
            imageRender.Buttons.Add(mixedColor);
            imageRender.Buttons.Add(chosenColor);

            for (int i = 0; i < NUM_CONTROL_PTS; i++)
            {

                imageRender.Buttons.Add(controlPoints[i]);

                imageRender.Buttons.Add(controlPointsBorder[i]);

                imageRender.Buttons.Add(stems[i]);
            }


        }
        public bool isChanging()
        {
            for (int i = 0; i < NUM_CONTROL_PTS; i++)
            {
                if (controlPoints[i].isSelected())
                {
                    return true;
                }
            }
            return (hueScroll.isChanging() || valueScroll.isChanging() || saturationScroll.isChanging());
        }
        public void Reload()
        {
            controlAmps.WriteToDevice(controlAmpsBuffer);
            controlColors.WriteToDevice(controlColorsBuffer);
            colorIndex = NUM_CONTROL_PTS / 2;
            for (int i = 0; i < NUM_CONTROL_PTS; i++)
            {
                controlPoints[i].ypos = (int)((controlPoints[i].maxY - controlPoints[i].minY) * controlAmpsBuffer[i] + controlPoints[i].minY);
                controlPoints[i].xpos = (int)((controlPoints[i].maxX - controlPoints[i].minX) * controlColorsBuffer[4 * i + 3] + controlPoints[i].minX);
                controlPoints[i].SetForegroundColor(new Color4(controlColorsBuffer[4 * i], controlColorsBuffer[4 * i + 1], controlColorsBuffer[4 * i + 2], 1.0f));
                UpdateControlPoint(i);
            }
            float4 hsv = RGBtoHSV(controlPoints[colorIndex].GetForegroundColor());
            chosenColor.SetBackgroundColor(controlPoints[colorIndex].GetForegroundColor());
            hueScroll.SetValue(hsv.x);
            saturationScroll.SetValue(hsv.y);
            valueScroll.SetValue(hsv.z);

            Update();
        }
        public CLCalc.Program.Image2D CreateMixedImage(float4[] colors, float[] amplitudes, float[] positions)
        {
            float[] colorBuffer = new float[this.controlColorsBuffer.Length];
            float[] ampBuffer = new float[this.controlAmpsBuffer.Length];
            for (int i = 0; i < ImageColorMixer.NUM_CONTROL_PTS; i++)
            {
                float4 c = colors[i];
                colorBuffer[4 * i] = c.x;
                colorBuffer[4 * i + 1] = c.y;
                colorBuffer[4 * i + 2] = c.z;
                colorBuffer[4 * i + 3] = positions[i];
                ampBuffer[i] = amplitudes[i];
            }
            controlColors.WriteToDevice(colorBuffer);
            controlAmps.WriteToDevice(ampBuffer);
            kernelUpdateImageGradients.Execute(new CLCalc.Program.MemoryObject[] { 
                    Utilities.ToMemoryObject(hueScroll.GetValue()), 
                    Utilities.ToMemoryObject(saturationScroll.GetValue()), 
                    Utilities.ToMemoryObject(valueScroll.GetValue()), 
                    Utilities.ToMemoryObject(colorIndex),
                    controlColors,
                    controlAmps,
                    hueImage, 
                    saturationImage, 
                    valueImage, 
                    mixedImage 
            }, scrollWidth);
            return mixedImage;
        }
        public void Update()
        {
            float4 currentColor = HSVtoRGB(new float4(hueScroll.GetValue(), saturationScroll.GetValue(), valueScroll.GetValue(), 1.0f));
            chosenColor.SetBackgroundColor(currentColor);


            if (hueScroll.isChanging() || saturationScroll.isChanging() || valueScroll.isChanging())
            {
                controlPoints[colorIndex].SetForegroundColor(currentColor);

            }

            for (int i = 0; i < NUM_CONTROL_PTS; i++)
            {

                UpdateControlPoint(i);
                controlAmpsBuffer[i] = (controlPoints[i].ypos - controlPoints[i].minY) / (float)(controlPoints[i].maxY - controlPoints[i].minY);
                float4 c = controlPoints[i].GetForegroundColor();
                controlColorsBuffer[4 * i] = c.x;
                controlColorsBuffer[4 * i + 1] = c.y;
                controlColorsBuffer[4 * i + 2] = c.z;
                controlColorsBuffer[4 * i + 3] = (controlPoints[i].xpos - controlPoints[i].minX) / (float)(controlPoints[i].maxX - controlPoints[i].minX);
            }
            controlAmps.WriteToDevice(controlAmpsBuffer);

            controlColors.WriteToDevice(controlColorsBuffer);
            kernelUpdateImageGradients.Execute(new CLCalc.Program.MemoryObject[] { 
                    Utilities.ToMemoryObject(hueScroll.GetValue()), 
                    Utilities.ToMemoryObject(saturationScroll.GetValue()), 
                    Utilities.ToMemoryObject(valueScroll.GetValue()), 
                    Utilities.ToMemoryObject(colorIndex),
                    controlColors,
                    controlAmps,
                    hueImage, 
                    saturationImage, 
                    valueImage, 
                    mixedImage 
            }, scrollWidth);

            hueScroll.UpdateText();
            saturationScroll.UpdateText();
            valueScroll.UpdateText();

        }
        private float4 HSVtoRGB(float4 hsv)
        {
            float4 rgb;
            if (hsv.y == 0)
            {
                rgb.x = hsv.z;
                rgb.y = hsv.z;
                rgb.z = hsv.z;
            }
            else
            {
                float varH = hsv.x * 6;
                float varI = (float)Math.Floor(varH);
                float var1 = hsv.z * (1 - hsv.y);
                float var2 = hsv.z * (1 - (hsv.y * (varH - varI)));
                float var3 = hsv.z * (1 - (hsv.y * (1 - (varH - varI))));

                if (varI == 0)
                {
                    rgb.x = hsv.z;
                    rgb.y = var3;
                    rgb.z = var1;
                }
                else if (varI == 1)
                {
                    rgb.x = var2;
                    rgb.y = hsv.z;
                    rgb.z = var1;
                }
                else if (varI == 2)
                {
                    rgb.x = var1;
                    rgb.y = hsv.z;
                    rgb.z = var3;
                }
                else if (varI == 3)
                {
                    rgb.x = var1;
                    rgb.y = var2;
                    rgb.z = hsv.z;
                }
                else if (varI == 4)
                {
                    rgb.x = var3;
                    rgb.y = var1;
                    rgb.z = hsv.z;
                }
                else
                {
                    rgb.x = hsv.z;
                    rgb.y = var1;
                    rgb.z = var2;
                }
            }
            rgb.w = hsv.w;
            return rgb;
        }

        // Expects and returns values in the range 0 to 1
        private float4 RGBtoHSV(float4 rgb)
        {
            float4 hsv = new float4(0, 0, 0, 0);
            float varMin = Math.Min(rgb.x, Math.Min(rgb.y, rgb.z));
            float varMax = Math.Max(rgb.x, Math.Max(rgb.y, rgb.z));
            float delMax = varMax - varMin;

            hsv.z = varMax;

            if (delMax == 0)
            {
                hsv.x = 0;
                hsv.y = 0;
            }
            else
            {
                float delR = (((varMax - rgb.x) / 6) + (delMax / 2)) / delMax;
                float delG = (((varMax - rgb.y) / 6) + (delMax / 2)) / delMax;
                float delB = (((varMax - rgb.z) / 6) + (delMax / 2)) / delMax;

                hsv.y = delMax / varMax;

                if (rgb.x == varMax)
                {
                    hsv.x = delB - delG;
                }
                else if (rgb.y == varMax)
                {
                    hsv.x = (1.0f / 3) + delR - delB;
                }
                else
                {
                    hsv.x = (2.0f / 3) + delG - delR;
                }

                if (hsv.x < 0)
                {
                    hsv.x += 1;
                }

                if (hsv.x > 1)
                {
                    hsv.x -= 1;
                }
            }
            return hsv;
        }

        string src = "#define NUM_CONTROL_PTS " + NUM_CONTROL_PTS + @"

    inline float4 HSVtoRGB(float4 hsv){
        float4 rgb;
        if (hsv.y == 0)
        {
            rgb.x = hsv.z;
            rgb.y = hsv.z;
            rgb.z = hsv.z;
        }
        else
        {
            float varH = hsv.x * 6;
            float varI = floor(varH);
            float var1 = hsv.z * (1 - hsv.y);
            float var2 = hsv.z * (1 - (hsv.y * (varH - varI)));
            float var3 = hsv.z * (1 - (hsv.y * (1 - (varH - varI))));

            if (varI == 0)
            {
                rgb.x = hsv.z;
                rgb.y = var3;
                rgb.z = var1;
            }
            else if (varI == 1)
            {
                rgb.x = var2;
                rgb.y = hsv.z;
                rgb.z = var1;
            }
            else if (varI == 2)
            {
                rgb.x = var1;
                rgb.y = hsv.z;
                rgb.z = var3;
            }
            else if (varI == 3)
            {
                rgb.x = var1;
                rgb.y = var2;
                rgb.z = hsv.z;
            }
            else if (varI == 4)
            {
                rgb.x = var3;
                rgb.y = var1;
                rgb.z = hsv.z;
            }
            else
            {
                rgb.x = hsv.z;
                rgb.y = var1;
                rgb.z = var2;
            }
        }
        rgb.w=hsv.w;
        return rgb;
    }

    // Expects and returns values in the range 0 to 1
    inline float4 RGBtoHSV(float4 rgb)
    {
        float4 hsv = (float4)(0, 0, 0, 0);
        float varMin = min(rgb.x, min(rgb.y, rgb.z));
        float varMax = max(rgb.x, max(rgb.y, rgb.z));
        float delMax = varMax - varMin;

        hsv.z = varMax;

        if (delMax == 0)
        {
            hsv.x = 0;
            hsv.y = 0;
        }
        else
        {
            float delR = (((varMax - rgb.x) / 6) + (delMax / 2)) / delMax;
            float delG = (((varMax - rgb.y) / 6) + (delMax / 2)) / delMax;
            float delB = (((varMax - rgb.z) / 6) + (delMax / 2)) / delMax;

            hsv.y = delMax / varMax;

            if (rgb.x == varMax)
            {
                hsv.x = delB - delG;
            }
            else if (rgb.y == varMax)
            {
                hsv.x = (1.0 / 3) + delR - delB;
            }
            else 
            {
                hsv.x = (2.0 / 3) + delG - delR;
            }

            if (hsv.x < 0)
            {
                hsv.x += 1;
            }

            if (hsv.x > 1)
            {
                hsv.x -= 1;
            }
        }
        hsv.w=rgb.w;
        return hsv;
    }

    inline float weightedColor(float4 color,float pos,float amp){
        if(amp<0.005f){
            return (fabs(pos-color.w)<1.0f/255.0f)?1.0f:0.0f;        
        } if(amp>0.995f){
            return 0.0f;
        } else {
            float ex=NUM_CONTROL_PTS*(pos-color.w)/amp;
            return exp(-0.5f*ex*ex);
        }
    }
            kernel void UpdateGradients(
float hue,float sat,float val,
int index,
global float4* controlColors,
global float* amplitudes,
write_only image2d_t hueImage,
write_only image2d_t satImage,
write_only image2d_t valImage,
write_only image2d_t mixedImage)
            {
                int i = get_global_id(0);
                int j = get_global_id(1);
                int2 coords = (int2)(i,j);   
                float x=1.0f-i/(float)get_global_size(0);
	            write_imagef(hueImage, coords,HSVtoRGB((float4)(x,1.0f,0.5f,1.0f)));  
	            write_imagef(satImage, coords,HSVtoRGB((float4)(hue,x,0.5f,1.0f)));  
	            write_imagef(valImage, coords,HSVtoRGB((float4)(hue,sat,x,1.0f)));  
                
                float4 color=(float4)(0,0,0,0);
                float wsum=0.0f,w;
                for(int i=0;i<NUM_CONTROL_PTS;i++){
                    w=weightedColor(controlColors[i],x,amplitudes[i]);
                    color+=w*RGBtoHSV(controlColors[i]);
                    wsum+=w;
                }
                color/=wsum;
                color.w=1;
	            write_imagef(mixedImage, coords,HSVtoRGB(color));  
            }
        ";

        public void OnMouseOver(ImageButton button)
        {
        }

        public void OnMouseClick(ImageButton button)
        {
            for (int i = 0; i < NUM_CONTROL_PTS; i++)
            {
                if (controlPoints[i] == button)
                {
                    colorIndex = i;
                    float4 hsv = RGBtoHSV(controlPoints[i].GetForegroundColor());
                    hueScroll.SetValue(hsv.x);
                    saturationScroll.SetValue(hsv.y);
                    valueScroll.SetValue(hsv.z);
                    hueScroll.UpdateText();
                    saturationScroll.UpdateText();
                    valueScroll.UpdateText();
                    return;
                }
            }
        }
    }

}
