using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothScrollBug {
    public class CycleVE : AbsoluteLayout {

        Border photoBorder;
        Image photo;
        Label titleLabel;
        Label bodyLabel;
        Label priceLabel;
        Label dateLabel;
        Size screenSize = new Size(-1,-1);
        List<string> titleList = new() { "Good Product", "Bad Product", "Amazing One", "Buy This", "Your Friend Likes This", "Another Thingy", "One More Thingy", "Thingy Two" };
        List<string> aboutProduct = new() { "This is a good product", "Everyone loves this product", "You should buy this product", "Get more of this", "Buy this one too" };
        List<string> photoList = new() { "photo1.jpg", "photo2.jpg", "photo3.jpg", "photo4.jpg", "photo5.jpg", "photo6.jpg" };
        public CycleVE() {

            photoBorder = new();
            photo = new();

            //FreezeBehavior photoFreeze = new();
            //photo.Behaviors.Add(photoFreeze);
            //photo.freezeView();

            titleLabel = new();
            bodyLabel = new();
            dateLabel = new();
            priceLabel = new();
            this.Add(priceLabel);
            this.Add(photoBorder);
            photoBorder.Content = photo;
            photoBorder.Shadow = new() { Radius = 5, Brush = Colors.Black, Offset = new Point(0,5)};
            this.Add(titleLabel);
            this.Add(bodyLabel);
            this.Add(dateLabel);
            photo.Aspect = Aspect.AspectFill;

            photoBorder.StrokeShape = new RoundRectangle() { CornerRadius = 15 };
            photoBorder.StrokeThickness = 5;
            photoBorder.Stroke = Colors.White;

            screenSizeChanged();
            ScreenSizeMonitor.Instance.ScreenSizeChanged += delegate {
                screenSizeChanged();
            };
            FreezeBehavior freezeVE = new();
            this.Behaviors.Add(freezeVE);
            freezeVE.freezeView();
        }
        public void screenSizeChanged() {
            if (ScreenSizeMonitor.Instance.screenSize != this.screenSize) {
                this.screenSize = ScreenSizeMonitor.Instance.screenSize;
                configurePositions();
            }
        }
        public void configurePositions() {

            if (screenSize.Width > 0) {
                double photoSize = screenSize.Width * 0.18;
                photoBorder.WidthRequest = photoSize;
                photoBorder.HeightRequest = photoSize;
                photo.HeightRequest = photoSize;
                photo.WidthRequest = photoSize;

                this.WidthRequest = screenSize.Width;
                this.HeightRequest = Random.Shared.Next(40) + 80;

                priceLabel.TranslationX = screenSize.Width *0.75;
                bodyLabel.TranslationX = photoSize + 10;
                titleLabel.TranslationX = photoSize + 10;
                bodyLabel.TranslationY = 20 + Random.Shared.Next(5);
            }
            
        }
        bool photoLoaded = false;
        public void updateData() {
            priceLabel.Text = "$" + Random.Shared.Next(10000);
            bodyLabel.Text = aboutProduct[Random.Shared.Next(aboutProduct.Count())];
            titleLabel.Text = titleList[Random.Shared.Next(titleList.Count())];

            if (!photoLoaded) {
                photo.Source = photoList[Random.Shared.Next(photoList.Count())];
                photoLoaded = true;
            }
            configurePositions();

            //Debug.WriteLine("UPDATED DATA " + Random.Shared.Next(10000));
        }
    }
}
