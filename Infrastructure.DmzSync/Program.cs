using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DmzModel;
using Core.DomainModel;
using Infrastructure.DmzDataAccess;
using Infrastructure.DataAccess;
using Infrastructure.DmzSync.Encryption;

namespace Infrastructure.DmzSync
{
    class Program
    {
        public static long StringToUnixTimestamp(string dateTime)
        {
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            return (long)(date - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        static void Main(string[] args)
        {
            //Use master context and dmz context
            using (var DMZContext = new DmzContext())
            {
                using (var MasterContext = new DataContext())
                {
                    int i = 0;

                    //Read all token statuses into the master context
                    foreach (var token in DMZContext.Tokens)
                    {
                        Token t = Encryptor.DecryptToken(token);
                        Guid guid = new Guid(t.GuId);

                        MobileToken mobileTok = MasterContext.MobileTokens.FirstOrDefault(x => x.Guid == guid);

                        if (mobileTok != null && t.Status == (int)MobileTokenStatus.Activated) //Only transfer active state
                        {
                            mobileTok.Status = (MobileTokenStatus)t.Status;
                        }
                    }

                    //Try to save transfer of tokens
                    try
                    {
                        MasterContext.SaveChanges();
                        Console.WriteLine("Activated tokens transfered succesfully");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not transfer or delete tokens");
                    }

                    //Read all Drivereports, GPSCoordinates and Routes from the DMZContext into the master context
                    foreach (var report in DMZContext.DriveReports)
                    {
                        //Create new Domainmodel object
                        Core.DomainModel.DriveReport dr = new Core.DomainModel.DriveReport
                        {
                            Comment = report.ManualEntryRemark,
                            Purpose = report.Purpose
                        };

                        MasterContext.DriveReports.Add(dr);

                        // Sync Rate 
                        Core.DomainModel.Rate tmp = MasterContext.Rates.FirstOrDefault(x => x.Id == report.RateId);

                        if (tmp != null)
                            dr.KmRate = tmp.KmRate;
                        else
                            dr.KmRate = 0.0f;

                        // Sync Date
                        long fullDate = StringToUnixTimestamp(report.Date);
                        dr.DriveDateTimestamp = fullDate; //dr.Date
                        dr.CreatedDateTimestamp = fullDate; //dr.CreationDate

                        //Reference the correct profile
                        dr.Person = MasterContext.Persons.FirstOrDefault(p => p.Id == report.Profile.Id);

                        //Reference the correct employment
                        dr.EmploymentId = report.EmploymentId;

                        var model = new ReportViewModel
                        {
                            SelectedReportMethod = 0,
                            StartingFromHome = report.StartsAtHome,
                            EndingAtHome = report.EndsAtHome,
                            SelectedEmployment = dr.EmploymentId,

                            TotalRouteDistance = report.Route.TotalDistance.ToString(),
                            Date = dr.Date
                        };

                        var adminRateReference = MasterContext.AdminRates.FirstOrDefault(x => x.TFCode == masterRate.TFCode && x.Year == masterRate.Year);

                        if (adminRateReference != null)
                            model.SelectedRateType = adminRateReference.Id;
                        else
                            model.SelectedRateType = -1;

                        try
                        {
                            //Calculate reimburstment info
                            IQueryHelper qHelper = new QueryHelper();
                            INetHelper nHelper = new NetHelper();
                            IReimbursementCalculator calc = new ReimbursementCalculator(qHelper, new DistanceCalculator(qHelper, nHelper));
                            model.SubjectToFourKmRule = qHelper.IsSubjectToFourKmRule(dr.Profile.CprNr);

                            calc.CalculateDriveReportReimbursement(model);
                            dr.AmountToReimburse = model.TotalCostToReimburse;
                            dr.IsExtraDistance = model.IsExtraDistance;
                            dr.ReimburseableDistance = model.ReimburseableDistance;
                        }
                        catch (Exception ex)
                        {
                            dr.AmountToReimburse = "0";
                            dr.IsExtraDistance = false;
                            dr.ReimburseableDistance = "0";
                        }

                        //Create route object
                        MasterModel.Route r = new MasterModel.Route { RouteDescription = "Aflæst" };
                        r.TotalDistance = report.Route.TotalDistance.ToString();
                        dr.Routes.Add(r);

                        //Add GPS coordinates to the route
                        foreach (DMZModel.GPSCoordinate gps in report.Route.GPSCoordinates)
                        {
                            DMZModel.GPSCoordinate g = Encryptor.DecryptGPSCoordinate(gps);

                            MasterModel.RouteCoordinate c = new RouteCoordinate
                            {
                                Latitude = decimal.Parse(g.Latitude, CultureInfo.InvariantCulture),
                                Longitude = decimal.Parse(g.Longitude, CultureInfo.InvariantCulture),
                            };

                            c.Time = DateTime.ParseExact(g.TimeStamp, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                            r.RouteCoordinates.Add(c);
                        }

                        i++;

                        DMZContext.DriveReports.Remove(report);
                    }

                    bool didSuccesfullyTransferReports = true;

                    try //Should do more
                    {
                        MasterContext.SaveChanges();
                        Console.WriteLine("{0} Drivereports inserted", i);
                        Console.WriteLine("DMZ drivereport data deleted");
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error inserting drivereport data, sync will quit");

                        didSuccesfullyTransferReports = false;
                    }

                    if (didSuccesfullyTransferReports)
                    {
                        //---
                        // Write all the data from the Mastercontext into the DMZContext
                        //---

                        //Append new Rates
                        i = 0;
                        foreach (var rate in MasterContext.Rates)
                        {
                            //Is this enough?
                            var rateExists =
                                DMZContext.Rates.FirstOrDefault(
                                    x => x.TFCode == rate.TFCode && x.Year == rate.Year.ToString());

                            if (rateExists == null)
                            {
                                Core.DmzModel.Rate r = new Core.DmzModel.Rate
                                {
                                    TFCode = rate.TFCode,
                                    Type = rate.Type,
                                    KmRate = rate.KmRate.ToString(),
                                    Year = rate.Year.ToString()
                                };

                                DMZContext.Rates.Add(r);
                                i++;
                            }
                        }

                        Console.WriteLine("{0} Rates inserted into dmz", i);

                        //Sync Profiles, employments and tokens
                        i = 0;
                        foreach (var emp in DMZContext.Employments)
                        {
                            DMZContext.Employments.Remove(emp);
                            i++;
                        }
                        Console.WriteLine("{0} employments deleted from dmz", i);

                        i = 0;
                        foreach (var tok in DMZContext.Tokens)
                        {
                            DMZContext.Tokens.Remove(tok);
                            i++;
                        }

                        Console.WriteLine("{0} tokens deleted from dmz", i);

                        i = 0;
                        foreach (var prof in DMZContext.Profiles)
                        {
                            DMZContext.Profiles.Remove(prof);
                            i++;
                        }
                        Console.WriteLine("{0} profiles deleted from dmz", i);
                        Console.WriteLine("Profiles, tokens and employments deleted");

                        //Setup lookup tables

                        i = 0;
                        foreach (var profile in MasterContext.Persons)
                        {

                            Core.DmzModel.Profile p = new Core.DmzModel.Profile
                            {
                                FirstName = profile.FirstName,
                                LastName = profile.LastName,
                                Id = profile.Id
                            };

                            //If coordinate exists, pick it from the lookup table
                            if (profile.HomeCoordinate != null && person.Gade == profile.HomeCoordinate.Gade)
                            {
                                p.HomeLatitude = profile.HomeCoordinate.Latitude.ToString();
                                p.HomeLongitude = profile.HomeCoordinate.Longitude.ToString();
                            }
                            else // Else look it up
                            {
                                Console.WriteLine("Looking up {0}", person.Gade);

                                try
                                {
                                    NetHelper nh = new NetHelper();
                                    SingleAddressCoordinates c = nh.ConvertSingleUnstructuredAddress(person.Gade, person.PostNr);

                                    p.HomeLatitude = c.Latitude;
                                    p.HomeLongitude = c.Longitude;

                                    if (profile.HomeCoordinate == null)
                                        profile.HomeCoordinate = new HomeCoordinate();

                                    profile.HomeCoordinate.Latitude = decimal.Parse(c.Latitude, CultureInfo.InvariantCulture);
                                    profile.HomeCoordinate.Longitude = decimal.Parse(c.Longitude, CultureInfo.InvariantCulture);
                                    profile.HomeCoordinate.Gade = person.Gade;

                                    try
                                    {
                                        MasterContext.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        //Could not save coordinate to mastermodel.. not a big deal..
                                    }
                                }
                                catch (Exception)
                                {
                                    p.HomeLatitude = "0";
                                    p.HomeLongitude = "0";
                                }
                            }

                            foreach (var emp in profile.Employments)
                            {
                                var e = new Core.DmzModel.Employment
                                {
                                    EmploymentPosition = emp.Position,
                                    ManNr = emp.EmploymentId.ToString(),
                                    Id = emp.Id
                                };

                                e = Encryptor.EncryptEmployment(e);
                                p.Employments.Add(e);
                            }

                            foreach (var tok in profile.MobileTokens)
                            {
                                var t = new Core.DmzModel.Token
                                {
                                    TokenString = tok.Token,
                                    Status = (int)tok.Status,
                                    GuId = tok.Guid.ToString()
                                };

                                t = Encryptor.EncryptToken(t);
                                p.Tokens.Add(t);
                            }
                        }

                        p = Encryptor.EncryptProfile(p);
                        DMZContext.Profiles.Add(p);

                        i++;
                    }

                    try
                    {
                        DMZContext.SaveChanges();
                        Console.WriteLine("{0} profiles inserted", i);
                    }
                    catch (Exception ex)
                    {
                        //Handle Sync Error.. send email?
                        Console.WriteLine("error");
                    }
                }
            }
        }
    }
}
