using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SmoothScrollBug {
    public class DebugLabel {
        public Label label;
        public DateTime lastEditTime;
        public Point lastPosition;

        public DebugLabel(AbsoluteLayout parent) {
            label = new();
            parent.Add(label);
        }

    }
    public class DebugWindow : AbsoluteLayout {
        //===================
        //MAKE LAZY SINGLETON
        //===================
        public static DebugWindow Instance { get { return lazy.Value; } }
        private static readonly Lazy<DebugWindow> lazy = new Lazy<DebugWindow>(() => new DebugWindow());


        private List<DebugLabel> labels = new();
        double lineSpacing = 40;
        int numLabels = 8;
        int currentIndex = 0;
        Size screenSize = new Size(-1, -1);
        private DebugWindow() {
            for (int i = 0; i < numLabels; i++) {
                DebugLabel label = new DebugLabel(this);
                labels.Add(label);
            }
            clearLabels();

        }
        public void clearLabels() {
            for (int i = 0; i < labels.Count; i++) {
                labels[i].label.Text = "";
                labels[i].lastPosition = new Point(0, 0);
                labels[i].lastEditTime = DateTime.Now;
                labels[i].label.BackgroundColor = Colors.Yellow;
            }
        }
        public void addMessage(string msg) {

            //need oldest at top (descending)
            labels.Sort((x, y) => x.lastEditTime.CompareTo(y.lastEditTime)); //https://stackoverflow.com/questions/3309188/how-to-sort-a-listt-by-a-property-in-the-object
            labels[0].label.Text = msg;
            labels[0].lastEditTime = DateTime.Now;

            //can move to subscribe to screen size monitor change
            if (screenSize != ScreenSizeMonitor.Instance.screenSize) {
                screenSize = ScreenSizeMonitor.Instance.screenSize;
                this.HeightRequest = screenSize.Height;
                this.WidthRequest = screenSize.Width;
            }

            //align to bottom upwards
            double yPos = screenSize.Height - lineSpacing;
            for (int i = 0; i < labels.Count; i++) {
                labels[i].label.TranslationY = yPos - i * lineSpacing;
            }

            for (int i = 0; i < labels.Count; i++) {
                //Debug.WriteLine("INDEX: " + i + " " + labels[i].lastEditTime + " " + labels[i].label.Text + " Y " + labels[i].label.TranslationY);
            }


            //increment index
            currentIndex++;
            if (currentIndex == numLabels) {
                currentIndex = 0;
            }
        }
    }
}
