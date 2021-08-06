-- The local functions are scoped at the project level, so that we can keep
-- track of the reading buffers throughout the test.
reset()
smua.reset()
local systemNodes = {}
local systemSmus = {}
local systemSmuReadingBufferIndexes = {}
local appendMode = 1
local rangei = 1e-3
local doCap =0
local gOverrunDetected = false
-- ==========================================================================
-- This function tests for overruns in any of the system smus' trigger models.
-- It returns an error to the application that will be displayed in a
-- dialog box if an overrun occurs.
-- ==========================================================================
local _Overruncheck = function()
    -- ==========================================================================
    -- This function test the results of the value in the smu's trigger overrun
    -- status register. -- It returns an error if an overrun occurs.
    --
    -- parameter   overrun     A bit pattern that contains the smu trigger overrun
    --                         status register result
    -- ==========================================================================
    local TestOverrunBits = function(overrun, selNode, whichSmu)
        local whichNode = [[localnode.]]
        if (selNode.tsplink.node ~= localnode.tsplink.node) then
            whichNode = [[node[]] .. selNode.tsplink.node .. [[].]]
        end
        whichNode = whichNode .. whichSmu .. [[ (]] .. selNode.model .. [[)]]
        if (bit.test(overrun, 2)) then
            gOverrunDetected = true
            return ("arm overrun on " .. whichNode .. "{eol}")
        elseif (bit.test(overrun, 3)) then
            gOverrunDetected = true
            return ("source overrun on " .. whichNode .. "{eol}")
        elseif (bit.test(overrun, 4)) then
            gOverrunDetected = true
            return ("measure overrun on " .. whichNode .. "{eol}")
        elseif (bit.test(overrun, 5)) then
            gOverrunDetected = true
            return ("end pulse overrun on " .. whichNode .. "{eol}")
        else
            return ("")
        end
    end

    -- Report an overrun error only once.
    if (gOverrunDetected == false) then
        local allSmusInOverrun = [[]]

        for i, selNode in ipairs(systemNodes) do
            for j, selSmu in ipairs(systemSmus[i]) do
                if (j == 1) then
                    local overrun = selNode.status.operation.instrument.smua.trigger_overrun.event
                    if (overrun > 0) then
                        allSmusInOverrun = allSmusInOverrun .. TestOverrunBits(overrun, selNode, [[smua]])
                    end
                elseif (j == 2) then
                    local overrun = selNode.status.operation.instrument.smub.trigger_overrun.event
                    if (overrun > 0) then
                        allSmusInOverrun = allSmusInOverrun .. TestOverrunBits(overrun, selNode, [[smub]])
                    end
                end
            end
        end

        if (gOverrunDetected == true) then
            print("[{error}]Script stopped due to:{eol}" .. allSmusInOverrun)
        end
    end
end

local gComplianceDetected = false
-- ==========================================================================
-- Checks smus for voltage or current compliance.
-- If a compliance event occurs, the message dialog box is presented to the
-- user.
-- ==========================================================================
local _ComplianceCheck = function()
    -- Report a compliance error only once.
    if (gComplianceDetected == false) then
        local allSmusInCompliance = [[]]

        for i, selNode in ipairs(systemNodes) do
            for j, selSmu in ipairs(systemSmus[i]) do
                local compDetect = selNode.status.measurement.instrument.event
                if (compDetect > 0) then
                    -- Compose and show error message
                    local whichNode = [[localnode.]]
                    if (selNode.tsplink.node ~= localnode.tsplink.node) then
                        whichNode = [[node[]] .. selNode.tsplink.node .. [[].]]
                    end
                    local whichSmu = [[]]
                    if (compDetect == 2) then
                        whichSmu = whichNode .. [[smua]]
                    elseif (compDetect == 4) then
                        whichSmu = whichNode .. [[smub]]
                    elseif (compDetect == 6) then
                        whichSmu = whichNode .. [[smua and ]] .. whichNode .. [[smub]]
                    end
                    whichSmu = whichSmu .. [[ (]] .. selNode.model .. [[){eol}]]
                    allSmusInCompliance = allSmusInCompliance .. whichSmu

                    -- Set gComplianceDetected so we don't report the error again.
                    gComplianceDetected = true
                end
            end
        end

        if (gComplianceDetected == true) then
            print("[{info}]Compliance detected on:{eol}" .. allSmusInCompliance ..
                      "{eol}Click the Advanced button and check the Source Limit setting.")
        end
    end
end

local gTestAborted = false
-- ==========================================================================
-- This function waits up to a specified period for the sweeps to complete.
-- If the delay parameter is -1, then the timeout is infinite. It returns true
-- if all sweeping actions are complete. Otherwise, the return value is false.
-- Note: If gTestAborted is true, this function returns false immediately.
--
-- parameter  interval     Maximum time to wait (in seconds).
-- ==========================================================================
local _WaitForComplete = function(interval)
    local pollInterval = 0.1

    -- ==========================================================================
    -- Checks the system smus for any that are still sweeping.
    -- ==========================================================================
    local IsSweepingComplete = function()
        for i, selNode in ipairs(systemNodes) do
            for j, selSmu in ipairs(systemSmus[i]) do
                if (j == 1) then
                    local statcond = selNode.status.operation.instrument.smua.condition
                    if (bit.test(statcond, 4)) then
                        return false
                    end
                elseif (j == 2) then
                    local statcond = selNode.status.operation.instrument.smub.condition
                    if (bit.test(statcond, 4)) then
                        return false
                    end
                end
            end
        end

        return true
    end

    local notimeout = false
    if (interval < 0) then
        notimeout = true
        interval = 1
    end

    while (interval > 0) do
        -- Check the abort flag
        if (gTestAborted == true) then
            return false
        end

        -- Check for errors
        if (errorqueue.count > 0) then
            return false
        end

        -- Check for overruns and compliance
        _Overruncheck()
        _ComplianceCheck()

        if (IsSweepingComplete() == true) then
            return true
        end

        delay(pollInterval)

        if (notimeout == false) then
            interval = interval - pollInterval
        end
    end

    if (IsSweepingComplete() == true) then
        return true
    end

    return false
end
----------------------------------------------------------------------------
-- START OF INITIALIZE SEGMENT ... do not modify this section
----------------------------------------------------------------------------
-- ==========================================================================
-- This function prepares the test for execution. 
-- It first verifies that current setup matches project's setup. 
-- Then, it initializes members used to keep track of reading buffer storage.
-- ==========================================================================
function _Initialize()
    local maxNodes = 64 -- Maximum possible nodes in TSP link system.

    local projectSetup = {{localnode, [[2636A]], [[2.2.1]]}} -- Instrument configuration for the project.
    local currentSetup = {} -- Current instrument configuration.

    local errorTag = "[{error}]"
    local errorMessage = {[[Instrument in project configuration not found.]],
                          [[Instrument configuration at does not match.]]}
    local errorNo = 0

    -- ==========================================================================
    -- Configures the status model to detect voltage or current compliance.
    -- ==========================================================================
    local ConfigStatusModel = function()
        for i, selNode in ipairs(systemNodes) do
            selNode.status.reset()
            for j, selSmu in ipairs(systemSmus[i]) do
                -- The selSmu.source.compliance call will force an update
                -- the condition register of the measurement.instrument.smuX to
                -- update
                local dum = selSmu.source.compliance
                if (j == 1) then
                    -- Clear the measurement.instrument.smua event register by reading its value
                    dum = selNode.status.measurement.instrument.smua.event
                    -- Now configure the status model to detect voltage or current compliance
                    selNode.status.measurement.instrument.smua.enable = 3
                    selNode.status.measurement.instrument.smua.ptr = 3
                    selNode.status.measurement.instrument.enable = 6
                    selNode.status.measurement.instrument.ptr = 6
                elseif (j == 2) then
                    -- Clear the measurement.instrument.smub event register by reading its value
                    dum = selNode.status.measurement.instrument.smub.event
                    -- Now configure the status model to detect voltage or current compliance
                    selNode.status.measurement.instrument.smub.enable = 3
                    selNode.status.measurement.instrument.smub.ptr = 3
                end
            end
        end
    end

    -- ==========================================================================
    -- Determines the current system configuration.
    -- ==========================================================================
    local GetSetup = function()
        local masterNode = tsplink.node

        systemSmus = {}
        systemNodes = {}
        systemNodes[1] = node[masterNode]

        currentSetup[1] = {}
        currentSetup[1][1] = node[masterNode]
        currentSetup[1][2] = node[masterNode].model
        currentSetup[1][3] = node[masterNode].revision

        systemSmus[1] = {}
        systemSmus[1][1] = node[masterNode].smua

        if (node[masterNode].smub ~= nil) then
            systemSmus[1][2] = node[masterNode].smub
        end

        local j = 2
        for i = 1, maxNodes do
            if ((tsplink.node ~= i) and (node[i] ~= nil)) then
                systemNodes[j] = node[i]
                currentSetup[j] = {}
                currentSetup[j][1] = node[i]
                currentSetup[j][2] = node[i].model
                currentSetup[j][3] = node[i].revision

                systemSmus[j] = {}
                systemSmus[j][1] = node[i].smua

                if (node[i].smub ~= nil) then
                    systemSmus[j][2] = node[i].smub
                end
                j = j + 1
            end
        end
    end

    GetSetup()

    -- Check the project setup versus the configuration setup.
    -- If there is a difference, report an error.
    local numpInstruments = table.getn(projectSetup)
    local numcInstruments = table.getn(currentSetup)

    for i = 1, numpInstruments do
        local pfields = table.getn(projectSetup[i])

        errorNo = 1
        for j = 1, numcInstruments do
            -- Compare nodes.
            if (projectSetup[i][1] == currentSetup[j][1]) then
                errorNo = 0

                for k = 2, pfields do
                    if (projectSetup[i][k] ~= currentSetup[j][k]) then
                        errorNo = 2
                    end
                end
                break
            end
        end

        if (errorNo > 0) then
            break
        end
    end

    if (errorNo > 0) then
        error(errorMessage[errorNo])
        print(errorTag .. errorMessage[errorNo])
    end

    -- ==========================================================================
    -- Initializes all system readings buffers by clearing and setting them to
    -- append mode. An array, sysSmuReadingBufferIndexes, is used to keep track of the
    -- data stored to each of the reading buffers.
    -- ==========================================================================
    local InitalizeReadingBuffers = function()
        systemSmuReadingBufferIndexes = {}
        for i, selNode in ipairs(systemSmus) do
            systemSmuReadingBufferIndexes[i] = {}

            for j, selSmu in ipairs(selNode) do
                -- Create an array for the smu.
                systemSmuReadingBufferIndexes[i][j] = {}

                local snvBuffers = {selSmu.nvbuffer1, selSmu.nvbuffer2}
                for k, selBuffer in ipairs(snvBuffers) do
                    -- Create an array for nvbuffers 1 and 2.
                    systemSmuReadingBufferIndexes[i][j][k] = {}

                    local sysSmuReadingBufferIndexes = systemSmuReadingBufferIndexes[i][j][k]
                    sysSmuReadingBufferIndexes["start"] = 0
                    sysSmuReadingBufferIndexes["stop"] = 0

                    selBuffer.clear()
                    selBuffer.appendmode = 1
                    selBuffer.collecttimestamps = 1
                    selBuffer.collectsourcevalues = 1
                    selBuffer.timestampresolution = 1e-6
                    if (selBuffer.fillmode ~= nil) then
                        selBuffer.fillmode = selSmu.FILL_ONCE
                    end
                end

            end
        end
    end

    InitalizeReadingBuffers()
    ConfigStatusModel()
end
----------------------------------------------------------------------------
-- END OF INITIALIZE SEGMENT ... do not modify code after this point
----------------------------------------------------------------------------

----------------------------------------------------------------------------
-- START OF SWEEP SEGMENT ... do not modify this section
----------------------------------------------------------------------------
-- ==========================================================================
-- Configures a sweeping test.
-- (This represents a Sweep segment of the script.  Click on the Sweep tab to customize it.)
-- ==========================================================================
function _Sweep()
    local highSpeedSampling = false
    local errorTag = "[{error}]"
    local biasIndex = 1
    local stepIndex = 2
    local sweepIndex = 3
    local numLevels = 3
    local nvBuffersUsed = {1, 1, 1}
    -- local alias for SMUs
    local sweep1 = localnode.smua -- is a 2636A
    -- "NoStepFixed" configuration (autorange enabled = false,  #step channels = 0):
    -- This function configures any number of sweeping smus with no stepping smus.
    -- It supports sweeps with autorange off and pulse mode.
    local testSmus = {{{}}, -- Bias
    {{}}, -- Step
    {{localnode.smua}} -- Sweep
    }

    local testNodes = {{}, -- Bias
    {}, -- Step
    {localnode} -- Sweep
    }

    local sweepPulseModes = {{false}}

    local timePoint = 0.1
    local pulseWidth = 99.0000009537E-3
    local measurePointTime = 16.7266666667E-3

    local numberOfSteps = 1
    local numberOfSweeps = #NumPoints

    local nplc = 1
    local lineFrequency = 60

    local numNodes = table.getn(testNodes[sweepIndex])
    local numMasterSmus = table.getn(testSmus[sweepIndex][1])
    local masterNode = testNodes[sweepIndex][1]
    local masterSweepSmu = testSmus[sweepIndex][1][1]

    local t1Timer = masterNode.trigger.timer[1] -- t1Timer is for sweep time per point
    local t2Timer = masterNode.trigger.timer[2] -- t2Timer is for sweep pulse width
    local t3Timer = masterNode.trigger.timer[3] -- t3Timer is for a 1ms initial delay

    local masterTspLine1 = masterNode.tsplink.trigger[1] -- for source stimulus
    local masterTspLine2 = masterNode.tsplink.trigger[2] -- for endpulse stimulus

    -- ==========================================================================
    -- This function checks for errors or conflicts in the test settings (timing, ranges, etc.)
    -- ==========================================================================
    local CheckSettings = function()
        local measureDelay = timePoint - measurePointTime

        local errorMsg = nil

        -- Valid timing paramaters
        -- None of these errors should appear to the user.
        if (masterSweepSmu == nil) then
            errorMsg = [[Must have at least one sweeping smu.]]
        elseif (timePoint <= 0) then
            errorMsg = [[timePoint <= 0]]
        elseif (pulseWidth <= 0) then
            errorMsg = [[pulseWidth <= 0]]
        elseif (pulseWidth > timePoint) then
            errorMsg = [[pulseWidth > timePoint]]
        elseif (measureDelay < 0) then
            errorMsg = [[timePoint too small, <= measurePointTime]]
        end

        if (errorMsg ~= nil) then
            print(errorTag .. errorMsg)
            error(errorMsg)
        end
    end

    -- ==========================================================================
    -- This function configures the trigger line interaction between the smus.
    -- ==========================================================================
    local ConfigureTriggerLines = function()
        -- ==========================================================================
        -- This function configures the timers that are used in the test.
        -- ==========================================================================
        local ConfigureSweepTimers = function()
            -- t3Timer provides a 1ms initial delay following the masterSweepSmu's armed event ID.
            -- This is to ensure that masterSweepSmu is fully ready for the first source stimulus.
            t3Timer.stimulus = masterSweepSmu.trigger.ARMED_EVENT_ID
            t3Timer.passthrough = false
            t3Timer.delay = 1.0e-3
            t3Timer.count = 1

            if (numberOfSweeps > 1) then
                t1Timer.stimulus = t3Timer.EVENT_ID
                t1Timer.passthrough = true
                t1Timer.delay = timePoint
                t1Timer.count = numberOfSweeps - 1

                t2Timer.stimulus = t1Timer.EVENT_ID
            else
                -- Cannot use t1Timer when numberOfSweeps <= 1
                t2Timer.stimulus = t3Timer.EVENT_ID
            end

            t2Timer.passthrough = false
            t2Timer.delay = pulseWidth
            t2Timer.count = 1
        end

        -- ==========================================================================
        -- This function configures the sweeping smus.
        -- ==========================================================================
        local ConfigureSweep = function()

            -- ==========================================================================
            -- This function configures all sweeping smus on the master node.
            -- ==========================================================================
            local ConfigureMasterNodeSweepSmus = function()
                for i = 1, numMasterSmus do
                    local pulseMode = sweepPulseModes[1][i]
                    local selSmu = testSmus[sweepIndex][1][i]
                    local sourceStimulus = masterTspLine1.EVENT_ID

                    if (i == 1) then
                        -- masterSweepSmu will be triggered by StartSweep()
                        -- Use a dummy event ID as a place holder for the arm stimulus
                        selSmu.trigger.arm.stimulus = masterNode.trigger.EVENT_ID
                    else
                        selSmu.trigger.arm.stimulus = 0
                    end

                    selSmu.trigger.source.stimulus = sourceStimulus
                    selSmu.trigger.source.action = selSmu.ENABLE

                    if ((pulseMode == true) and (pulseWidth < timePoint)) then
                        selSmu.trigger.endpulse.stimulus = masterTspLine2.EVENT_ID
                        selSmu.trigger.endpulse.action = selSmu.SOURCE_IDLE
                    else
                        selSmu.trigger.endpulse.stimulus = selSmu.trigger.MEASURE_COMPLETE_EVENT_ID
                        selSmu.trigger.endpulse.action = selSmu.SOURCE_HOLD
                    end

                    if (highSpeedSampling == true) then
                        selSmu.trigger.measure.stimulus = sourceStimulus
                        selSmu.trigger.measure.action = selSmu.ASYNC

                        -- Cannot collect source values when in asynchronous mode
                        selSmu.nvbuffer1.collectsourcevalues = 0
                        selSmu.nvbuffer2.collectsourcevalues = 0
                    else
                        selSmu.trigger.measure.stimulus = 0
                        selSmu.trigger.measure.action = selSmu.ENABLE
                    end

                    selSmu.trigger.arm.count = numberOfSteps
                    selSmu.trigger.count = numberOfSweeps
                end
            end

            -- ==========================================================================
            -- This function configures a sweeping smu on a remote node
            --
            -- parameter pulseMode      true for Pulse mode,  false otherwise.
            -- parameter selNode        Node to which the smu belongs.
            -- parameter selSmu         Smu to configure.
            -- ==========================================================================
            local ConfigureRemoteSweepSmu = function(pulseMode, selNode, selSmu)
                local tspl1 = selNode.tsplink.trigger[1]
                local tspl2 = selNode.tsplink.trigger[2]
                local sourceStimulus = tspl1.EVENT_ID

                selSmu.trigger.arm.stimulus = 0
                selSmu.trigger.source.stimulus = sourceStimulus
                selSmu.trigger.source.action = selSmu.ENABLE

                if ((pulseMode == true) and (pulseWidth < timePoint)) then
                    selSmu.trigger.endpulse.stimulus = tspl2.EVENT_ID
                    selSmu.trigger.endpulse.action = selSmu.SOURCE_IDLE
                else
                    selSmu.trigger.endpulse.stimulus = selSmu.trigger.MEASURE_COMPLETE_EVENT_ID
                    selSmu.trigger.endpulse.action = selSmu.SOURCE_HOLD
                end

                if (highSpeedSampling == true) then
                    selSmu.trigger.measure.stimulus = sourceStimulus
                    selSmu.trigger.measure.action = selSmu.ASYNC

                    -- Cannot collect source values when in asynchronous mode
                    selSmu.nvbuffer1.collectsourcevalues = 0
                    selSmu.nvbuffer2.collectsourcevalues = 0
                else
                    selSmu.trigger.measure.stimulus = 0
                    selSmu.trigger.measure.action = selSmu.ENABLE
                end

                selSmu.trigger.arm.count = numberOfSteps
                selSmu.trigger.count = numberOfSweeps
            end

            -- ===========================
            -- START of ConfigureSweep()
            -- ===========================
            -- Prepare a tsplink trigger line for source stimulus on the master node
            masterTspLine1.mode = tsplink.TRIG_FALLING
            masterTspLine1.pulsewidth = 1e-6
            if (numberOfSweeps > 1) then
                masterTspLine1.stimulus = t1Timer.EVENT_ID
            else
                masterTspLine1.stimulus = t3Timer.EVENT_ID
            end

            -- Prepare a tsplink trigger line for endpulse stimulus on the master node
            masterTspLine2.mode = tsplink.TRIG_FALLING
            masterTspLine2.pulsewidth = 1e-6
            masterTspLine2.stimulus = t2Timer.EVENT_ID

            -- Configure all sweep smus on the master node.
            -- The master node contains the smu that kicks off the sweeps.
            ConfigureMasterNodeSweepSmus()

            -- Configure the sweeping smus on remote nodes
            if (numNodes > 1) then
                for i = 2, numNodes do
                    -- For each remote node, configure the tsplink trigger lines
                    local selNode = testNodes[sweepIndex][i]

                    local tspl1 = selNode.tsplink.trigger[1]
                    tspl1.mode = tsplink.TRIG_FALLING
                    tspl1.pulsewidth = 1e-6
                    tspl1.stimulus = 0

                    local tspl2 = selNode.tsplink.trigger[2]
                    tspl2.mode = tsplink.TRIG_FALLING
                    tspl2.pulsewidth = 1e-6
                    tspl2.stimulus = 0

                    local sweepSmus = testSmus[sweepIndex]
                    local numSmus = table.getn(sweepSmus[i])

                    for j = 1, numSmus do
                        -- For each sweeping smu on each remote node, configure the remote sweeping smu
                        local extSmu = sweepSmus[i][j]
                        local pulseMode = sweepPulseModes[i][j]

                        ConfigureRemoteSweepSmu(pulseMode, selNode, extSmu)
                    end
                end
            end
        end

        -- Configure timers and sweep trigger lines.
        ConfigureSweepTimers()
        ConfigureSweep()
    end

    -- ==========================================================================
    -- This function kicks off the sweep.
    -- ==========================================================================
    local StartSweep = function()
        masterSweepSmu.trigger.arm.set()
    end
    -- ==========================================================================
    -- Resets all components that may be used in the test.
    -- ==========================================================================
    local Reset = function()
        for i, selTest in ipairs(testSmus) do
            for j, selNode in ipairs(testNodes[i]) do
                if (selNode ~= nil) then
                    -- Reset blenders
                    local k = 1
                    while (selNode.tsplink.trigger[k] ~= nil) do
                        selNode.tsplink.trigger[k].reset()
                        k = k + 1
                    end

                    k = 1
                    while (selNode.trigger.blender[k] ~= nil) do
                        selNode.trigger.blender[k].reset()
                        k = k + 1
                    end

                    -- Reset Timers
                    k = 1
                    while (selNode.trigger.timer[k] ~= nil) do
                        selNode.trigger.timer[k].reset()
                        k = k + 1
                    end
                end
            end

            -- Reset Smus
            for j, selNode in ipairs(selTest) do
                for k, selSmu in ipairs(selNode) do
                    if (selSmu ~= nil) then
                        -- Resets all smus in the test.
                        selSmu.reset()
                    end
                end
            end
        end
    end

    -- ==========================================================================
    -- Turns all smus in the test on.
    -- ==========================================================================
    local TurnSmusOn = function()
        for i, selTest in ipairs(testSmus) do
            for j, selNode in ipairs(selTest) do
                for k, selSmu in ipairs(selNode) do
                    if (selSmu ~= nil) then
                        if (selSmu.source.output ~= 1) then
                            selSmu.source.output = 1
                        end
                    end
                end
            end
        end
    end

    -- ==========================================================================
    -- This function initiates the steping and sweeping smus.
    -- ==========================================================================
    local InitiateSmus = function()
        for i = 2, numLevels do
            for j, selNode in ipairs(testSmus[i]) do
                for k, selSmu in ipairs(selNode) do
                    if (selSmu ~= nil) then
                        selSmu.trigger.initiate()
                    end
                end
            end
        end
    end

    -- ==========================================================================
    -- This function determines the number of points that will be in the buffer at the end of the test.
    -- ==========================================================================
    local StoreReadingBufferIndexes = function()
        -- ==========================================================================
        -- Determines reading buffer starting and stopping indexes.
        -- parameter    testLevel    Index referring to bias, step, or sweep.
        -- parameter    i            Node index.
        -- parameter    selSmuIndex  Selected smu index.
        -- ==========================================================================
        local SetupLastPoints = function(testLevel, i, selSmuIndex)
            local stoppingPoint1 = 0
            local stoppingPoint2 = 0
            local incVal = 0
            local selSmuRbIndex1 = systemSmuReadingBufferIndexes[i][selSmuIndex][1]
            local selSmuRbIndex2 = systemSmuReadingBufferIndexes[i][selSmuIndex][2]

            selSmuRbIndex1["start"] = 1
            selSmuRbIndex2["start"] = 1

            -- appendMode is a local variable common to the script.
            if (appendMode == 1) then
                stoppingPoint1 = selSmuRbIndex1["stop"]
                stoppingPoint2 = selSmuRbIndex2["stop"]
            end
            -- Bias setup
            if (testLevel == biasIndex) then
                incVal = 1
                -- Step setup
            elseif (testLevel == stepIndex) then
                incVal = (1 * 1)
                -- Sweep setup
            elseif (testLevel == sweepIndex) then
                if (highSpeedSampling == true) then
                    incVal = (1 * 200 * 60)
                else
                    incVal = (1 * 200 * 1)
                end
            end

            selSmuRbIndex1["stop"] = stoppingPoint1 + incVal
            if (nvBuffersUsed[testLevel] == 2) then
                selSmuRbIndex2["stop"] = stoppingPoint2 + incVal
            end
        end

        for i, selTest in ipairs(testSmus) do
            for j, selNode in ipairs(selTest) do
                for k, selSmu in ipairs(selNode) do
                    for l, selSysNode in ipairs(systemSmus) do
                        if (selSysNode[1] == selSmu) then
                            SetupLastPoints(i, l, 1)
                        end

                        if (selSysNode[2] ~= nil) then
                            if (selSysNode[2] == selSmu) then
                                SetupLastPoints(i, l, 2)
                            end
                        end
                    end
                end
            end
        end
    end
    ---==========================================================================
    -- The following makes sure we capture a measurement from the bias channels
    -- ==========================================================================
    local MeasureBiasChannels = function()
    end
    -- ==========================================================================
    -- This function Configures the smu options such as nplc, function, range, etc.
    -- ==========================================================================
    local ConfigureSmus = function(rangei)
        -- set up Sweep (inner sweep) for sweep1
        sweep1.sense = sweep1.SENSE_LOCAL
        sweep1.source.delay = 0
        sweep1.measure.nplc = nplc
        sweep1.measure.autozero = sweep1.AUTOZERO_OFF
        sweep1.measure.count = 1
        sweep1.measure.filter.enable = sweep1.FILTER_OFF
        sweep1.measure.filter.type = sweep1.FILTER_MOVING_AVG
        sweep1.measure.filter.count = 1
        sweep1.measure.delay = 0
        sweep1.measure.delayfactor = 1
        sweep1.measure.analogfilter = 0
        sweep1.source.func = sweep1.OUTPUT_DCVOLTS
        if sweep1.source.highc == sweep1.ENABLE then
            sweep1.source.highc = sweep1.DISABLE
        end
        sweep1.source.limiti = 10E-3
        sweep1.trigger.source.limiti = sweep1.LIMIT_AUTO
        sweep1.source.autorangev = 0
        sweep1.source.rangev = 2
        sweep1.trigger.source.listv({#VoltList})
        sweep1.measure.autorangei = 0
        sweep1.measure.rangei = rangei -- 100E-12
        sweep1.measure.autorangev = sweep1.source.autorangev
        if (sweep1.measure.autorangev == 0) then
            sweep1.measure.rangev = sweep1.source.rangev
        end
        sweep1.trigger.measure.iv(sweep1.nvbuffer1, sweep1.nvbuffer2)
        nvBuffersUsed[sweepIndex] = 2
    end
    -- Test starts here.

    -- Make sure previous test is done.

    _WaitForComplete(-1)

    CheckSettings()
    Reset()

    smua.source.limiti = 1e-3
    smua.source.levelv = .001
    smua.source.output = 1
    delay(.1)
    local reading = smua.measure.i() * 2000

    if (reading < rangei) then
        rangei = rangei / 100
        smua.source.output = 1
        smua.source.limiti = 1e-3
        smua.measure.rangei = rangei
        smua.source.levelv = .001
        delay(.1)
        reading = smua.measure.i() * 2000
        if (reading < rangei) then
            rangei = rangei / 100
            smua.source.output = 1
            smua.source.limiti = 1e-3
            smua.measure.rangei = rangei
            smua.source.levelv = .001
            delay(.1)
            reading = smua.measure.i() * 2000
            if (reading < rangei) then
                rangei = rangei / 1000
                doCap =1
                smua.source.levelv = 0
            end
        end
    end

    ConfigureSmus(rangei)
    ConfigureTriggerLines()

    StoreReadingBufferIndexes()
    TurnSmusOn()
    MeasureBiasChannels()
    smua.source.levelv = 0

    InitiateSmus()
    delay(1)
    StartSweep()
end
----------------------------------------------------------------------------
-- END OF SWEEP SEGMENT ... do not modify code after this point
----------------------------------------------------------------------------

----------------------------------------------------------------------------
-- START OF DATAREPORT SEGMENT ... do not modify this section
----------------------------------------------------------------------------
-- ==========================================================================
-- This script returns a series of reading buffers. 
-- (This represents a DataReport segment of the script.  Click on the Data tab to customize it.)
-- ==========================================================================
function _Data()
    local buffers = {localnode.smua.nvbuffer1, localnode.smua.nvbuffer2} -- Array of reading buffers to return
    local bufferNames = {[[localnode.smua.nvbuffer1]], [[localnode.smua.nvbuffer2]]} -- Array of reading buffer names
    local bufferSmuNames = {[[Sweep_SMU]], [[Sweep_SMU]]} -- Array of SMU names for each reading buffer
    local expectedCount = {} -- Array containing the expected number of return values.
    local waitInterval = 1 -- Interval between message transfers
    local maxToReturn = 100 -- Maximum reading buffer points between message transfers.

    -- The following are special tokens used internally by the data report function
    local DATA_REPORT = "(({{Data}}))"
    local SWEEPSTART = "{SWEEP-START}"
    local START = "{START}"
    local NAME = "{NAME}"
    local EXPECTED_COUNT = "{EXPECTED-COUNT}"
    local PTS_IN_BUFF = "{PTS-IN-BUFF}"
    local PTS_RETURNED = "{PTS-RETURNED}"
    local BASE_TIME_STAMP = "{BASE-TIME-STAMP}"
    local READINGS = "{READINGS}"
    local TIMESTAMPS = "{TIMESTAMPS}"
    local SRCVALS = "{SRCVALS}"
    local END = "{END}"
    local COMPLETE = "{COMPLETE}"

    local errorTag = "[{error}]"
    local dataIndexes = {} -- Data transfer index for each reading buffer
    local done = true -- True when data transfer from all of reading buffers is completed
    local dataComplete = true -- True when all of reading buffers have reached their expected counts

    -- ==========================================================================
    -- This function determines the ending points that will be in the buffer
    -- at the time data collection takes place.
    -- ==========================================================================
    local GetStopPoints = function()
        for i, selBuffer in ipairs(buffers) do
            for j, selSysNode in ipairs(systemSmus) do
                for k, selSmu in ipairs(selSysNode) do
                    if (selSmu.nvbuffer1 == selBuffer) then
                        expectedCount[i] = systemSmuReadingBufferIndexes[j][k][1]["stop"]
                    elseif (selSmu.nvbuffer2 == selBuffer) then
                        expectedCount[i] = systemSmuReadingBufferIndexes[j][k][2]["stop"]
                    end
                end
            end
        end
    end

    -- ==========================================================================
    -- Checks if the SMU for selBuffer is sweeping.
    -- ==========================================================================
    local IsSweeping = function(selBuffer)
        for i, selNode in ipairs(systemNodes) do
            for j, selSmu in ipairs(systemSmus[i]) do
                if (j == 1) then
                    if ((selSmu.nvbuffer1 == selBuffer) or (selSmu.nvbuffer2 == selBuffer)) then
                        local statcond = selNode.status.operation.instrument.smua.condition
                        return (bit.test(statcond, 4)) -- Check Bit B3, Sweeping (SWE)
                    end
                elseif (j == 2) then
                    if ((selSmu.nvbuffer1 == selBuffer) or (selSmu.nvbuffer2 == selBuffer)) then
                        local statcond = selNode.status.operation.instrument.smub.condition
                        return (bit.test(statcond, 4)) -- Check Bit B3, Sweeping (SWE)
                    end
                end
            end
        end
        return false
    end

    -- ==========================================================================
    -- Clears the reading buffers and resets the array that contains the
    -- reading buffer storage start/ stop points.
    -- ==========================================================================
    local ClearReturnedBuffers = function()
        for i, selBuffer in ipairs(buffers) do
            selBuffer.clear()

            local smuBufferIndex = nil
            for j, selSysNode in ipairs(systemSmus) do
                for k, selSmu in ipairs(selSysNode) do
                    if (selSmu.nvbuffer1 == selBuffer) then
                        smuBufferIndex = systemSmuReadingBufferIndexes[j][k][1]
                    elseif (selSmu.nvbuffer2 == selBuffer) then
                        smuBufferIndex = systemSmuReadingBufferIndexes[j][k][2]
                    end
                end
            end
            if (smuBufferIndex ~= nil) then
                smuBufferIndex["start"] = 0
                smuBufferIndex["stop"] = 0
            end
        end
    end

    GetStopPoints()

    -- Initialize dataIndex. This array is used to keep track of the points returned to
    -- the application.

    for i, v in ipairs(buffers) do
        dataIndexes[i] = 1
    end

    -- The following print statements turns on data report feature within the application.

    repeat
        done = true
        dataComplete = true
        for i, selBuffer in ipairs(buffers) do
            -- Check for smu trigger model overruns.
            _Overruncheck()

            -- Check for smu compliance.
            _ComplianceCheck()

            -- Caution: Check for stillSweeping BEFORE getting numPointsStored
            local stillSweeping = IsSweeping(selBuffer)
            local numPointsStored = selBuffer.n

            -- Check for sweep completion before data is complete (i.e. something bad happened)
            if ((numPointsStored < expectedCount[i]) and (stillSweeping == false)) then
                gTestAborted = true
            end

            dataComplete = dataComplete and (numPointsStored >= expectedCount[i])
            done = done and (dataIndexes[i] > expectedCount[i])

            if (dataIndexes[i] <= numPointsStored) then
                local points = numPointsStored - dataIndexes[i] + 1

                if (points > maxToReturn) then
                    points = maxToReturn
                end

                local retString = string.format("%d,%d,", i, points)

                local start = dataIndexes[i]
                local stop = start + points - 1

                local readings = ""

                for j = start, stop do
                    if (j == start) then
                        readings = readings .. string.format("%e,", selBuffer.readings[j])
                    else
                        readings = readings .. string.format("%e,", selBuffer.readings[j])
                    end
                end

                local timestamps = ""
                if (selBuffer.collecttimestamps == 1) then
                    for j = start, stop do
                        if (j == start) then
                            timestamps = timestamps .. string.format("%e,", selBuffer.timestamps[j])
                        else
                            timestamps = timestamps .. string.format("%e,", selBuffer.timestamps[j])
                        end
                    end
                end

                local srcValues = ""
                if (selBuffer.collectsourcevalues == 1) then
                    for j = start, stop do
                        if (j == start) then
                            srcValues = srcValues .. string.format("%e,", selBuffer.sourcevalues[j])
                        else
                            srcValues = srcValues .. string.format("%e,", selBuffer.sourcevalues[j])
                        end
                    end
                end

                dataIndexes[i] = stop + 1

                print(string.format("%d,%d,", i + 1, points) .. readings)
                print(string.format("%d,%d,", 0, points) .. timestamps)
                print(string.format("%d,%d,", 1, points) .. srcValues)

            end
        end

        if (dataComplete == false) then
            if (gTestAborted == true) then
                done = true -- Stop waiting for data completion because it will never happen
            else
                delay(waitInterval)
            end
        end
    until (done == true)

    print(COMPLETE .. "\n")

    -- The application may not have selected all the buffers being used in the test, so
    -- we need to make sure the overlapped operations are complete before clearing the buffers.
    local sweepCompleted = _WaitForComplete(-1)

    if (sweepCompleted == true) then
        ClearReturnedBuffers()
    end
end
----------------------------------------------------------------------------
-- END OF DATAREPORT SEGMENT ... do not modify code after this point
----------------------------------------------------------------------------

----------------------------------------------------------------------------
-- START OF FINALIZE SEGMENT ... do not modify this section
----------------------------------------------------------------------------
-- ==========================================================================
-- The function completes the script and places the instrument in a known state.
-- ==========================================================================
function _Finalize()
    -- Wait for test completion
    local sweepCompleted = _WaitForComplete(-1)

    -- Check for abnornal completion
    if (sweepCompleted == false) then
        -- Abort all overlapped operations on all smus
        for i, selNode in ipairs(systemNodes) do
            for j, selSmu in ipairs(systemSmus[i]) do
                selSmu.abort()
            end
        end

        -- Check for errors
        if (errorqueue.count > 0) then
            print("[{error}]Script stopped due to error(s).")
        else
            print("[{error}]Script stopped before completion.")
        end
    end

    -- Set output level to 0V and 0A for all SMUs
    for i, selNode in ipairs(systemNodes) do
        for j, selSmu in ipairs(systemSmus[i]) do
            selSmu.source.leveli = 0
            selSmu.source.levelv = 0
        end
    end

    -- Turn output off for all SMUs
    for i, selNode in ipairs(systemNodes) do
        for j, selSmu in ipairs(systemSmus[i]) do
            selSmu.source.output = 0
        end
    end

    -- Reset the system,  placing it into a known safe state.
    reset()
end
----------------------------------------------------------------------------
-- END OF FINALIZE SEGMENT ... do not modify code after this point
----------------------------------------------------------------------------

_Initialize()
_Sweep()
_Data()
_Finalize()




function AC_Waveform_Sweep(Vrms,VLin, numCycles, frequency, limitI,skips,center,display)
	reset()
	local COMPLETE = "{COMPLETE}"
	-- Generate the source values
	local Vpp				= Vrms * math.sqrt(2)
	local sourceValues		= {} 
	local pointsPerCycle	= 7200 / frequency
	local numDataPoints		= pointsPerCycle * numCycles

	local res = {}
    local cap = {}
    for i = 1, numDataPoints do
        res[i] = math.sin(i * 2 * math.pi / pointsPerCycle)
        cap[i] = math.cos(i * 2 * math.pi / pointsPerCycle)
        sourceValues[i] = (Vpp * res[i]) + Vpp*center + VLin
    end

	-- Configure the SMU ranges
	smua.reset()
	smua.source.settling		= smua.SETTLE_FAST_POLARITY
	smua.source.autorangev		= smua.AUTORANGE_OFF
	smua.source.autorangei		= smua.AUTORANGE_OFF
    smua.source.rangev			= math.abs( Vpp+ 1.1*Vpp*center) +math.abs( VLin)
	smua.source.delay			= 0
	smua.source.limiti			= 1e-3

	smua.measure.autorangev		= smua.AUTORANGE_OFF
	smua.measure.autorangei		= smua.AUTORANGE_OFF
	smua.measure.autozero		= smua.AUTOZERO_OFF
	smua.measure.delay			= 0
	smua.measure.delayfactor    = 1
	smua.measure.analogfilter   = 0

	-- Voltage will be measured on the same range as the source range
	smua.measure.rangei			= limitI
	smua.measure.nplc			= 0.001

	-- Prepare the Reading Buffers
	smua.nvbuffer1.clear()
	smua.nvbuffer1.collecttimestamps	= 1
	smua.nvbuffer2.clear()
	smua.nvbuffer2.collecttimestamps	= 1

	-- Configure the trigger model
	--============================
	
	-- Timer 1 controls the time between source points
	trigger.timer[1].delay = (1 / 7200)
	trigger.timer[1].passthrough = true
	trigger.timer[1].stimulus = smua.trigger.ARMED_EVENT_ID
	trigger.timer[1].count = numDataPoints - 1

	-- Configure the SMU trigger model
	smua.trigger.source.listv(sourceValues)
	smua.trigger.source.limiti		= limitI
	smua.trigger.measure.action		= smua.ENABLE
	smua.trigger.measure.iv(smua.nvbuffer1, smua.nvbuffer2)
	smua.trigger.endpulse.action	= smua.SOURCE_HOLD
	smua.trigger.endsweep.action	= smua.SOURCE_IDLE
	smua.trigger.count				= numDataPoints
	smua.trigger.arm.stimulus		= 0
	smua.trigger.source.stimulus	= trigger.timer[1].EVENT_ID
	smua.trigger.measure.stimulus	= 0
	smua.trigger.endpulse.stimulus	= 0
	smua.trigger.source.action		= smua.ENABLE
	-- Ready to begin the test

	smua.source.output					= smua.OUTPUT_ON
	-- Start the trigger model execution
	smua.trigger.initiate()
	-- Wait until the sweep has completed
	waitcomplete()
	smua.source.output					= smua.OUTPUT_OFF

	-- Print the data back to the Console in tabular format
    singleWave = {}
	visits ={}
	size1=0
	phaseC = 0
    phaseS = 0
    mid = 0
    cc=0
    for x=pointsPerCycle, smua.nvbuffer2.n do
      	local i = math.floor( x - math.floor(x/(pointsPerCycle))*(pointsPerCycle))
		v=	smua.nvbuffer1[x]
		if (math.abs(v)<1e-3) then
            if (singleWave[i]==nil) then 
                singleWave[i]=v
                visits[i]=1
                if (size1<i) then size1=i end
            else 
                singleWave[i]=singleWave[i]+v
                visits[i]=visits[i]+ 1
            end	
        end
		phaseC = phaseC + cap[x] * v
        phaseS = phaseS + res[x] * v
        mid = mid + v
        cc=cc+1
--	    print(smua.nvbuffer1.timestamps[x], smua.nvbuffer2[x], smua.nvbuffer1[x])
    end

    mid = mid / cc
    phaseC = 2 * phaseC / cc
    phaseS = 2 * phaseS / cc

    rms=0
    for x=pointsPerCycle, smua.nvbuffer2.n do
      v=	smua.nvbuffer1[x]
      rms=rms + (v-mid)*(v-mid)
    end
    rms = math.sqrt(rms/cc)

    phase = 0
    cc=0
    for c=1,numCycles-1 do
		x0=pointsPerCycle*c
		for x=x0+1,x0+pointsPerCycle-2 do
             if ( smua.nvbuffer1[x]<=mid and  smua.nvbuffer1[x+1]>mid) then
				phase =phase + (x-x0)/pointsPerCycle
                cc=cc+1
				break
			 end
		end
	end
   
   pointsPerCycle=math.floor(pointsPerCycle)
   omega=frequency*2*math.pi

   if (display==1) then
       for i=1,table.getn(singleWave) do
         print(frequency, VLin, Vpp, phaseC/Vpp* math.sqrt(omega) *1565856180.88521 -57.2098793899397 , smua.nvbuffer2[i], phaseC/Vpp, phaseS/Vpp,singleWave[i],2*math.pi* phase/cc)
       end
   else 
        print(frequency, VLin, Vpp, phaseC/Vpp* math.sqrt(omega) *1565856180.88521 -57.2098793899397 , rms/Vrms, phaseC/Vpp, phaseS/Vpp,rms,2*math.pi* phase/cc)
   end
   
  
end




function Square_Waveform_Sweep(Vrms, numCycles, frequency, limitI,display)
    reset()
    local COMPLETE = "{COMPLETE}"
    -- Generate the source values
    local Vpp = Vrms * math.sqrt(2)
    local sourceValues = {}
    local pointsPerCycle = 7200 / frequency
    local numDataPoints = pointsPerCycle * numCycles

    for i = 1, numDataPoints do
        if ((Vpp * math.sin(i * 2 * math.pi / pointsPerCycle)) < 0) then
            sourceValues[i] = Vpp
        else
            sourceValues[i] = 0
        end
    end

    -- Configure the SMU ranges
    smua.reset()
    smua.source.settling = smua.SETTLE_FAST_POLARITY
    smua.source.autorangev = smua.AUTORANGE_OFF
    smua.source.autorangei = smua.AUTORANGE_OFF
    smua.source.rangev = Vpp
    smua.source.delay = 0
    smua.source.limiti = 1e-12

    smua.measure.autorangev = smua.AUTORANGE_OFF
    smua.measure.autorangei = smua.AUTORANGE_OFF
    smua.measure.autozero = smua.AUTOZERO_OFF
    smua.measure.delay = 0
    smua.measure.delayfactor = 1
    smua.measure.analogfilter = 0

    -- Voltage will be measured on the same range as the source range
    smua.measure.rangei = limitI
    smua.measure.nplc = 0.001

    -- Prepare the Reading Buffers
    smua.nvbuffer1.clear()
    smua.nvbuffer1.collecttimestamps = 1
    smua.nvbuffer2.clear()
    smua.nvbuffer2.collecttimestamps = 1

    -- Configure the trigger model
    -- ============================

    -- Timer 1 controls the time between source points
    trigger.timer[1].delay = (1 / 7200)
    trigger.timer[1].passthrough = true
    trigger.timer[1].stimulus = smua.trigger.ARMED_EVENT_ID
    trigger.timer[1].count = numDataPoints - 1

    -- Configure the SMU trigger model
    smua.trigger.source.listv(sourceValues)
    smua.trigger.source.limiti = limitI
    smua.trigger.measure.action = smua.ENABLE
    smua.trigger.measure.iv(smua.nvbuffer1, smua.nvbuffer2)
    smua.trigger.endpulse.action = smua.SOURCE_HOLD
    smua.trigger.endsweep.action = smua.SOURCE_IDLE
    smua.trigger.count = numDataPoints
    smua.trigger.arm.stimulus = 0
    smua.trigger.source.stimulus = trigger.timer[1].EVENT_ID
    smua.trigger.measure.stimulus = 0
    smua.trigger.endpulse.stimulus = 0
    smua.trigger.source.action = smua.ENABLE
    -- Ready to begin the test

    smua.source.output = smua.OUTPUT_ON
    -- Start the trigger model execution
    smua.trigger.initiate()
    -- Wait until the sweep has completed
    waitcomplete()
    smua.source.output = smua.OUTPUT_OFF

    -- Print the data back to the Console in tabular format
    singleWave = {}
    visits = {}
    size1 = 0
    for x = 1, smua.nvbuffer2.n do
        local i = math.floor(x - math.floor(x / (pointsPerCycle)) * (pointsPerCycle))
        v = smua.nvbuffer1[x]
        if (math.abs(v) < 1e-3) then
            if (singleWave[i] == nil) then
                singleWave[i] = v
                visits[i] = 1
                if (size1 < i) then
                    size1 = i
                end
            else
                singleWave[i] = singleWave[i] + v
                visits[i] = visits[i] + 1
            end
        end
        --    print(smua.nvbuffer1.timestamps[x], smua.nvbuffer2[x], smua.nvbuffer1[x],i,singleWave[i])
    end

    
    maxV = 0
    maxI = 0
    timeI = 0
    for x = 1, size1 do
        singleWave[x] = singleWave[x] / visits[x]
        v = smua.nvbuffer2[x + pointsPerCycle]
        if (v > maxV) then
            maxV = v
        end
        if (singleWave[x] > maxI) then
            maxI = singleWave[x]
            timeI = x
        end
    end

    pointsPerCycle = math.floor(pointsPerCycle)
    timeEnd =0
    for x = timeI, size1 - 1 do
        --print(smua.nvbuffer1.timestamps[x] - smua.nvbuffer1.timestamps[timeI], smua.nvbuffer2[x + pointsPerCycle],            singleWave[x] / maxI)
        i = singleWave[x] / maxI
        if (i < .36) then
            timeEnd = smua.nvbuffer1.timestamps[x] - smua.nvbuffer1.timestamps[timeI]
            break
        end
    end

    if (display==1) then
        for i=1,table.getn(singleWave) do
          print(frequency, VLin, Vpp, phaseC/Vpp* math.sqrt(omega) *1565856180.88521 -57.2098793899397 , smua.nvbuffer2[i], phaseC/Vpp, phaseS/Vpp,singleWave[i],2*math.pi* phase/cc)
        end
    else 
        print(frequency, 0, Vpp, timeEnd/4031000 , maxI/Vpp, timeEnd,0,maxI,timeEnd)
    end

end

additional = "#Additional"

if (additional=="IVC") then 
    if (doCap==1) then 
        reset()
        AC_Waveform_Sweep(.01,0, 15, 30,  10e-9,10,1,0)
        Square_Waveform_Sweep(.01, 40, 80,  10e-9,0,0)
        smua.source.output = smua.OUTPUT_OFF
    else 
        for c=1,2 do
            print(0, 0, 0, 0 , 0, 0,0,0,0)
        end
    end
    print("{COMPLETE}")
end

if (additional=="Breakdown") then 
    if (doCap==1) then 

        reset()
        smua.source.output = 1

        gm_array = gm_vsweep(smua, -.5, .5, 10)
        first = gm_array[9]*.25
        for v=.1,20,.5 do
            gm_array ,ibuf,vbuf= gm_vsweep(smua, -1*v, v, 10)
            for i=1,10 do
                print(i, vbuf[i], 0, 0 , ibuf[i], 0, 0,gm_array[i],0)
            end
           
            if (v>3 and gm_array[9]<first) then break        end
        end
        
        smua.source.output = 0

    else 
        for c=1,15 do
            print(0, 0, 0, 0 , 0, 0,0,0,0)
        end
    end
    print("{COMPLETE}")
end

if (additional=="Transconductance") then 
    if (doCap==1) then 

        reset()
        smua.source.output = 1
        gm_array, ibuf, vbuf  = gm_vsweep(smua, -.1, .1, 100)
        for i=1,100 do
            print(0, vbuf[i], 0, 0 , ibuf[i], 0, 0,gm_array[i],0)
        end
        smua.source.output = 0

    else 
        for c=1,15 do
            print(0, 0, 0, 0 , 0, 0,0,0,0)
        end
    end
    print("{COMPLETE}")
end

if (additional=="Joule") then 
    if (doCap==1) then 
        reset()
        smua.source.limiti = 1e-9
        smua.source.levelv = 7
        smua.source.output = 1

        first =.5*( smua.measure.i() + smua.measure.i() + smua.measure.i() + smua.measure.i() )/4

        for v=0,100 do
            delay(.5)
            reading =( smua.measure.i() + smua.measure.i() + smua.measure.i() + smua.measure.i() )/4
            time = os.date("*t")
           print(0, 1, 0,60*60*time.hour+ 60*time.min+ time.sec , 0, 0, 0,reading,0)
            if (reading<first) then break end
        end
        smua.source.output = smua.OUTPUT_OFF
    else 
        for c=1,2 do
            print(0, 0, 0, 0 , 0, 0,0,0,0)
        end
    end
    print("{COMPLETE}")
end

if (additional=="dC/dV") then
    if (doCap==1) then 
        reset()
        for v=-1,1,.1 do
            AC_Waveform_Sweep(.01, v, 200, 400,  10e-9,1,0,1)
        end
        smua.source.output = smua.OUTPUT_OFF
    else 
        for c=1,2 do
            print(0, 0, 0, 0 , 0, 0,0,0,0)
        end
    end
    print("{COMPLETE}")
end


if (additional=="C/F") then
    if (doCap==1) then 
        reset()
        AC_Waveform_Sweep(.01,0, 10, 10,  10e-9,5,1,1)
        AC_Waveform_Sweep(.01,0, 15, 30,  10e-9,5,1,1)
        AC_Waveform_Sweep(.01,0, 30, 60,  10e-9,5,1,1)
        AC_Waveform_Sweep(.01,0, 100, 100,  10e-9,1,1,1)
        AC_Waveform_Sweep(.01,0, 100, 200,  10e-9,1,1,1)
        AC_Waveform_Sweep(.01,0, 150, 300,  10e-9,1,1,1)
        AC_Waveform_Sweep(.01,0, 150, 400,  10e-9,1,1,1)
        AC_Waveform_Sweep(.01,0, 150, 600,  10e-9,1,1,1)
        AC_Waveform_Sweep(.01,0, 150, 800,  10e-9,1,1,1)
        AC_Waveform_Sweep(.01,0, 150, 900,  10e-9,1,1,1)
        smua.source.output = smua.OUTPUT_OFF
    else 
        for c=1,10 do
            print(0, 0, 0, 0 , 0, 0,0,0,0)
        end
    end
    print("{COMPLETE}")
end

if (additional=="Leakage") then
    if (doCap==1) then 
        reset()
        smua.source.highc = smua.ENABLE
        smua.source.levelv = 5
        smua.source.output = smua.OUTPUT_ON
        delay(1)
        imeas =  i_leakage_measure(smua, 0, 1e-3, 300e-3, 100e-12, 100e-3)
        print(0, 0, 5, 0 , 0, 0, 0,imeas,0)
        
        smua.source.output = smua.OUTPUT_OFF
    else 
            print(0, 0, 0, 0 , 0, 0,0,0,0)
    end
    print("{COMPLETE}")
end

if (additional=="Leakage Threshold") then
    if (doCap==1) then 
        reset()
        smua.source.highc = smua.ENABLE
        smua.source.levelv = 5
        smua.source.output = smua.OUTPUT_ON
        delay(1)
        pass = i_leakage_threshold(smua, 0, 1e-3,300e-3, 100e-12, 100e-6, 10e-12, 1)
        print(0, 0, 5, pass , 0, 0, 0, 10e-12 ,0)
        smua.source.output = smua.OUTPUT_OFF
    else 
       
            print(0, 0, 0, 0 , 0, 0,0,0,0)
       
    end
    print("{COMPLETE}")
end
