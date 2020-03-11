//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Input.Preview;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction { /*

namespace GazeHidParsers {
    public class GazeHidPosition sealed
    {
    public:
        property long long X;
        property long long Y;
        property long long Z;
    };

    public class GazeHidPositions sealed
    {
    public:
        property GazeHidPosition LeftEyePosition;
        property GazeHidPosition RightEyePosition;
        property GazeHidPosition HeadPosition;
        property GazeHidPosition HeadRotation;
    };

    public class GazeHidPositionParser sealed
    {
    public:
        GazeHidPositionParser(GazeDevicePreview  gazeDevice, uint16 usage);

        GazeHidPosition GetPosition(HidInputReport  report);

    private:
        HidNumericControlDescription  _X = null;
        HidNumericControlDescription  _Y = null;
        HidNumericControlDescription  _Z = null;
        uint16 _usage                     = 0x0000;
    };

    public class GazeHidRotationParser sealed
    {
    public:
        GazeHidRotationParser(GazeDevicePreview  gazeDevice, uint16 usage);

        GazeHidPosition GetRotation(HidInputReport report);

    private:
        HidNumericControlDescription  _X = null;
        HidNumericControlDescription  _Y = null;
        HidNumericControlDescription  _Z = null;
        uint16 _usage                     = 0x0000;
    };

    public class GazeHidPositionsParser sealed
    {
    public:
        GazeHidPositionsParser(GazeDevicePreview  gazeDevice);

        GazeHidPositions GetGazeHidPositions(HidInputReport  report);

    private:
        GazeHidPositionParser  _leftEyePositionParser;
        GazeHidPositionParser  _rightEyePositionParser;
        GazeHidPositionParser  _headPositionParser;
        GazeHidRotationParser  _headRotationParser;
    };
}

*/ }
