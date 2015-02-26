using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
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

                        var licensePlate = dr.Person.LicensePlates.FirstOrDefault();

                        if (licensePlate != null)
                            dr.Licenseplate = licensePlate.ToString();
                        else
                            dr.Licenseplate = "No Licenseplate";

                        //Create ReimburstmentCalculator
                        ReimbursementCalculator calc = new ReimbursementCalculator();
                        dr = calc.Calculate(dr);

                        //Add GPS coordinates to the route
                        foreach (Core.DmzModel.GPSCoordinate gps in report.Route.GPSCoordinates)
                        {
                            Core.DmzModel.GPSCoordinate g = Encryptor.DecryptGPSCoordinate(gps);

                           var drp = new Core.DomainModel.DriveReportPoint
                            {
                                Latitude =  g.Latitude,
                                Longitude = g.Longitude
                            };

                            // What to do with time? - or now just throw aray.. assume gpscoordinatea re in the correct order
                            //c.Time = DateTime.ParseExact(g.TimeStamp, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                            dr.DriveReportPoints.Add(drp);
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

                            //Find profiles home coordinate
                            var personalAddress =
                                profile.PersonalAddresses.FirstOrDefault(x => x.Type == PersonalAddressType.Home);

                            if (personalAddress != null)
                            {
                                p.HomeLatitude = personalAddress.Latitude;
                                p.HomeLongitude = personalAddress.Longitude;
                            }
                            else
                            {
                                p.HomeLatitude = "0";
                                p.HomeLongitude = "0";
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
                        

                            p = Encryptor.EncryptProfile(p);
                            DMZContext.Profiles.Add(p);

                            i++;
                        }
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
