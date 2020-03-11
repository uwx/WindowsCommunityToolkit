//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
//See LICENSE in the project root for license information.

using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction { /*

namespace GazeHidParsers {
    static HidNumericControlDescription  GetGazeUsageFromCollectionId(
        GazeDevicePreview  gazeDevice,
        uint16 childUsageId,
        uint16 parentUsageId)
    {
        IVectorView<HidNumericControlDescription >  numericControls = gazeDevice.GetNumericControlDescriptions(
            (USHORT)GazeHidUsages.UsagePage_EyeHeadTracker, childUsageId);

        for (unsigned int i = 0; i < numericControls.Size; i++)
        {
            var parentCollections = numericControls.GetAt(i).ParentCollections;
            if (parentCollections.Size > 0 &&
                parentCollections.GetAt(0).UsagePage == (USHORT)GazeHidUsages.UsagePage_EyeHeadTracker &&
                parentCollections.GetAt(0).UsageId == parentUsageId)
            {
                return numericControls.GetAt(i);
            }
        }
        return null;
    }

#pragma region GazeHidPositionParser
    GazeHidPositionParser.GazeHidPositionParser(GazeDevicePreview  gazeDevice, uint16 usage)
    {
        _usage = usage;

        // Find all the position usages from the device's
        // descriptor and store them for easy access
        _X = GetGazeUsageFromCollectionId(gazeDevice, (USHORT)GazeHidUsages.Usage_PositionX, _usage);
        _Y = GetGazeUsageFromCollectionId(gazeDevice, (USHORT)GazeHidUsages.Usage_PositionY, _usage);
        _Z = GetGazeUsageFromCollectionId(gazeDevice, (USHORT)GazeHidUsages.Usage_PositionZ, _usage);
    }

    GazeHidPosition GazeHidPositionParser.GetPosition(HidInputReport  report)
    {
        GazeHidPosition result = null;

        if (_X != null &&
            _Y != null &&
            _Z != null &&
            _usage != 0x0000)
        {
            var descX = report.GetNumericControlByDescription(_X);
            var descY = report.GetNumericControlByDescription(_Y);
            var descZ = report.GetNumericControlByDescription(_Z);

            var controlDescX = descX.ControlDescription;
            var controlDescY = descY.ControlDescription;
            var controlDescZ = descZ.ControlDescription;

            if ((controlDescX.LogicalMaximum < descX.ScaledValue || controlDescX.LogicalMinimum > descX.ScaledValue) ||
                (controlDescY.LogicalMaximum < descY.ScaledValue || controlDescY.LogicalMinimum > descY.ScaledValue) ||
                (controlDescZ.LogicalMaximum < descZ.ScaledValue || controlDescZ.LogicalMinimum > descZ.ScaledValue))
            {
                // One of the values is outside of the valid range.
            }
            else
            {
                result = new GazeHidPosition();
                result.X = descX.ScaledValue;
                result.Y = descY.ScaledValue;
                result.Z = descZ.ScaledValue;
            }
        }

        return result;
    }
#pragma endregion GazeHidPositionParser

#pragma region GazeHidRotationParser
    GazeHidRotationParser.GazeHidRotationParser(GazeDevicePreview  gazeDevice, uint16 usage)
    {
        _usage = usage;

        // Find all the rotation usages from the device's
        // descriptor and store them for easy access
        _X = GetGazeUsageFromCollectionId(gazeDevice, (USHORT)GazeHidUsages.Usage_RotationX, _usage);
        _Y = GetGazeUsageFromCollectionId(gazeDevice, (USHORT)GazeHidUsages.Usage_RotationY, _usage);
        _Z = GetGazeUsageFromCollectionId(gazeDevice, (USHORT)GazeHidUsages.Usage_RotationZ, _usage);
    }

    GazeHidPosition GazeHidRotationParser.GetRotation(HidInputReport  report)
    {
        GazeHidPosition result = null;

        if (_X != null &&
            _Y != null &&
            _Z != null &&
            _usage != 0x0000)
        {
            var descX = report.GetNumericControlByDescription(_X);
            var descY = report.GetNumericControlByDescription(_Y);
            var descZ = report.GetNumericControlByDescription(_Z);

            var controlDescX = descX.ControlDescription;
            var controlDescY = descY.ControlDescription;
            var controlDescZ = descZ.ControlDescription;

            if ((controlDescX.LogicalMaximum < descX.ScaledValue || controlDescX.LogicalMinimum > descX.ScaledValue) ||
                (controlDescY.LogicalMaximum < descY.ScaledValue || controlDescY.LogicalMinimum > descY.ScaledValue) ||
                (controlDescZ.LogicalMaximum < descZ.ScaledValue || controlDescZ.LogicalMinimum > descZ.ScaledValue))
            {
                // One of the values is outside of the valid range.
            }
            else
            {
                result = new GazeHidPosition();
                result.X = descX.ScaledValue;
                result.Y = descY.ScaledValue;
                result.Z = descZ.ScaledValue;
            }
        }

        return result;
    }
#pragma endregion GazeHidRotationParser

#pragma region GazeHidPositionsParser
    GazeHidPositionsParser.GazeHidPositionsParser(GazeDevicePreview  gazeDevice)
    {
        _leftEyePositionParser  = new GazeHidPositionParser(gazeDevice, (USHORT)GazeHidUsages.Usage_LeftEyePosition);
        _rightEyePositionParser = new GazeHidPositionParser(gazeDevice, (USHORT)GazeHidUsages.Usage_RightEyePosition);
        _headPositionParser     = new GazeHidPositionParser(gazeDevice, (USHORT)GazeHidUsages.Usage_HeadPosition);
        _headRotationParser     = new GazeHidRotationParser(gazeDevice, (USHORT)GazeHidUsages.Usage_HeadDirectionPoint);
    }

    GazeHidPositions  GazeHidPositionsParser.GetGazeHidPositions(HidInputReport  report)
    {
        var retval = new GazeHidPositions();

        retval.LeftEyePosition  = _leftEyePositionParser.GetPosition(report);
        retval.RightEyePosition = _rightEyePositionParser.GetPosition(report);
        retval.HeadPosition     = _headPositionParser.GetPosition(report);
        retval.HeadRotation     = _headRotationParser.GetRotation(report);

        return retval;
    }
#pragma endregion GazeHidPositionsParser
}

*/ }
