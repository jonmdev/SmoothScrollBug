using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmoothScrollBug {
    public class TestPage: ContentPage {

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

            //======================================================================
            //START TIMER 
            //======================================================================
            var timer = Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1 / 500f);
            timer.Tick += delegate {
                runScroll();
            };
            timer.Start();

        }

        //==================
        //SCROLL FUNCTION
        //==================
        DateTime lastUpdate = DateTime.Now;
        double amtMoved = 0;
        float defaultSpacing = 100;
        List<double> deltaTimeQueue = new List<double>();
        int numDeltaTimeToAverage = 20;
        int dbgUpdateTimer = 0;
        public void runScroll() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            double deltaTime = (DateTime.Now - lastUpdate).TotalSeconds;
            lastUpdate = DateTime.Now;

            //alternate scroll speed by increments of seconds
            double scrollSpeed = ScreenSizeMonitor.Instance.screenSize.Height / 2;
            if (Math.Floor(DateTime.Now.Second * 0.5) % 3 == 0) {
                scrollSpeed = 0;
            }
            double amtToMove = deltaTime * scrollSpeed;
            amtMoved += amtToMove;

            //increment units
            for (int i = 0; i < cycleVE.Count; i++) {
                cycleVE[i].TranslationY += amtToMove;
            }
            //cycleVE[0].TranslationY += 0.000001;

            //check for looping
            if (amtMoved > 100) {
                for (int i=0; i <cycleVE.Count; i++) {
                    if (cycleVE[i].TranslationY > ScreenSizeMonitor.Instance.screenSize.Height) {
                        cycleVE[i].TranslationY -= defaultSpacing * cycleVE.Count;
                        cycleVE[i].updateData();
                    }
                }
            }
            stopwatch.Stop();

            //store delta time to list for averaging
            if (deltaTimeQueue.Count >= numDeltaTimeToAverage) {
                deltaTimeQueue.RemoveAt(0);
            }
            deltaTimeQueue.Add(deltaTime);

            //solve average delta time;
            int denominator = 0;
            double numerator = 0;
            double maxDt = 0;
            double minDt = 1000;
            for (int i = 0; i < deltaTimeQueue.Count; i++) {
                numerator += deltaTimeQueue[i];
                denominator++;
                minDt = Math.Min(minDt, deltaTimeQueue[i]);
                maxDt = Math.Max(maxDt, deltaTimeQueue[i]);
            }
            double averageDeltaTime = numerator / denominator;


            string debugMsg = "FPS OF DELTA TIME + " + Math.Round(1 / averageDeltaTime) + "\n" + "MAX FPS " + Math.Round(1 / minDt) + " MIN FPS " + Math.Round(1 / maxDt);
            //Debug.WriteLine(debugMsg);

            //MUST ONLY UPDATE THIS PERIODICALLY OR CRUSHES PERFORMANCE:
            dbgUpdateTimer++;
            if (dbgUpdateTimer > 100) {
                dbgUpdateTimer = 0;
                //DebugWindow.Instance.addMessage(debugMsg);
                if (cycleVE.Count > 0) {
                    cycleVE[0].updateData();
                }
            }
        }

        //=======================
        //RESIZE FUNCTION
        //=======================
        public void screenSizeChanged() {
            abs.HeightRequest = this.Height;
            abs.WidthRequest = this.Width;
            addElements();
        }
        //ensure enough elements to fill screen
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
            //Debug.WriteLine("NUM ELEMENTS " +  cycleVE.Count + " height " + ScreenSizeMonitor.Instance.screenSize.Height);
        }



    }
}
