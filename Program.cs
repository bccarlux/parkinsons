using System;
using System.Media;
using System.IO;

namespace Program
{
    class Program
    {
        //Start of program

        static void Main(string[] args)
        {
            //declarations

            ConsoleKeyInfo cki;
            ConsoleKeyInfo ckans;
            int npress = 50;
            int ninterval = npress - 1;

            double[] secsarray = new double[npress];
            double[] intervalarray = new double[npress - 1];
            double[] diffmean = new double[npress - 1];
            double[] diffmeansq = new double[npress - 1];
            DateTime Start = DateTime.Now;
            DateTime dateOnly = Start.Date;
            int houronly = Start.Hour;
            int minuteonly = Start.Minute;
            int pillnumber = 0;
            double pilltime = 0;
            string userInput;

            //Input pill data

            Console.Write("Have you taken a pill since last test?");
            ckans = Console.ReadKey();
            Console.WriteLine();
            if (ckans.Key.ToString() == "Y")
            {
                Console.Write("Input which number of today's pills, 1-5:  ");
                userInput = Console.ReadLine();
                pillnumber = Convert.ToInt32(userInput);
                Console.WriteLine("You entered {0}", pillnumber);
                Console.Write("Input time of pill, hh.mm:   ");
                userInput = Console.ReadLine();
                pilltime = Convert.ToDouble(userInput);
                Console.WriteLine("You entered {0}", pilltime);
            }
            Console.WriteLine("Start test");

            // Loop to recognise and time pressed keys

            Console.TreatControlCAsInput = true;
            for (int i = 0; i < npress; i = i + 1)
            {

                //First key ("z")

                againz:
                cki = Console.ReadKey();
                DateTime date1 = DateTime.Now;
                double elapsedTicks = (date1.Ticks - Start.Ticks);
                double secs = elapsedTicks / 10000000;
                secsarray[i] = secs;
                string Val = cki.Key.ToString();
                if ((cki.Modifiers & ConsoleModifiers.Alt) == 0)
                {
                    SystemSounds.Question.Play();
                    goto againz;
                }
                if (Val != "Q")
                {
                    SystemSounds.Question.Play();
                    goto againz;
                }
                else
                {

                    //Second key ("x")

                    i = i + 1;
                    if (i == npress)
                    {
                        goto results;
                    }
                    againx:
                    cki = Console.ReadKey();
                    date1 = DateTime.Now;
                    elapsedTicks = (date1.Ticks - Start.Ticks);
                    secs = elapsedTicks / 10000000;
                    secsarray[i] = secs;
                    Val = cki.Key.ToString();
                    if ((cki.Modifiers & ConsoleModifiers.Alt) == 0)
                    {
                        SystemSounds.Question.Play();
                        goto againx;
                    }
                    if (Val != "W")
                    {
                        SystemSounds.Question.Play();
                        goto againx;
                    }
                }
            }

            //Calculate intervals, mean and sd

            results:
            double sum = 0;
            for (int i = 0; i < ninterval; i = i + 1)
            {
                intervalarray[i] = secsarray[i + 1] - secsarray[i];
                sum = sum + intervalarray[i];
            }
            double mean = sum / ninterval;
            double sumdiffmeansq = 0;
            for (int i = 0; i < ninterval; i = i + 1)
            {
                diffmean[i] = (intervalarray[i] - mean);
                diffmeansq[i] = Math.Pow(diffmean[i], 2);
                sumdiffmeansq = sumdiffmeansq + diffmeansq[i];

                //Write results to console

                Console.WriteLine(intervalarray[i]);
            }
            double stdev = (Math.Pow(sumdiffmeansq / ninterval, 0.5));
            Console.WriteLine("Mean is   " + mean);
            Console.WriteLine("SD is   " + stdev);

            //Sound end

            SystemSounds.Exclamation.Play();

            //option to discard results

            retry:
            Console.Write("Do you want to save this run?  y/n  ");
            ckans = Console.ReadKey();
            Console.WriteLine();
            if (ckans.Key.ToString() != "Y" & ckans.Key.ToString() != "N")
            {
                goto retry;
            }
                if (ckans.Key.ToString() == "Y")
            {
                //Write results to csv file

                using (StreamWriter outfile = new StreamWriter(@"c:\temp\Analysis.csv", true))
                {
                    outfile.Write("\n");
                    outfile.Write("{0},", dateOnly.ToString("d"));
                    outfile.Write("{0},", houronly);
                    outfile.Write("{0},", minuteonly);
                    outfile.Write("{0},", ninterval);
                    outfile.Write("{0},", mean);
                    outfile.Write("{0},", stdev);
                    outfile.Write("{0},", pillnumber);
                    outfile.Write("{0},", pilltime);

                    for (int i = 0; i < ninterval; i = i + 1)
                    {
                        outfile.Write("{0},", intervalarray[i]);
                    }

                    //outfile.Close();
                }
            }
            //Wait for enter to close console

            Console.WriteLine("Press Enter to terminate...");
            Console.Read();
        }
    }
}