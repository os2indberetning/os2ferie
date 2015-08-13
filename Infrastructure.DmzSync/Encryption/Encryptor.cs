using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DmzModel;

namespace Infrastructure.DmzSync.Encryption
{
    public static class Encryptor
    {
        public const string EncryptKey = "testpasswordkey";

        public static Profile EncryptProfile(Profile profile)
        {

                profile.FirstName = StringCipher.Encrypt(profile.FirstName, EncryptKey);
                profile.LastName = StringCipher.Encrypt(profile.LastName, EncryptKey);
                profile.HomeLatitude = StringCipher.Encrypt(profile.HomeLatitude, EncryptKey);
                profile.HomeLongitude = StringCipher.Encrypt(profile.HomeLongitude, EncryptKey);
                profile.FullName = StringCipher.Encrypt(profile.FullName, EncryptKey);
                return profile;

        }

        public static Profile DecryptProfile(Profile profile)
        {
            try
            {
                profile.FirstName = StringCipher.Decrypt(profile.FirstName, EncryptKey);
                profile.LastName = StringCipher.Decrypt(profile.LastName, EncryptKey);
                profile.HomeLatitude = StringCipher.Decrypt(profile.HomeLatitude, EncryptKey);
                profile.HomeLongitude = StringCipher.Decrypt(profile.HomeLongitude, EncryptKey);
                profile.FullName = StringCipher.Decrypt(profile.FullName, EncryptKey);
                return profile;               
            }
            catch (FormatException)
            {
                return profile;
            } 
        }

        public static Employment EncryptEmployment(Employment employment)
        {
            employment.EmploymentPosition = StringCipher.Encrypt(employment.EmploymentPosition, EncryptKey);
            return employment;
        }

        public static GPSCoordinate DecryptGPSCoordinate(GPSCoordinate gpscoord)
        {
            gpscoord.Latitude = StringCipher.Decrypt(gpscoord.Latitude, EncryptKey);
            gpscoord.Longitude = StringCipher.Decrypt(gpscoord.Longitude, EncryptKey);
            return gpscoord;
        }

        public static Token EncryptToken(Token token)
        {
            token.GuId = StringCipher.Encrypt(token.GuId, EncryptKey);
            token.TokenString = StringCipher.Encrypt(token.TokenString, EncryptKey);
            return token;
        }

        public static Token DecryptToken(Token token)
        {
            token.GuId = StringCipher.Decrypt(token.GuId, EncryptKey);
            token.TokenString = StringCipher.Decrypt(token.TokenString, EncryptKey);
            return token;
        }

    }
}
