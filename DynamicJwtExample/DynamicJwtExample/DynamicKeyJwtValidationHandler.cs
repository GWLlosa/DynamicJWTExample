using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DynamicJwtExample
{
    public class DynamicKeyJwtValidationHandler : JwtSecurityTokenHandler, ISecurityTokenValidator
    {
        public SecurityKey GetKeyForClaimedId(string claimedId)
        {
            //Here's where we would have the logic to go look up the key in our database.  
            //Omitted for brevity.
            throw new NotImplementedException();
        }
        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            //We can read the token before we've begun validating it.
            JwtSecurityToken incomingToken = ReadJwtToken(token);
            //Extract the external system ID from the token.
            string externalSystemId = incomingToken
                .Claims
                .First(claim => claim.Type == "externalId")
                .Value;
            //Retrieve the corresponding Public Key from our data store
            SecurityKey publicKeyForExternalSystem = GetKeyForClaimedId(externalSystemId);
            //Set our parameters to use the public key we've looked up
            validationParameters.IssuerSigningKey = publicKeyForExternalSystem;
            //And let the framework take it from here.
            return base.ValidateToken(token, validationParameters, out validatedToken);
        }
    }
}
