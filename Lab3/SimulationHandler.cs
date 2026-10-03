using System.ComponentModel;
using Core;
using Core.Distributions;
using Core.Elements.Basic;
using Core.Elements.Functional;
using Lab3.Bank;
using Lab3.Hospital;

namespace Lab3;

public static class SimulationHandler
{
    public static void SimulateBank(double time)
    {
        var c = new Create<int>
        {
            Name = "Entrance",
            Distribution = new ExponentialDistribution(0.5),
            TNext = 0.1,
            RequestFactory = () => Random.Shared.Next()
        };

        var normalDist = new NormalDistribution(1.0, 0.3);

        var p1 = new Process<int>
        {
            Name = "Lane A",
            Distribution = new ExponentialDistribution(0.3),
            MaxChannels = 1,
            MaxQueue = 3
        };

        var p2 = new Process<int>
        {
            Name = "Lane B",
            Distribution = new ExponentialDistribution(0.3),
            MaxChannels = 1,
            MaxQueue = 3
        };

        p1.ForceInitialize(initialQueueCount: 2, initialRequest: 0, initialDepartureTime: normalDist.Generate());
        p2.ForceInitialize(initialQueueCount: 2, initialRequest: 0, initialDepartureTime: normalDist.Generate());

        var bp = new BankProcess<int>(p1, p2);
        var d = new Dispose<int>
        {
            Name = "Exit"
        };

        c.NextElement = bp;
        p1.NextElement = d;
        p2.NextElement = d;

        var model = new Model<int>([c, p1, p2, bp, d])
        {
            OnCustomStep = bp.Calculate
        };

        model.Simulate(time);

        BankStatisticsLogger<int>.ShowBankReport(model, bp);
    }

    public static void SimulateHospital(double time)
    {
        #region declare
        var entrance = new Create<Patient>
        {
            Name = "Entrance",
            Distribution = new ExponentialDistribution(15),
            RequestFactory = () =>
            {
                var roll = Random.Shared.NextDouble();
                return roll switch
                {
                    <= 0.5 => new Patient(PatientType.Type1),
                    <= 0.6 => new Patient(PatientType.Type2),
                    _ => new Patient(PatientType.Type3)
                };
            }
        };

        var admissionDepartment = new Process<Patient>
        {
            Name = "AdmissionDepartment",
            ServiceTimeCalculator = (patient) =>
            {
                return patient.Type switch
                {
                    PatientType.Type1 => 15,
                    PatientType.Type2 => 40,
                    PatientType.Type3 => 30,
                    _ => throw new InvalidEnumArgumentException()
                };
            },
            MaxChannels = 2
        };

        var wardAccompany = new Process<Patient>
        {
            Name = "WardAccompany",
            Distribution = new UniformDistribution(3, 8),
            MaxChannels = 3
        };

        var receptionWalk = new Process<Patient>
        {
            Name = "ReceptionWalk",
            Distribution = new UniformDistribution(2, 5),
            MaxChannels = int.MaxValue
        };

        var receptionEnrolling = new Process<Patient>
        {
            Name = "ReceptionEnrolling",
            Distribution = new ErlangDistribution(k: 3, 4.5),
            MaxChannels = 1
        };

        var laboratoryExamination = new Process<Patient>
        {
            Name = "LaboratoryExamination",
            Distribution = new ErlangDistribution(k: 2, 4),
            MaxChannels = 2
        };

        var backingToAd = new Process<Patient>
        {
            Name = "BackingToAD",
            Distribution = new UniformDistribution(2, 5)
        };

        var ward = new Dispose<Patient>
        {
            Name = "Ward"
        };

        var exit = new Dispose<Patient>
        {
            Name = "Exit"
        };

        var reclassifier = new Assign<Patient>
        {
            Name = "Type2To1",
            ModifyAction = (patient) => patient.Type = PatientType.Type1
        };

        var labArrivalTracker = new Assign<Patient>
        {
            Name = "LabArrivalTracker",
        };
        labArrivalTracker.ModifyAction = _ =>
        {
            HospitalTracker.RecordLabArrival(labArrivalTracker.TCurr);
        };

        var wardTracker = new Assign<Patient>
        {
            Name = "WardTracker",
        };
        wardTracker.ModifyAction = patient =>
        {
            HospitalTracker.RecordPatientFinish(patient, wardTracker.TCurr);
        };

        var labExitType3Tracker = new Assign<Patient>
        {
            Name = "LabExitType3Tracker",
        };
        labExitType3Tracker.ModifyAction = patient =>
        {
            HospitalTracker.RecordPatientFinish(patient, labExitType3Tracker.TCurr);
        };

        var creationTracker = new Assign<Patient>
        {
            Name = "CreationTracker"
        };
        creationTracker.ModifyAction = patient =>
        {
            if (patient.CreationTime < 0)
            {
                patient.CreationTime = creationTracker.TCurr;
            }
        };
        #endregion

        #region routing
        entrance.NextElement = creationTracker;
        creationTracker.NextElement = admissionDepartment;

        var typeSplitter = new PatientRouter
        {
            Name = "Router1SplitType",
            RoutingLogic = patient => patient.Type switch
            {
                PatientType.Type1 => wardAccompany,
                PatientType.Type2 or PatientType.Type3 => receptionWalk,
                _ => null
            }
        };
        admissionDepartment.NextElement = typeSplitter;
        wardAccompany.NextElement = wardTracker;
        wardTracker.NextElement = ward;

        receptionWalk.NextElement = receptionEnrolling;
        receptionEnrolling.NextElement = labArrivalTracker;
        labArrivalTracker.NextElement = laboratoryExamination;

        var labExitSplitter = new PatientRouter
        {
            Name = "Router2LabExit",
            RoutingLogic = patient => patient.Type switch
            {
                PatientType.Type2 => reclassifier,
                PatientType.Type3 => labExitType3Tracker,
                _ => null
            }
        };
        laboratoryExamination.NextElement = labExitSplitter;
        reclassifier.NextElement = backingToAd;
        backingToAd.NextElement = admissionDepartment;

        labExitType3Tracker.NextElement = exit;
        #endregion

        var model = new Model<Patient>([
            entrance,
            creationTracker,
            admissionDepartment,
            typeSplitter,
            wardAccompany,
            wardTracker,
            ward,
            receptionWalk,
            receptionEnrolling,
            labArrivalTracker,
            laboratoryExamination,
            labExitSplitter,
            reclassifier,
            backingToAd,
            labExitType3Tracker,
            exit
        ]);
        model.Simulate(time);
        HospitalTracker.ShowCustomReport();
    }
}
