using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Bara_Tehni.Methods
{
    class AnimationsEffects
    {
        //Data variable for main window
        
        public AnimationsEffects()
        {
         
        }
        public static void Scale_Effect(UIElement controlName, double InitscaleSize, double finalscaleSize, int milliSec)
        {
            var oTransform = new ScaleTransform();
            DoubleAnimation anim = null;

            anim = new DoubleAnimation(InitscaleSize, finalscaleSize, TimeSpan.FromMilliseconds(milliSec));

            controlName.RenderTransform = oTransform;
            oTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            oTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
        #region Fade in
        public static Storyboard FadeIn_Effect(string controlName)
        {
            // Create a storyboard to contain the animations.
            var storyboard = new Storyboard();
            var duration = new TimeSpan(0, 0, 1);
            // Create a DoubleAnimation to fade the not selected option control
            var animation = new DoubleAnimation {From = 0.0, To = 1.0, Duration = new Duration(duration)};

            // Configure the animation to target de property Opacity
            Storyboard.SetTargetName(animation, controlName);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard
            return storyboard;
        }
        #endregion
        public Storyboard FadeIn_Effect(IEnumerable<string> controlNames, string style)
        {
            // Create a storyboard to contain the animations.
            var speed = 1.0;
            var storyboard = new Storyboard();

            // Create a DoubleAnimation to fade the not selected option control
            foreach (var str in controlNames)
            {
                var duration = new TimeSpan(0, 0, (int)speed);
                var animation = new DoubleAnimation {From = 0.0, To = 1.0};

                if (style == "one")
                {
                    speed += 1.0;
                }
                animation.Duration = new Duration(duration);
                // Configure the animation to target de property Opacity
                Storyboard.SetTargetName(animation, str);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
                // Add the animation to the storyboard
                storyboard.Children.Add(animation);
            }
            // Begin the storyboard
            return storyboard;
        }
        #region Type Effect
        public void Typerwriter_Effect(Label controlName)
        {
            var iter = 0;
            System.Windows.Forms.Timer timer = null;
            var temp = controlName.Content.ToString();
            controlName.Content = "";
            var cControl = controlName;
            timer = new System.Windows.Forms.Timer {Interval = 120};
            timer.Tick += (s, args) => timer_Tick(controlName, temp, ref iter, timer);
            timer.Start();
        }

        public void Rotate_Effect(Control controlName, double angle)
        {
            var oLabelAngleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(10)),
                RepeatBehavior = new RepeatBehavior(4)
            };

            var oTransform = new RotateTransform();
            
            controlName.RenderTransform = oTransform;
            oTransform.BeginAnimation(RotateTransform.AngleProperty,  oLabelAngleAnimation);
        }

        public void Scale_Effect(Control controlName, double scaleSize,int milliSec,string type)
        {
            var oTransform = new ScaleTransform();
            DoubleAnimation anim = null;
            if( type.ToLower() == "in" )
                anim = new DoubleAnimation(1, scaleSize, TimeSpan.FromMilliseconds(milliSec));
            else //out
                anim = new DoubleAnimation(scaleSize, 1, TimeSpan.FromMilliseconds(milliSec));
            controlName.RenderTransform = oTransform;
            oTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            oTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
        public void Scale_Effect(Ellipse controlName, double scaleSize, int milliSec, string type)
        {
            var oTransform = new ScaleTransform();
            DoubleAnimation anim = null;
            anim = type.ToLower() == "in" ? new DoubleAnimation(1, scaleSize, TimeSpan.FromMilliseconds(milliSec)) : new DoubleAnimation(scaleSize, 1, TimeSpan.FromMilliseconds(milliSec));
            controlName.RenderTransform = oTransform;
            oTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            oTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        static void timer_Tick(ContentControl cControl, string temp, ref int iter, System.Windows.Forms.Timer timer)
        {
            if (temp == null || timer == null) return;
            cControl.Content += temp[iter].ToString();
            iter++;
            if (cControl.Content.ToString().Length == temp.Length)
            {
                timer.Stop();
            }
        }

        #endregion
    }
}
