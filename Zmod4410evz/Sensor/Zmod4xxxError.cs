namespace Zmod4410evz.Sensor
{
    public enum Zmod4xxxError
    {
        None = 0,
        /// <summary>
        /// The initialization value is out of range.
        /// </summary>
        ErrorInitOutOfRange = -1,
        /// <summary>
        ///  A previous measurement is running that could not be stopped or sensor does not respond.
        /// </summary>
        ErrorGasTimeout = -2,
        /// <summary>
        /// I2C communication was not successful.
        /// </summary>
        ErrorI2c = -3,
        /// <summary>
        /// The Firmware configuration used does not match the sensor module.
        /// </summary>
        ErrorSensorUnsupported = -4,
        /// <summary>
        /// There is no pointer to a valid configuration.
        /// </summary>
        ErrorConfigMissing = -5,
        /// <summary>
        /// Invalid ADC results due to a still running measurement while results readout.
        /// </summary>
        ErrorAccessConflict = -6,
        /// <summary>
        /// Power-on reset event. Check power supply and reset pin.
        /// </summary>
        ErrorPorEvent = -7,
        /// <summary>
        /// The maximum numbers of cleaning cycles ran on this sensor. Cleaning function has no effect anymore.
        /// </summary>
        ErrorCleaning = -8,
        /// <summary>
        /// The dev structure did not receive the pointers for I2C read, write and/or delay.
        /// </summary>
        ErrorNullPtr = -9
    }
}
