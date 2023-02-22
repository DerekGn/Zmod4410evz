/*
* MIT License
*
* Copyright (c) 2023 Derek Goslin https://github.com/DerekGn
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using McMaster.Extensions.CommandLineUtils;
using System.Runtime.InteropServices;
using Zmod4410evz.Interop;
using Zmod4410evz.Sensor;

namespace Zmod4410evz.Commands
{
    [Command(Description = "Execute Iaq sampling")]
    [Subcommand(typeof(IaqUlpCommand))]
    internal class IaqCommand : BaseCommand
    {
        private const int Interval = 3000;

        public IaqCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            return ExecuteCommand((sensor) =>
            {
                IaqHandle algoHandle = new()
                {
                    LogRcda = new float[9]
                };

                IaqResults algoResults = new()
                {
                    Rmox = new float[13]
                };

                IaqInputs algoInput = new()
                {
                    HumidityPct = 50.0f,
                    TemperatureDegc = 20.0f
                };

                var result = Iaq.Init(ref algoHandle);

                if (result != IaqError.OK)
                {
                    console.Error.WriteLine("Library Initialise failed");
                }
                else
                {
                    console.WriteLine($"Starting Measurement Loop. Sensor Read every [{Interval / 1000}] Seconds. Press any key to exit");

                    do
                    {
                        sensor.StartMeasurement();

                        Thread.Sleep(Interval);

                        var status = sensor.GetStatus();

                        ErrorEvent errorEvent;

                        if ((status & Zmod4410Status.SequencerRunningMask) == Zmod4410Status.SequencerRunningMask)
                        {
                            console.WriteLine("Sequencer still running");

                            errorEvent = sensor.GetErrorEvent();
#warning TODO handle error
                            switch (errorEvent)
                            {
                                case ErrorEvent.None:
                                    break;
                                case ErrorEvent.PowerOn:
                                    break;
                                case ErrorEvent.AccessConflict:
                                    break;
                                default:
                                    break;
                            }
                        }

                        console.WriteLine("Reading Adc");

                        var adc = sensor.ReadAdc();

                        algoInput.AssignAdcResult(adc.ToArray());

#warning TODO handle error
                        //errorEvent = sensor.GetErrorEvent();

                        Zmod4xxxDevice device = sensor.ToMarshalType();                        

                        result = Iaq.Calc(ref algoHandle, ref device, ref algoInput, ref algoResults);

                        console.WriteLine("*********** Measurements ***********");
                        for (int i = 0; i < 13; i++)
                        {
                            console.WriteLine($" Rmox[{i}] = {algoResults.Rmox[i] / 1e3} kOhm", i);
                        }
                        console.WriteLine($" Rcda = {(Math.Pow(10, algoResults.LogRcda) / 1e3)} kOhm");
                        console.WriteLine($" EtOH = {algoResults.Etoh} ppm");
                        console.WriteLine($" TVOC = {algoResults.Tvoc} mg/m^3");
                        console.WriteLine($" eCO2 = {algoResults.Eco2} ppm");
                        console.WriteLine($" IAQ  = {algoResults.Iaq}");

                        /* Check validity of the algorithm results. */
                        switch (result)
                        {
                            case IaqError.Stabilization:
                                console.WriteLine("Warm-Up!");
                                break;
                            case IaqError.OK:
                                console.WriteLine("Valid!");
                                break;
                            case IaqError.Damage:
                                console.WriteLine("Error: Sensor probably damaged. Algorithm results may be incorrect.");
                                break;
                            default:
                                console.WriteLine("Unexpected Error during algorithm calculation: Exiting Program.");
                                break;
                        }

                    } while (!Console.KeyAvailable);
                }

                return 0;
            });
        }
    }
}
