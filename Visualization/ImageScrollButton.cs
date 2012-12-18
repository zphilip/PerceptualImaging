using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTKWrapper;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
namespace Perceptual.Visualization
{
    public class ImageScrollButton : ImageButtonObserver
    {
        protected ImageButton sliderButton;
        public ImageButton sliderTrack;
        protected float minVal, maxVal;
        protected int initXpos, initYpos;
        protected int width, height;
        protected int lastVal;
        protected ImageTextButton labelButton;
        protected ImageTextButton valueButton;
        public ImageScrollButton(string name, int xpos, int ypos, int width, int height, float minVal, float maxVal)
        {
            sliderButton = new ImageButton(name + "_slider", true, xpos, ypos, height, height);
            sliderButton.SetDragAndDrop(true);
            sliderButton.SetDragBoundingBox(xpos, ypos, xpos + width - height, ypos);
            sliderTrack = new ImageButton(name + "_slider_track", false, xpos, ypos, width, height);
            this.initXpos = xpos;
            this.initYpos = ypos;
            this.width = width;
            this.height = height;
            this.maxVal = maxVal;
            this.minVal = minVal;
            sliderTrack.SetBackgroundColor(new Color4(0.2f, 0.2f, 0.2f, 1.0f));
            sliderButton.SetSelectedColor(new Color4(0.5f, 0.5f, 0.5f, 0.5f));
            sliderButton.SetBackgroundColor(new Color4(1.0f, 1.0f, 1.0f, 0.5f));
            labelButton = new ImageTextButton(new Message(name, (int)(height * 0.6f)), false, xpos, ypos, width, height);
            valueButton = new ImageTextButton(new Message("", (int)(height * 0.6f)), false, xpos + width, ypos, width, height);
            labelButton.message.SetColor(new Color4(1.0f, 1.0f, 1.0f, 0.7f));
            sliderButton.AddObserver(this);
            labelButton.update();
            UpdateText();
        }
        public float GetValue()
        {
            return (minVal + (maxVal - minVal) * (sliderButton.xpos - initXpos) / (float)(width - height));
            

        }
        public void SetValue(float value)
        {
            sliderButton.xpos = (int)(initXpos + (width - height) * (value - minVal) / (maxVal - minVal));
            UpdateText();
        }
        public bool isChanging()
        {
            return sliderButton.isSelected();
        }
        public void AppendScrollBar(ImageButtonRender render)
        {
            render.Buttons.Add(sliderTrack);

            render.Buttons.Add(labelButton);
            render.Buttons.Add(sliderButton);
            render.Buttons.Add(valueButton);
        }
        public void UpdateText()
        {
            valueButton.message.SetText(GetValue().ToString("F1"));
            valueButton.update();
        }
        public void OnMouseOver(ImageButton button)
        {
        }

        public void OnMouseClick(ImageButton button)
        {
        }
    }
}
