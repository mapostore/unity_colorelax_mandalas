using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEu : MonoBehaviour {

    private static string[] euCountryCode = { "GB", "BE", "EL", "LT", "PT", 
        "BG", "ES", "LU", "RO", "CZ", "FR", "HU", "SI", "DK", "HR", "MT", 
        "SK", "DE", "IT", "NL", "FI", "EE", "CY", "AT", "SE", "IE", 
        "LV", "PL", "UK", "CH", "NO", "IS", "LI" };

    public static bool isEuCountry(string countryToCheck){

        // TODO : set this for debug only :
        // countryToCheck = "IT";

        for (int i = 0; i < euCountryCode.Length;i++){
            if (countryToCheck.Equals(euCountryCode[i]))  {
                return true;
            }
        }
        return false;
    }
}
