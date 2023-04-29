using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS_G1_L1.Model
{
    public class ProgressModel
    {
        #region Attributes
        //Attributes

        private double _pourcent;
        private double _max;
        private double _actualNumber;
        private string _currentInformation;
        private string _titleOfProgress;
        private bool _defaultProgress = false;

        /// <summary>
        /// Sets or gets the pourcents
        /// </summary>
        public double Pourcent { get { return _pourcent; } set { _pourcent = value; } }

        /// <summary>
        /// Sets of gets the max
        /// </summary>
        public double Max { get { return _max; } set { _max = value; } }

        /// <summary>
        /// Sets or gets the min
        /// </summary>
        public double ActualNumber { get { return _actualNumber; } set { _actualNumber = value; } }

        /// <summary>
        /// Sets or gets the information to add at the end of the progress bar
        /// </summary>
        public string Information { get { return _currentInformation; } set { _currentInformation = value; } }

        /// <summary>
        /// Sets or gets the title that should be added above the progress bar
        /// </summary>
        public string TitleOfPregress { get { return _titleOfProgress; } set { _titleOfProgress = value; } }

        #endregion
        #region Constructors
        //Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProgressModel()
        {
            ActualNumber = 0f;
            Max = 0f;
            Pourcent = 0f;
        }

        #endregion
        #region Methods
        //Methods

        public void DisplayProgression()
        {
            PrintTitleOfProgress();
            Console.CursorVisible = false;
            Console.SetCursorPosition(7, 2);

            Console.SetCursorPosition(2, 4);
            Console.WriteLine(Information);

            int maxBarSize = 100;

            if (Pourcent > maxBarSize) Pourcent = maxBarSize;

            if (!_defaultProgress)
            {
                for (int i = 0; i < 100; i++)
                {
                    //Draw the progress bar
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write('\u2588');
                    Console.SetCursorPosition(2, 2);

                    //Writing the pourcent
                    Console.Write("0%");
                    Console.SetCursorPosition(7 + i, 2);
                }
                Console.ResetColor();
                _defaultProgress = !_defaultProgress;
            }

            Console.SetCursorPosition(7, 2);

            for (int i = 0; i < Pourcent; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                //System.Threading.Thread.Sleep(10);
                //Draw the progress bar
                Console.Write('\u2588');
                Console.SetCursorPosition(2, 2);

                //Writing the pourcent
                Console.Write($"{Pourcent.ToString("0")}%");
                Console.SetCursorPosition(7 + i, 2);

                if (Pourcent > 100 || i >= 101)
                    break;
            }

            if (Pourcent >= Max)
                PrintEndOfProgress();

            Console.CursorVisible = true;

            Console.ResetColor();
        }

        private void PrintTitleOfProgress()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write($"Tâche en cours : {TitleOfPregress}");
        }

        public void PrintEndOfProgress()
        {
            Console.SetCursorPosition(0, 6);
            Console.Write($"Tâche : {TitleOfPregress} done !");
        }

        public void Progression()
        {
            DisplayProgression();
        }

        public double CalculatePourcent()
        {
            Pourcent = ActualNumber * 100 / Max;
            return Pourcent;
        }

        public void ResetProgression()
        {
            ActualNumber = 0;
            Pourcent = 0;
            Max = 0;
            TitleOfPregress = "";
            Information = "";
        }

        #endregion
    }
}
