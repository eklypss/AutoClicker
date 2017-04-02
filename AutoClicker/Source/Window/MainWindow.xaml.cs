using AutoClicker.Source.Enum;
using AutoClicker.Source.Mouse;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AutoClicker.Source.Mouse.MouseControl;

namespace AutoClicker
{
    public partial class MainWindow : Window
    {
        /// <summary>
        ///  Private variables
        /// </summary>
        private ProgramStatus status;
        private MouseEventFlags[] clickType = new MouseEventFlags[2];
        private MousePoint clickPoint;
        private int timesRan;
        private Timer clickTimer;

        /// <summary>
        /// Window/program startup code
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            status = ProgramStatus.None;
            clickType[0] = MouseEventFlags.LeftDown;
            clickType[1] = MouseEventFlags.LeftUp;

            // Timer for updating mouse position label on UI since WPF doesn't have native event for this.
            Timer timer = new Timer()
            {
                Enabled = true,
                Interval = 100
            };

            timer.Elapsed += (sender, e) =>
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    MousePositionLabel.Content = string.Format("Mouse position: {0}, {1}", MouseControl.GetCursorPosition().X, MouseControl.GetCursorPosition().Y);
                }));
            };
        }

        /// <summary>
        /// Runs when X coordinate TextBox is changed and changes the border if given coordinate is valid.
        /// </summary>
        private void XCoordinateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int tempNumber;
            bool isNumeric = Int32.TryParse(XCoordinateTextBox.Text, out tempNumber);
            if (isNumeric)
            {
                this.XCoordinateTextBox.BorderBrush = Brushes.LimeGreen;
                if (IsNumeric(YCoordinateTextBox.Text) && status != ProgramStatus.Running)
                {
                    StartButton.IsEnabled = true;
                }
            }
            else
            {
                this.XCoordinateTextBox.BorderBrush = Brushes.Firebrick;
                StartButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Runs when Y coordinate TextBox is changed and changes the border if given coordinate is valid.
        /// </summary>
        private void YCoordinateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsNumeric(YCoordinateTextBox.Text))
            {
                this.YCoordinateTextBox.BorderBrush = Brushes.LimeGreen;
                if (IsNumeric(XCoordinateTextBox.Text) && status != ProgramStatus.Running)
                {
                    StartButton.IsEnabled = true;
                }
            }
            else
            {
                this.YCoordinateTextBox.BorderBrush = Brushes.Firebrick;
                StartButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Checks whether text is numeric only and returns result as boolean.
        /// </summary>
        public bool IsNumeric(string text)
        {
            int tempNumber;
            return Int32.TryParse(text, out tempNumber);
        }

        /// <summary>
        /// Function to start clicking. Creates timer that runs on given interval.
        /// </summary>
        private void StartClicking()
        {
            status = ProgramStatus.Running;
            timesRan = 0;
            clickPoint = new MousePoint(Int32.Parse(XCoordinateTextBox.Text), Int32.Parse(YCoordinateTextBox.Text));

            clickTimer = new Timer()
            {
                Enabled = true,
                Interval = Int32.Parse(IntervalTextBox.Text)
            };
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            clickTimer.Elapsed += ClickTimer_Elapsed;
        }

        /// <summary>
        /// Stops clicking and disposes clickTimer object.
        /// </summary>
        private void StopClicking()
        {
            status = ProgramStatus.Stopped;
            clickTimer.Dispose();
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
        }

        /// <summary>
        /// Keyboard shortcut events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                XCoordinateTextBox.Text = MouseControl.GetCursorPosition().X.ToString();
                YCoordinateTextBox.Text = MouseControl.GetCursorPosition().Y.ToString();
            }
            else if (e.Key == Key.F2)
            {
                if (status == ProgramStatus.Running)
                {
                    StopClicking();
                }
                else
                {
                    StartClicking();
                }
            }
        }

        /// <summary>
        /// StartButton interaction code
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartClicking();
        }

        /// <summary>
        /// StopButton interaction code
        /// </summary>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopClicking();
        }

        /// <summary>
        /// Main clicking function that gets called called X times every X milliseconds as defined by the user, and once done stops.
        /// </summary>
        private void ClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Console.WriteLine(clickPoint.X + " " + clickPoint.Y);
                if (timesRan <= Int32.Parse(RepeatTextBox.Text))
                {
                    MouseControl.SetCursorPosition(clickPoint);
                    MouseControl.MouseEvent(clickType[0]);
                    MouseControl.MouseEvent(clickType[1]);
                    timesRan++;
                    Console.WriteLine(timesRan + "/" + Int32.Parse(RepeatTextBox.Text));
                }
                else
                {
                    status = ProgramStatus.None;
                    StartButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    clickTimer.Dispose();
                }
            }));
        }

        /// <summary>
        /// Interaction logic for changing mouse button used for clicking.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseButtonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MouseButtonComboBox.SelectedIndex)
            {
                case 0:
                {
                    clickType[0] = MouseEventFlags.LeftDown;
                    clickType[1] = MouseEventFlags.LeftUp;
                    break;
                }
                case 1:
                {
                    clickType[0] = MouseEventFlags.MiddleDown;
                    clickType[1] = MouseEventFlags.MiddleUp;
                    break;
                }
                case 2:
                {
                    clickType[0] = MouseEventFlags.RightDown;
                    clickType[1] = MouseEventFlags.RightUp;
                    break;
                }
                default:
                {
                    clickType[0] = MouseEventFlags.LeftDown;
                    clickType[1] = MouseEventFlags.LeftUp;
                    break;
                }
            }
        }
    }
}