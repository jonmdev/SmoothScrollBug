using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothScrollBug {
    public class TestPage: ContentPage {

        //private static readonly Lazy<TestPage> lazy = new Lazy<TestPage>(() => new TestPage());
        //public static TestPage Instance { get { return lazy.Value; } }

        List<CycleVE> cycleVE = new();
        AbsoluteLayout abs;
        public TestPage() {
            this.BackgroundColor = Colors.AliceBlue;

            ScreenSizeMonitor.Instance.ScreenSizeChanged += delegate {
                screenSizeChanged();
            };
            AbsoluteLayout rootDummy = new(); //needed due to glitch in root absolute not resizing properly last I checked
            this.Content = rootDummy;
            abs = new();
            rootDummy.Add(abs);

            rootDummy.Add(DebugWindow.Instance);

            var timer =Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1 / 120f);
            timer.Tick += delegate {
                runScroll();
            };
            timer.Start();
        }
        public void screenSizeChanged() {
            abs.HeightRequest = this.Height;
            abs.WidthRequest = this.Width;
            addElements();
        }
        DateTime lastUpdate = DateTime.Now;
        double scrollSpeed = 600;
        double amtMoved = 0;
        float defaultSpacing = 100;

        public void runScroll() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            double deltaTime = (DateTime.Now - lastUpdate).TotalSeconds;
            lastUpdate = DateTime.Now;

            //alternate scroll speed by seconds
            double realScrollSpeed = ScreenSizeMonitor.Instance.screenSize.Height / 2;
            //realScrollSpeed = 0;
            if (DateTime.Now.Second % 6 == 0) {
                realScrollSpeed = 0;
            }
            double amtToMove = deltaTime * realScrollSpeed;
            amtMoved += amtToMove;

            //increment units
            for (int i=0; i <cycleVE.Count; i++) {
                cycleVE[i].TranslationY += amtToMove;
            }

            //check for looping
            if (amtMoved > 100) {
                for (int i=0; i <cycleVE.Count; i++) {
                    if (cycleVE[i].TranslationY> ScreenSizeMonitor.Instance.screenSize.Height) {
                        cycleVE[i].TranslationY -= defaultSpacing * cycleVE.Count;
                        cycleVE[i].updateData();
                    }
                }
            }
            stopwatch.Stop();
            Debug.WriteLine("DURATION OF UPDATE " + stopwatch.Elapsed.Milliseconds + " ms");
            Debug.WriteLine("DURATION OF UPDATE " + stopwatch.Elapsed.Ticks + " ticks");
            Debug.WriteLine("FPS OF UPDATE " + 1/stopwatch.Elapsed.TotalSeconds + " fps");
            DebugWindow.Instance.addMessage("FPS OF UPDATE " + Math.Round(1 / stopwatch.Elapsed.TotalSeconds) + " fps");
        }
        public void addElements() {

            int numElementsNeeded = (int)(ScreenSizeMonitor.Instance.screenSize.Height / defaultSpacing) + 2;
            int numToAdd = numElementsNeeded - cycleVE.Count;
            for (int i = 0; i< numToAdd; i++) {
                CycleVE newVE = new();
                abs.Add(newVE);
                newVE.updateData();
                cycleVE.Add(newVE);
            }
            for (int i=0; i < cycleVE.Count; i++) {
                cycleVE[i].TranslationY = i * defaultSpacing;
            }
            Debug.WriteLine("NUM ELEMENTS " +  cycleVE.Count + " height " + ScreenSizeMonitor.Instance.screenSize.Height);
        }

    }
}
