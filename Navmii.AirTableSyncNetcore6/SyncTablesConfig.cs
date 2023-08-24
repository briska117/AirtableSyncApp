using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6
{
    internal static class SyncTablesConfig
    {
        public const string SyncIds = "Sync IDs";
        public const string MiscSpendDetail = "Misc. Spend Detail";
        public const string Teams = "Teams";
        public const string DailyReportDetails = "Daily Report Details";
        public const string Systems = "Systems";
        public const string WeeklyStatusUpdate = "Weekly Status Update";
        public const string Transponders = "Transponders";
        public const string RigAssignments = "Rig Assignments";
        public const string TSheets = "TSheets";
        public const string ReDriveList = "Re-Drive List";
        public const string Rigs = "Rigs";
        public const string ChaseVehicles = "Chase Vehicles";
        public const string MaintenanceSpendDetail = "Maintenance Spend Detail";
        public const string Cars = "Cars";
        public const string Camps = "Camps";
        public const string Packs = "Packs";
        public const string DailyCampReports = "Daily Camp Reports";
        public const string PackAssignments = "Pack Assignments";
        public const string Interactions = "Interactions";
        public const string CommuteTimes="Commute Times";
        public static string[] TSheetsFields = {
                        "First Name (required)",
                        "Last Name (required)",
                        "cf_Job Title",
                        "cf_Work Number" };

        public static string[] CampsFields = {
                        "Camp",
                        "Camp Lead",
                        "State",
                        "Address",
                        "Storage Type",
                        "Number of Systems",
                        "First Collection Day",
                        "Last Collection Day",
                        "Notes"
        };

        public static string[] CampsFieldsPedestrians = {
                        "Camp",
                        "Camp Lead",
                        "State",
                        "Address",
                        "Number of Packs (not counting spares)",
                        "First Collection Day",
                        "Last Collection Day",
                        "Notes"
        };

        public static string[] SystemsFields =
        {
            "System ID#"
        };

        public static string[] PacksFields =
        {
                        "Pack ID#",
                        "Spare?"
        };

        public static string[] CarsFieldsTeam = {
                            "Vehicle ID Number (DVN)",
                            "VIN#",
                            "License Plate",
                            "Donlen Packet Present", // remove for TechM
                            "Registration Expiry Date",
                            "Insurance Expiry Date",
                            "Visible Damage",
                            "Date of Accident/Damage",
                            "Description of Damage",
                            "Resolution",
                            "Estimated Repair Date",
                            "Open Claims + Ins POC",
                            "Recall Service Needed",
                            "Recall Notes",
                            "Maintenance Needed",
                            "Maintenance Notes",
                            "Other Notes"

        };
        public static string[] CarsFieldsMain = {
                                "Vehicle ID Number (DVN)",
                                "VIN#",
                                "License Plate",
                                "Donlen Packet Present", // remove for TechM
                                "Registration Expiry Date",
                                "Insurance Expiry Date",
                                "Visible Damage",
                                "Date of Accident/Damage",
                                "Description of Damage",
                                "Resolution",
                                "Estimated Repair Date",
                                "Open Claims + Ins POC",
                                "Recall Service Needed",
                                "Recall Notes",
                                "Maintenance Needed",
                                "Maintenance Notes",
                                "Other Notes",
                                "Owning Team"

        };

        public static string[] RigsFields = {
                        "Car",
                        "System"};

        public static string[] ChaseVehiclesFields = {
                        "Company",
                        "PickUp Location",
                        "Pickup Date",
                        "Reservation Number",
                        "Renter Name on Reservation",
                        "Make/Model",
                        "Color",
                        "License Plate Number",
                        "Returned",
                        "Return Location",
                        "Return Date",
                        "Country",
                        "Owning Team"

        };

        public static string[] DailyReportDetailsFields =
        {
        "Report Date",
                            "Project",
                            "Camp",
                            "Build Region",
                            "DC",
                            "CCs",
                            "Counties",
                            "Sun Angle Start",
                            "Sun Angle End",
                            "Weather",
                            "Total Drivers IN CAMP (Working today or not)",
                            "Total Operators IN CAMP (working today or not)",
                            "AM Topics",
                            "Personnel Arrivals/Departures",
                            "Unique Identifier",
                            "Total Collection Hours By Partial Collect Rigs",
                            "SSDs",
                            "Equipment",
                            "Other Notable Events",
                            "Tomorrow Plan",
                            "Tomorrow Weather Forecast",
                            "Debrief Topics"
        };

        public static string[] TranspondersFields = {
        "Transponder ID",
                            "Date Installed",
                            "Vehicle #",
                            "Toll Network/System",
                            "URL for Toll System",
                            "Username",
                            "Password",
                            "Removal Date"
        };

        public static string[] RigAssignmentFields =
        {
            "Assignment Date",
                            "Project",
                            "Rig",
                            "Team",
                            "Camp",
                            "Status",
                            "Start Time",
                            "Tiles Assigned",
                            "Tiles Completed",
                            "Driver",
                            "Operator",
                            "Action Taken",
                            "Resolution",
                            "Return",
                            "Odometer",
                            "Precip Percent",
                            "SSD Percent",
                            "Notes",
                            "Case#/Contact#",
                            "End Odometer"
        };

        public static string[] ReDriveListFields =
        {
            "Original Collect Date",
                            "Re-Drive Date",
                            "Build Region",
                            "Mission",
                            "Teams",
                            "System",
                            "Total Collected KM",
                            "Reason",
                            "Source",
                            "Re-Collection Type"
        };

        public static string[] WeeklyStatusUpdateFields =
        {
            "Team",
                            "Date Entered",
                            "Rig Count",
                            "Up Rig Count",
                            "Collection Project",
                            "Projected UKMs",
                            "Current Camp",
                            "Current City",
                            "Current Collection Start Date",
                            "Estimated End of Collection Date",
                            "Estimated Completion %",
                            "Possible Weather Down Dates",
                            "Next Collection Project",
                            "Next Camp City",
                            "Next Estimated End of Collection Date"
        };

        public static string[] MaintenanceSpendDetailFields =
        {
            "Record Date",
                            "Vehicle Number",
                            "Damage?",
                            "Vendor Name",
                            "TechM Invoice #",
                            "Invoice Date",
                            "Service Date",
                            "Service Description",
                            "Parts Cost",
                            "Labor Cost/Fees"
        };

        public static string[] MiscSpendDetailFields =
        {
            "Record Date",
                            "Vehicle Number",
                            "TechM Invoice #",
                            "Invoice Date",
                            "State/Province",
                            "Expense Date",
                            "Uncategorized Expense Description",
                            "Total Expense Cost"
        };

        public static string[] DailyCampReportsfields =
        {
            "Report Date",
                            "Camp",
                            "FC",
                            "T3(s)",
                            "Sun Angle Start",
                            "Sun Angle End",
                            "Weather",
                            "Total Collection Hours By Partial Collect Packs",
                            "Total Field Staff in camp (working today or not)",
                            "AM Topics",
                            "Personnel Joining Or Leaving Camp",
                            "SSDs",
                            "Equipment",
                            "Personnel Issues (Sick etc)",
                            "Other Notable Events",
                            "Tomorrow Plan",
                            "Tomorrow Weather Forecast",
                            "Debrief Topics",
                            "Team"
        };

        public static string[] PackAssignmentsFields =
        {
            "Assignment Date",
                            "Pack",
                            "Status",
                            "Start Time",
                            "Missions Assigned",
                            "Missions Completed",
                            "Spotter",
                            "Operator",
                            "Duty Fit Check",
                            "Action Taken If Down",
                            "Resolution of Down Reason",
                            "Return",
                            "Notes",
                            "Case#/Contact#",
                            "Team"
        };

        public static string[] InteractionsFields =
        {
            "Date And Time Of Interaction",
                            "Interaction Type",
                            "Pack Assignment",
                            "Situation",
                            "Resolution",
                            "Team"
        };

        public static string[] CommuteTimesFields =
        {
            "Date and Commute Start Time",
                            "Pack and Operators",
                            "Type of Commute",
                            "Commute Mode",
                            "Commute End Time",
                            "Notes",
                            "Team"
        };

        public static string[] RigsModeMain =
        {
            "TSheets",
                    "Teams",
                    "Camps",
                    "Chase Vehicles",
                    "Daily Report Details",
                    "Transponders",
                    "Cars",
                    "Systems",
                    "Rigs",
                    "Rig Assignments",
                    "Re-Drive List",
                    "Weekly Status Update"
        };

        public static string[] RigsModeTeam =
        {
             "TSheets",
                    "Camps",
                    "Chase Vehicles",
                    "Daily Report Details",
                    "Transponders",
                    "Cars",
                    "Systems",
                    "Rigs",
                    "Rig Assignments",
                    "Re-Drive List",
                    "Weekly Status Update",
                    "Sync IDs"
        };

        public static string[] RigsModeTechMMain =
        {
            "TSheets",
                    "Teams",
                    "Camps",
                    "Chase Vehicles",
                    "Daily Report Details",
                    "Transponders",
                    "Cars",
                    "Systems",
                    "Rigs",
                    "Rig Assignments",
                    "Re-Drive List",
                    "Weekly Status Update",
                    "Maintenance Spend Detail",
                    "Misc. Spend Detail"
        };

        public static string[] RigsModeTechMTeam =
       {
            "TSheets",
                    "Camps",
                    "Chase Vehicles",
                    "Daily Report Details",
                    "Transponders",
                    "Cars",
                    "Systems",
                    "Rigs",
                    "Rig Assignments",
                    "Re-Drive List",
                    "Weekly Status Update",
                    "Sync IDs",
                    "Maintenance Spend Detail",
                    "Misc. Spend Detail"
        };

        public static string[] PedestriansModeMain =
        {
            "TSheets",
                    "Teams",
                    "Camps",
                    "Chase Vehicles",
                    "Daily Report Details",
                    "Transponders",
                    "Cars",
                    "Systems",
                    "Rigs",
                    "Rig Assignments",
                    "Re-Drive List",
                    "Weekly Status Update",
                    "Maintenance Spend Detail",
                    "Misc. Spend Detail"
        };

        public static string[] PedestriansModeTeam =
        {
            "TSheets",
                    "Camps",
                    "Chase Vehicles",
                    "Packs",
                    "Daily Camp Reports",
                    "Pack Assignments",
                    "Interactions",
                    "Commute Times",
                    "Sync IDs"
        };

    }
}
