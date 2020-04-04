using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Common
{
    public enum SignInStatusExt
    {
        //
        // Summary:
        //     Sign in was successful
        Success = 0,
        //
        // Summary:
        //     User is locked out
        LockedOut = 1,
        //
        // Summary:
        //     Sign in requires addition verification (i.e. two factor)
        RequiresVerification = 2,
        //
        // Summary:
        //     Sign in failed
        Failure = 3,
        //
        // Summary:
        //     Username provided didn't exist
        InvalidUsername = 4,
        //
        // Summary:
        //     JWT was malformed and could be decoded properly
        InvalidCredentials = 5,
        //
        // Summary:
        //     JWT was malformed and could be decoded properly
        JWTMalformed = 6,
    }
}